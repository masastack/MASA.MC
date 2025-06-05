// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Jobs;

public class MessageTaskExecuteJob : SchedulerJob
{
    public override async Task<object?> ExcuteAsync(BuildingBlocks.StackSdks.Scheduler.Model.JobContext context)
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
                options.Mapping(nameof(MasaUser.CurrentTeamId), IdentityClaimConsts.CURRENT_TEAM);
                options.Mapping(nameof(MasaUser.StaffId), IdentityClaimConsts.STAFF);
                options.Mapping(nameof(MasaUser.Account), IdentityClaimConsts.ACCOUNT);
            });
            builder.Services.AddSequentialGuidGenerator();
            //builder.Services.AddMasaConfiguration(configurationBuilder =>
            //{
            //    configurationBuilder.UseDcc();
            //});
            await builder.Services.AddMasaStackConfigAsync(MasaStackProject.MC, MasaStackApp.Service);
            var masaStackConfig = builder.Services.GetMasaStackConfig();
            var configuration = builder.Services.GetMasaConfiguration().ConfigurationApi.GetDefault();
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
            serviceCollection.AddAuthClient(masaStackConfig.GetAuthServiceDomain(), redisOptions);
            serviceCollection.AddMcClient(masaStackConfig.GetMcServiceDomain());
            serviceCollection.AddSchedulerClient(masaStackConfig.GetSchedulerServiceDomain());
            serviceCollection.AddAliyunSms();
            serviceCollection.AddMailKit();
            serviceCollection.AddAppNotification(redisOptions);
            serviceCollection.AddSignalR();
            serviceCollection.AddSingleton<ITemplateRenderer, TextTemplateRenderer>();
            serviceCollection.AddAuthChannelUserFinder();
            serviceCollection.AddDomainEventBus(dispatcherOptions =>
            {
                dispatcherOptions
                .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseDapr().UseEventLog<McDbContext>())
                .UseEventBus(eventBusBuilder =>
                {
                    eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>));
                    eventBusBuilder.UseMiddleware(typeof(LogMiddleware<>));
                })
                .UseUoW<McDbContext>(dbOptions => dbOptions.UseDbSql(masaStackConfig.GetDbType()).UseFilter())
                .UseRepository<McDbContext>();
            }).AddIsolation(isolationBuilder => isolationBuilder.UseMultiEnvironment("env_key"));

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