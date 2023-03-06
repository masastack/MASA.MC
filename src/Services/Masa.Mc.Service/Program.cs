// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
GlobalValidationOptions.SetDefaultCulture("zh-CN");

await builder.Services.AddMasaStackConfigAsync();
var masaStackConfig = builder.Services.GetMasaStackConfig();

var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();

var identityServerUrl = masaStackConfig.GetSsoDomain();

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 20602;
    opt.DaprGrpcPort = 20601;
});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();

var ossOptions = publicConfiguration.GetSection("$public.OSS").Get<OssOptions>();
builder.Services.AddObjectStorage(option => option.UseAliyunStorage(new AliyunStorageOptions(ossOptions.AccessId, ossOptions.AccessSecret, ossOptions.Endpoint, ossOptions.RoleArn, ossOptions.RoleSessionName)
{
    Sts = new AliyunStsOptions()
    {
        RegionId = ossOptions.RegionId
    }
}));

builder.Services.AddObservable(builder.Logging, () =>
{
    return new MasaObservableOptions
    {
        ServiceNameSpace = builder.Environment.EnvironmentName,
        ServiceVersion = masaStackConfig.Version,
        ServiceName = masaStackConfig.GetServerId(MasaStackConstant.MC)
    };
}, () =>
{
    return masaStackConfig.OtlpUrl;
});
builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
    options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
    options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
    options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
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
    Password = masaStackConfig.RedisModel.RedisPassword
};
var configuration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddAuthClient(masaStackConfig.GetAuthServiceDomain(), redisOptions);
builder.Services.AddMcClient(masaStackConfig.GetMcServiceDomain());
builder.Services.AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain());
builder.Services.AddMultilevelCache(options => options.UseStackExchangeRedisCache(redisOptions));
builder.Services.AddAliyunSms();
builder.Services.AddMailKit();
builder.Services.AddGetui();
builder.Services.AddCsv();
builder.Services.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
builder.Services.AddTransient<Microsoft.AspNetCore.SignalR.IUserIdProvider, McUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddTransient<NotificationsHub>();
builder.Services.AddAuthChannelUserFinder();
builder.Services.AddMessageTaskHttpJobService();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("A healthy result."))
    .AddDbContextCheck<McDbContext>();

builder.Services.AddScoped(service =>
{
    var content = service.GetRequiredService<IHttpContextAccessor>();
    AuthenticationHeaderValue.TryParse(content.HttpContext?.Request.Headers.Authorization.ToString(), out var auth);
    return new TokenProvider { AccessToken = auth?.Parameter };
});

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
        builder.UseSqlServer(masaStackConfig.GetConnectionString(AppSettings.Get("DBName")));
        builder.UseFilter(options => options.EnableSoftDelete = true);
    })
    .AddMasaDbContext<McQueryContext>(builder =>
    {
        builder.UseSqlServer(masaStackConfig.GetConnectionString(AppSettings.Get("DBName")));
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
        .UseIsolationUoW<McDbContext>(isolationBuilder => isolationBuilder.UseMultiEnvironment("env_key"), null)
        .UseRepository<McDbContext>();
    });

builder.Services.AddStackMiddleware();
await builder.MigrateDbContextAsync<McDbContext>();

var app = builder.AddServices(options =>
{
    options.MapHttpMethodsForUnmatched = new string[] { "Post" };
});

app.UseMiddleware<AdminSafeListMiddleware>(publicConfiguration.GetSection("$public.WhiteListOptions").Get<WhiteListOptions>());

app.UseI18n();

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
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAddStackMiddleware();
app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapHub<NotificationsHub>("/signalr-hubs/notifications");
});

app.UseHttpsRedirection();

app.Run();
