// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class ChannelStatisticsService : ServiceBase
{
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    public ChannelStatisticsService(IServiceCollection services) : base("api/channel-statistics")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    [RoutePattern("app/vendors", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<AppVendorSendStatisticsDto>> GetAppVendorSendStatisticsAsync(
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? templateId,
        [FromQuery] AppVendor? vendor,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var input = new ChannelSendStatisticsInputDto
        {
            ChannelId = channelId,
            TemplateId = templateId,
            Vendor = vendor,
            StartTime = startTime,
            EndTime = endTime
        };
        var query = new GetAppVendorSendStatisticsQuery(input);
        await EventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("channel", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<ChannelSendStatisticsDto> GetChannelSendStatisticsAsync(
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? templateId,
        [FromQuery] AppVendor? vendor,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var input = new ChannelSendStatisticsInputDto
        {
            ChannelId = channelId,
            TemplateId = templateId,
            Vendor = vendor,
            StartTime = startTime,
            EndTime = endTime
        };
        var query = new GetChannelSendStatisticsQuery(input);
        await EventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("trend", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<ChannelSendTrendDto>> GetChannelSendTrendAsync(
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? templateId,
        [FromQuery] AppVendor? vendor,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var input = new ChannelSendStatisticsInputDto
        {
            ChannelId = channelId,
            TemplateId = templateId,
            Vendor = vendor,
            StartTime = startTime,
            EndTime = endTime
        };
        var query = new GetChannelSendTrendQuery(input);
        await EventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("failure-reasons", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<ChannelFailureReasonOverviewDto>> GetChannelFailureReasonOverviewAsync(
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? templateId,
        [FromQuery] AppVendor? vendor,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var input = new ChannelSendStatisticsInputDto
        {
            ChannelId = channelId,
            TemplateId = templateId,
            Vendor = vendor,
            StartTime = startTime,
            EndTime = endTime
        };
        var query = new GetChannelFailureReasonOverviewQuery(input);
        await EventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("failure-reasons/export", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<byte[]> ExportChannelFailureReasonDetailsAsync(
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? templateId,
        [FromQuery] AppVendor? vendor,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var input = new ChannelSendStatisticsInputDto
        {
            ChannelId = channelId,
            TemplateId = templateId,
            Vendor = vendor,
            StartTime = startTime,
            EndTime = endTime
        };
        var query = new ExportChannelFailureReasonDetailsQuery(input);
        await EventBus.PublishAsync(query);
        return query.Result;
    }
}
