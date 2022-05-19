// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();
builder.Services.AddActors(options =>
{
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
builder.Services.AddAliyunSms();
builder.Services.AddEmail();
builder.Services.AddCsv();
builder.Services.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
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
        .UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<McDbContext>())
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
    //.AddDomainEventBus(dispatcherOptions =>
    //{
    //    dispatcherOptions
    //    .UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<McDbContext>())
    //    .UseEventBus(eventBusBuilder =>
    //    {
    //        eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
    //        eventBusBuilder.UseMiddleware(typeof(LogMiddleware<>));
    //    })
    //    .UseIsolationUoW<McDbContext>(
    //        isolationBuilder => isolationBuilder.UseMultiEnvironment("env"),
    //        dbOptions => dbOptions.UseSqlServer().UseFilter())
    //    .UseRepository<McDbContext>();
    //})
    .AddServices(builder);
app.UseMasaExceptionHandling(opt =>
{
    opt.CustomExceptionHandler = exception =>
    {
        Exception friendlyException = exception;
        if (exception is ValidationException validationException)
        {
            friendlyException = new UserFriendlyException(validationException.Errors.Select(err => err.ToString()).FirstOrDefault()!);
        }
        return (friendlyException, false);
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
});

app.UseHttpsRedirection();

app.Run();
