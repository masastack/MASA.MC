// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Jobs;

public class MessageTaskExecuteJob : ISchedulerJob
{
    public async Task BeforeExcuteAsync(JobContext context)
    {
        await Task.CompletedTask;
    }

    public async Task AfterExcuteAsync(JobContext context)
    {
        await Task.CompletedTask;
    }

    public async Task<object?> ExcuteAsync(string messageTaskId)
    {
        try
        {
            var serviceCollection = new ServiceCollection();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(assemblyPath);

            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .Build();

            var connectstr = config.GetValue<string>("ConnectionStrings:DefaultConnection");

            serviceCollection.AddDomainEventBus(dispatcherOptions =>
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
                    dbOptions => dbOptions.UseSqlServer(connectstr).UseFilter())
                .UseRepository<McDbContext>();
            });

            serviceCollection.AddLogging();

            var provider = serviceCollection.BuildServiceProvider();
            var eventBus = provider.GetRequiredService<IDomainEventBus>();

            await eventBus.PublishAsync(new ExecuteMessageTaskEvent(Guid.Parse(messageTaskId)));

            Console.WriteLine("messageTaskId:" + messageTaskId);

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