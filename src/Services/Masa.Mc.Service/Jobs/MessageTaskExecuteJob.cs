// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Jobs;

public class MessageTaskExecuteJob : SchedulerJob
{
    public override async Task<object?> ExcuteAsync(JobContext context)
    {
        try
        {
            var builder = WebApplication.CreateBuilder();
            var serviceCollection = builder.Services;

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            env = string.IsNullOrWhiteSpace(env) ? "Development" : env;
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(assemblyPath);
            builder.Configuration.SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
            builder.Services.AddMasaIdentity(options =>
            {
                options.Environment = "environment";
                options.UserName = "name";
                options.UserId = "sub";
            });
            builder.Services.AddSequentialGuidGenerator();
            builder.Services.AddMasaConfiguration(configurationBuilder =>
            {
                configurationBuilder.UseDcc();
            });
            var configuration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
            var publicConfiguration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetPublic();
            var redisOptions = publicConfiguration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
            serviceCollection.AddAuthClient(publicConfiguration.GetValue<string>("$public.AppSettings:AuthClient:Url"), redisOptions);
            serviceCollection.AddMcClient(publicConfiguration.GetValue<string>("$public.AppSettings:McClient:Url"));
            serviceCollection.AddSchedulerClient(publicConfiguration.GetValue<string>("$public.AppSettings:SchedulerClient:Url"));
            serviceCollection.AddAliyunSms();
            serviceCollection.AddMailKit();
            serviceCollection.AddSignalR();
            serviceCollection.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
            serviceCollection.AddDomainEventBus(dispatcherOptions =>
            {
                dispatcherOptions
                .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<McDbContext>())
                .UseEventBus(eventBusBuilder =>
                {
                    eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
                    eventBusBuilder.UseMiddleware(typeof(LogMiddleware<>));
                })
                .UseIsolationUoW<McDbContext>(
                    isolationBuilder => isolationBuilder.UseMultiEnvironment("env_key"),
                    dbOptions => dbOptions.UseSqlServer().UseFilter())
                .UseRepository<McDbContext>();
            });

            serviceCollection.AddLogging();

            var provider = serviceCollection.BuildServiceProvider();
            var eventBus = provider.GetRequiredService<IDomainEventBus>();

            var messageId = context.ExcuteParameters[0];
            await eventBus.PublishAsync(new ExecuteMessageTaskEvent(Guid.Parse(messageId), false, context.JobId, context.TaskId));
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            throw;
        }
    }
}