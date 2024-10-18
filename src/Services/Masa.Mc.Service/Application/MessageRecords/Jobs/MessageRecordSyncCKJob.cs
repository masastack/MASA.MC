// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Jobs;

public class MessageRecordSyncCKJob : BackgroundJobBase<MessageRecordSyncCKJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public MessageRecordSyncCKJob(ILogger<BackgroundJobBase<MessageRecordSyncCKJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(MessageRecordSyncCKJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var (queryContext, clickHouseContext) = await GetRequiredServiceAsync(scope.ServiceProvider, args.Environment);

        var data = await queryContext.MessageRecordQueries.Where(x => x.CreationTime < args.Time).ToListAsync();

        // 分批次插入数据
        int batchSize = 10000;
        for (int i = 0; i < data.Count; i += batchSize)
        {
            var batch = data.Skip(i).Take(batchSize).ToList();
            await InsertBatchToClickHouseAsync(clickHouseContext, batch);
        }

        Console.WriteLine("Data inserted successfully.");
    }

    private async Task<(IMcQueryContext, McClickHouseContext)> GetRequiredServiceAsync(IServiceProvider serviceProvider, string environment)
    {
        var multiEnvironmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(environment);

        var queryContext = serviceProvider.GetRequiredService<IMcQueryContext>();

        var clickHouseContext = serviceProvider.GetRequiredService<McClickHouseContext>();

        return (queryContext, clickHouseContext);
    }

    static async Task InsertBatchToClickHouseAsync(McClickHouseContext context, List<MessageRecordQueryModel> batch)
    {
        await context.Database.EnsureCreatedAsync();

        context.MessageRecords.AddRange(batch);

        await context.SaveChangesAsync();
    }
}
