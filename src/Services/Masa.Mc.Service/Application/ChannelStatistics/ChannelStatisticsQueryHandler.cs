// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics;

public class ChannelStatisticsQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IDataFilter _dataFilter;
    private readonly ICsvExporter _exporter;

    public ChannelStatisticsQueryHandler(IMcQueryContext context, IDataFilter dataFilter, ICsvExporter exporter)
    {
        _context = context;
        _dataFilter = dataFilter;
        _exporter = exporter;
    }

    [EventHandler]
    public async Task GetAppVendorSendStatisticsAsync(GetAppVendorSendStatisticsQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var input = query.Input;
        var records = ApplyBaseFilter(_context.MessageRecordQueries.Include(x => x.Channel).AsNoTracking(), input)
            .Where(x => x.Channel.Type == ChannelTypes.App)
            .Where(x => x.ChannelId.HasValue);

        var receiverUsers = _context.MessageReceiverUserQueries.AsNoTracking();
        if (input.Vendor.HasValue)
        {
            var vendorPlatform = input.Vendor.Value.ToString();
            receiverUsers = receiverUsers.Where(x => x.Platform == vendorPlatform);
        }

        var grouped = await (from record in records
                             join receiver in receiverUsers
                                 on new { record.MessageTaskHistoryId, record.ChannelUserIdentity }
                                 equals new { receiver.MessageTaskHistoryId, receiver.ChannelUserIdentity } into receiverGroup
                             from receiver in receiverGroup.DefaultIfEmpty()
                             select new
                             {
                                 record.Success,
                                 Platform = receiver != null ? receiver.Platform : string.Empty
                             })
            .GroupBy(x => x.Platform)
            .Select(g => new VendorStat(
                g.Key,
                g.LongCount(),
                g.LongCount(x => x.Success == true),
                g.LongCount(x => x.Success == false),
                g.LongCount(x => x.Success == null)))
            .ToListAsync();

        var result = new List<AppVendorSendStatisticsDto>();
        foreach (var vendor in Enum.GetValues<AppVendor>())
        {
            var platform = vendor.ToString();
            var stat = grouped.FirstOrDefault(x => x.Platform == platform);
            result.Add(BuildVendorStatistics(vendor, vendor.ToString(), stat));
        }

        var unknownStats = grouped
            .Where(x => string.IsNullOrWhiteSpace(x.Platform) || !Enum.TryParse<AppVendor>(x.Platform, out _))
            .ToList();
        if (unknownStats.Count > 0)
        {
            var total = unknownStats.Sum(x => x.TotalCount);
            var success = unknownStats.Sum(x => x.SuccessCount);
            var fail = unknownStats.Sum(x => x.FailCount);
            var noReceipt = unknownStats.Sum(x => x.NoReceiptCount);
            result.Add(new AppVendorSendStatisticsDto
            {
                Vendor = null,
                VendorName = "Unknown",
                TotalCount = total,
                SuccessCount = success,
                FailCount = fail,
                NoReceiptCount = noReceipt,
                SuccessRate = CalculateSuccessRate(success, total)
            });
        }

        if (input.Vendor.HasValue)
        {
            result = result.Where(x => x.Vendor == input.Vendor).ToList();
        }

        query.Result = result;
    }

    [EventHandler]
    public async Task GetChannelSendStatisticsAsync(GetChannelSendStatisticsQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var input = query.Input;
        var records = ApplyBaseFilter(_context.MessageRecordQueries.Include(x => x.Channel).AsNoTracking(), input);
        records = ApplyVendorFilter(records, input);

        var summary = await BuildSummaryAsync(records);
        query.Result = summary;
    }

    [EventHandler]
    public async Task GetChannelSendTrendAsync(GetChannelSendTrendQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var input = query.Input;
        var records = ApplyBaseFilter(_context.MessageRecordQueries.AsNoTracking(), input)
            .Where(x => x.SendTime.HasValue);
        records = ApplyVendorFilter(records, input);

        var result = await records
            .GroupBy(x => x.SendTime!.Value.Date)
            .Select(g => new ChannelSendTrendDto
            {
                Date = g.Key,
                TotalCount = g.LongCount(),
                SuccessCount = g.LongCount(x => x.Success == true),
                FailCount = g.LongCount(x => x.Success == false),
                NoReceiptCount = g.LongCount(x => x.Success == null)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        query.Result = result;
    }

    [EventHandler]
    public async Task GetChannelFailureReasonOverviewAsync(GetChannelFailureReasonOverviewQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var input = query.Input;
        var records = ApplyBaseFilter(_context.MessageRecordQueries.AsNoTracking(), input);
        records = ApplyVendorFilter(records, input);

        var result = await records
            .Where(x => x.Success == false)
            .GroupBy(x => string.IsNullOrWhiteSpace(x.FailureReason) ? "Unknown" : x.FailureReason)
            .Select(g => new ChannelFailureReasonOverviewDto
            {
                FailureReason = g.Key,
                Count = g.LongCount()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        query.Result = result;
    }

    [EventHandler]
    public async Task ExportChannelFailureReasonDetailsAsync(ExportChannelFailureReasonDetailsQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var input = query.Input;
        var records = ApplyBaseFilter(_context.MessageRecordQueries.AsNoTracking(), input);
        records = ApplyVendorFilter(records, input);

        var exportItems = await records
            .Where(x => x.Success == false)
            .OrderByDescending(x => x.SendTime ?? x.ExpectSendTime ?? DateTimeOffset.MinValue)
            .Select(x => new FailureReasonDetailExportItem
            {
                DisplayName = x.DisplayName,
                ChannelUserIdentity = x.ChannelUserIdentity,
                FailureReason = string.IsNullOrWhiteSpace(x.FailureReason) ? "Unknown" : x.FailureReason,
                ExpectSendTime = x.ExpectSendTime,
                SendTime = x.SendTime,
                MessageId = x.MessageId
            })
            .ToListAsync();

        query.Result = await _exporter.ExportAsByteArray(exportItems);
    }

    private static AppVendorSendStatisticsDto BuildVendorStatistics(AppVendor vendor, string vendorName, VendorStat? stat)
    {
        var total = stat?.TotalCount ?? 0L;
        var success = stat?.SuccessCount ?? 0L;
        var fail = stat?.FailCount ?? 0L;
        var noReceipt = stat?.NoReceiptCount ?? 0L;

        return new AppVendorSendStatisticsDto
        {
            Vendor = vendor,
            VendorName = vendorName,
            TotalCount = total,
            SuccessCount = success,
            FailCount = fail,
            NoReceiptCount = noReceipt,
            SuccessRate = CalculateSuccessRate(success, total)
        };
    }

    private static decimal CalculateSuccessRate(long successCount, long totalCount)
    {
        if (totalCount == 0)
        {
            return 0;
        }

        return Math.Round(successCount * 100m / totalCount, 2, MidpointRounding.AwayFromZero);
    }

    private IQueryable<MessageRecordQueryModel> ApplyBaseFilter(IQueryable<MessageRecordQueryModel> query, ChannelSendStatisticsInputDto input)
    {
        query = query.Where(x => x.ChannelId.HasValue);
        query = query.Where(x => !input.ChannelId.HasValue || x.ChannelId == input.ChannelId);
        query = query.Where(x => !input.TemplateId.HasValue
            || (x.MessageTask.EntityType == MessageEntityTypes.Template && x.MessageTask.EntityId == input.TemplateId));
        query = query.Where(x => !input.StartTime.HasValue || x.SendTime >= input.StartTime);
        query = query.Where(x => !input.EndTime.HasValue || x.SendTime <= input.EndTime);
        return query;
    }

    private IQueryable<MessageRecordQueryModel> ApplyVendorFilter(IQueryable<MessageRecordQueryModel> records, ChannelSendStatisticsInputDto input)
    {
        if (!input.Vendor.HasValue)
        {
            return records;
        }

        var platform = input.Vendor.Value.ToString();
        var receiverUsers = _context.MessageReceiverUserQueries.AsNoTracking()
            .Where(x => x.Platform == platform);

        return from record in records
               join receiver in receiverUsers
                   on new { record.MessageTaskHistoryId, record.ChannelUserIdentity }
                   equals new { receiver.MessageTaskHistoryId, receiver.ChannelUserIdentity }
               select record;
    }

    private static async Task<ChannelSendStatisticsDto> BuildSummaryAsync(IQueryable<MessageRecordQueryModel> records)
    {
        var summary = await records
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalCount = g.LongCount(),
                SuccessCount = g.LongCount(x => x.Success == true),
                FailCount = g.LongCount(x => x.Success == false),
                NoReceiptCount = g.LongCount(x => x.Success == null)
            })
            .FirstOrDefaultAsync();

        if (summary == null)
        {
            return new ChannelSendStatisticsDto();
        }

        return new ChannelSendStatisticsDto
        {
            TotalCount = summary.TotalCount,
            SuccessCount = summary.SuccessCount,
            FailCount = summary.FailCount,
            NoReceiptCount = summary.NoReceiptCount,
            SuccessRate = CalculateSuccessRate(summary.SuccessCount, summary.TotalCount)
        };
    }

    private sealed record VendorStat(string Platform, long TotalCount, long SuccessCount, long FailCount, long NoReceiptCount);

}
