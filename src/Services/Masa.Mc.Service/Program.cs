﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Dispatcher.IntegrationEvents;

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
builder.Services.AddActors(options =>
{
});
builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
});
builder.Services.AddAuthClient(builder.Configuration.GetValue<string>("AuthClient:Url"));
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
.AddJwtBearer(options =>
{
    options.Authority = "";
    options.RequireHttpsMetadata = false;
    options.Audience = "";
});
builder.Services.AddMasaRedisCache(builder.Configuration.GetSection("RedisConfig")).AddMasaMemoryCache();
builder.Services.AddAliyunSms();
builder.Services.AddMailKit();
builder.Services.AddCsv();
builder.Services.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
builder.Services.AddTransient<Microsoft.AspNetCore.SignalR.IUserIdProvider, McUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddTransient<NotificationsHub>();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly(), Assembly.Load("Masa.Mc.Contracts.Admin"));

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
    .AddTransient(typeof(IMiddleware<>), typeof(LogMiddleware<>))
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .AddTransient(typeof(IMiddleware<>), typeof(ValidatorMiddleware<>))
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
        .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<McDbContext>())
        .UseEventBus(eventBusBuilder =>
        {
            eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
            eventBusBuilder.UseMiddleware(typeof(LogMiddleware<>));
        })
        .UseIsolationUoW<McDbContext>(
            isolationBuilder => isolationBuilder.UseMultiEnvironment("env"),
            dbOptions => dbOptions.UseSqlServer().UseFilter())
        .UseRepository<McDbContext>();
    })
    .AddServices(builder);
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
    endpoints.MapActorsHandlers();
    endpoints.MapHub<NotificationsHub>("/signalr-hubs/notifications");
});

app.UseHttpsRedirection();

app.Run();
