// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Infrastructure.Firewall;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();

#if DEBUG
builder.Services.AddDaprStarter(opt =>
{
    opt.DaprHttpPort = 20602;
    opt.DaprGrpcPort = 20601;
});
#endif

builder.Services.AddDaprClient();

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddFirewall(serviceProvider =>
{
    var daprClient = serviceProvider.GetRequiredService<DaprClient>();
    var firewallOptions = daprClient.GetSecretAsync("localsecretstore", "firewallOptions").Result;

    return new FirewallOptions
    {
        IpWhiteList = firewallOptions["ipWhiteList"],
        UrlWhiteList = firewallOptions["urlWhiteList"]
    };
});
builder.Services.AddAliyunStorage(serviceProvider =>
{
    var daprClient = serviceProvider.GetRequiredService<DaprClient>();
    var aliyunOssConfig = daprClient.GetSecretAsync("localsecretstore", "ali-masa-cdn-dev").Result;
    var accessId = aliyunOssConfig["access_id"];
    var accessSecret = aliyunOssConfig["access_secret"];
    var endpoint = aliyunOssConfig["endpoint"];
    var roleArn = aliyunOssConfig["role_arn"];
    return new AliyunStorageOptions(accessId, accessSecret, endpoint, roleArn, "SessionTest")
    {
        Sts = new AliyunStsOptions()
        {
            RegionId = "cn-hangzhou"
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = builder.GetMasaConfiguration().ConfigurationApi.GetDefault().GetValue<string>("AppSettings:IdentityServerUrl");
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.ValidateIssuer = false;
    options.MapInboundClaims = false;
});
builder.Services.AddSequentialGuidGenerator();
builder.AddMasaConfiguration(configurationBuilder =>
{
    configurationBuilder.UseDcc();
});
var configuration = builder.GetMasaConfiguration().ConfigurationApi.GetDefault();
builder.Services.AddAuthClient(configuration.GetValue<string>("AppSettings:AuthClient:Url"));
builder.Services.AddMcClient(configuration.GetValue<string>("AppSettings:McClient:Url"));
builder.Services.AddSchedulerClient(configuration.GetValue<string>("AppSettings:SchedulerClient:Url"));
builder.Services.AddMasaRedisCache(configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>()).AddMasaMemoryCache();
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
    .AddServices(builder);

app.UseFirewall();
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

app.Run();
