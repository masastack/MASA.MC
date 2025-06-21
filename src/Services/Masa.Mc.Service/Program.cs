// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync(MasaStackProject.MC, MasaStackApp.Service);
var masaStackConfig = builder.Services.GetMasaStackConfig();

var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();

var identityServerUrl = masaStackConfig.GetSsoDomain();

builder.Services.AddObservable(builder.Logging, new MasaObservableOptions
{
    ServiceNameSpace = builder.Environment.EnvironmentName,
    ServiceVersion = masaStackConfig.Version,
    ServiceName = masaStackConfig.GetServiceId(MasaStackProject.MC),
    Layer = masaStackConfig.Namespace,
    ServiceInstanceId = builder.Configuration.GetValue<string>("HOSTNAME")!
}, masaStackConfig.OtlpUrl, activitySources: new string[] { MessageTaskExecuteJobConsts.ActivitySource.Name });

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 20602;
    opt.DaprGrpcPort = 20601;
});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();

builder.Services.AddObjectStorage(option => option.UseAliyunStorage());

builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
    options.Mapping(nameof(McUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(McUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(McUser.Account), IdentityClaimConsts.ACCOUNT);
    options.Mapping(nameof(McUser.ClientId), "client_id");
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = identityServerUrl;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.MapInboundClaims = false;
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (
            sender,
            certificate,
            chain,
            sslPolicyErrors) =>
        { return true; }
    };
});
builder.Services.AddSequentialGuidGenerator();
builder.Services.AddI18n(Path.Combine("Assets", "I18n"));
var redisOptions = new RedisConfigurationOptions
{
    Servers = new List<RedisServerOptions> {
        new RedisServerOptions()
        {
            Host= masaStackConfig.RedisModel.RedisHost,
            Port=   masaStackConfig.RedisModel.RedisPort
        }
    },
    DefaultDatabase = masaStackConfig.RedisModel.RedisDb,
    Password = masaStackConfig.RedisModel.RedisPassword,
    ClientName = builder.Configuration.GetValue<string>("HOSTNAME") ?? masaStackConfig.GetServiceId(MasaStackProject.MC)
};
var configuration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddCache(redisOptions);
builder.Services.AddScoped<ITokenGenerater, TokenGenerater>();
builder.Services.AddAuthClient(masaStackConfig.GetAuthServiceDomain(), redisOptions);
builder.Services.AddMcClient(masaStackConfig.GetMcServiceDomain());
builder.Services.AddPmClient(masaStackConfig.GetPmServiceDomain());
builder.Services.AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain());
builder.Services.AddAliyunSms();
builder.Services.AddMailKit();
builder.Services.AddAppNotification(redisOptions);
builder.Services.AddWeixinWork(builder.Configuration);
builder.Services.AddCsv();
builder.Services.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
builder.Services.AddTransient<IUserIdProvider, McUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddTransient<NotificationsHub>();
builder.Services.AddAuthChannelUserFinder();
builder.Services.AddMessageTaskHttpJobService();
builder.Services.AddBackgroundJob(options =>
{
    options.UseInMemoryDatabase(_ =>
    {
        _.MaxRetryTimes = 1;
    }, serviceProvider => serviceProvider.GetService<IIdGenerator<Guid>>()!);
});
var mock = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault().GetValue<bool>("Mock:Enable");
if (mock)
{
    builder.Services.ConfigureMockService();
}

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));

builder.Services
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    })
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .AddMasaDbContext<McDbContext>(builder =>
    {
        builder.UseDbSql(masaStackConfig.GetDbType());
        builder.UseFilter(options => options.EnableSoftDelete = true);
    })
    .AddMasaDbContext<McQueryContext>(builder =>
    {
        builder.UseDbSql(masaStackConfig.GetDbType());
        builder.UseFilter(options => options.EnableSoftDelete = true);
    })
    .AddScoped<IMcQueryContext, McQueryContext>()
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
        .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<McDbContext>())
        .UseEventBus(eventBusBuilder =>
        {
            eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
        })
        .UseUoW<McDbContext>()
        .UseRepository<McDbContext>();
    });

await builder.Services.AddStackIsolationAsync(MasaStackProject.MC.Name);

builder.Services.AddStackMiddleware();
await builder.MigrateDbContextAsync<McDbContext>();

var app = builder.AddServices(options =>
{
    options.MapHttpMethodsForUnmatched = new string[] { "Post" };
});

app.UseMiddleware<AdminSafeListMiddleware>(publicConfiguration.GetSection("$public.WhiteListOptions").Get<WhiteListOptions>());

app.UseI18n();
app.UseWeixinWork(app.Environment);
app.UseMasaExceptionHandler(opt =>
{
    opt.ExceptionHandler = context =>
    {
        if (context.Exception is ValidationException validationException)
        {
            context.ToResult(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
    };
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStackIsolation();
app.UseStackMiddleware();
app.UseCloudEvents();
app.UseMasaCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapHub<NotificationsHub>("/signalr-hubs/notifications");
});
app.UseHttpsRedirection();

app.Run();