// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
builder.AddObservability();

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 20602;
    opt.DaprGrpcPort = 20601;
});
#endif

builder.Services.AddAutoInject();
builder.Services.AddDaprClient();

builder.Services.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
var ossOptions = publicConfiguration.GetSection("$public.OSS").Get<OssOptions>();
builder.Services.AddAliyunStorage(new AliyunStorageOptions(ossOptions.AccessId, ossOptions.AccessSecret, ossOptions.Endpoint, ossOptions.RoleArn, ossOptions.RoleSessionName)
{
    Sts = new AliyunStsOptions()
    {
        RegionId = ossOptions.RegionId
    }
});
builder.Services.AddMasaIdentity(options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = publicConfiguration.GetValue<string>("$public.AppSettings:IdentityServerUrl");
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.MapInboundClaims = false;
});
builder.Services.AddSequentialGuidGenerator();

var redisOptions = publicConfiguration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
var configuration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddAuthClient(publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:Url"), redisOptions);
builder.Services.AddMcClient(publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"));
builder.Services.AddSchedulerClient(publicConfiguration.GetValue<string>("$public.AppSettings:SchedulerClient:Url"));
builder.Services.AddStackExchangeRedisCache(redisOptions)
    .AddMultilevelCache();
builder.Services.AddAliyunSms();
builder.Services.AddMailKit();
builder.Services.AddCsv();
builder.Services.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
builder.Services.AddTransient<Microsoft.AspNetCore.SignalR.IUserIdProvider, McUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddTransient<NotificationsHub>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));


builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("A healthy result."))
    .AddDbContextCheck<McDbContext>();

var app = builder.Services
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
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
        .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<McDbContext>())
        .UseEventBus(eventBusBuilder =>
        {
            eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
        })
        .UseIsolationUoW<McDbContext>(
            isolationBuilder => isolationBuilder.UseMultiEnvironment("env_key"),
            dbOptions => dbOptions.UseSqlServer().UseFilter())
        .UseRepository<McDbContext>();
    })
    .AddServices(builder, options =>
    {
        options.GetPrefixes = new() { "Get", "Select", "Find" };
        options.MapHttpMethodsForUnmatched = new string[] { "Post" };
    });
app.UseMiddleware<AdminSafeListMiddleware>(configuration.GetSection("WhiteListOptions").Get<WhiteListOptions>());
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

app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapHub<NotificationsHub>("/signalr-hubs/notifications");
});

app.UseHttpsRedirection();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.Run();
