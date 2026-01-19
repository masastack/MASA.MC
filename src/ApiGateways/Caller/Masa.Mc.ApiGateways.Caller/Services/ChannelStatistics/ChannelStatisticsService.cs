// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.ChannelStatistics;

public class ChannelStatisticsService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ChannelStatisticsService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/channel-statistics";
    }

    public async Task<List<AppVendorSendStatisticsDto>> GetAppVendorSendStatisticsAsync(ChannelSendStatisticsInputDto inputDto)
    {
        return await GetAsync<ChannelSendStatisticsInputDto, List<AppVendorSendStatisticsDto>>("app/vendors", inputDto) ?? new();
    }

    public async Task<ChannelSendStatisticsDto?> GetChannelSendStatisticsAsync(ChannelSendStatisticsInputDto inputDto)
    {
        return await GetAsync<ChannelSendStatisticsInputDto, ChannelSendStatisticsDto>("channel", inputDto);
    }

    public async Task<List<ChannelSendTrendDto>> GetChannelSendTrendAsync(ChannelSendStatisticsInputDto inputDto)
    {
        return await GetAsync<ChannelSendStatisticsInputDto, List<ChannelSendTrendDto>>("trend", inputDto) ?? new();
    }

    public async Task<List<ChannelFailureReasonOverviewDto>> GetChannelFailureReasonOverviewAsync(ChannelSendStatisticsInputDto inputDto)
    {
        return await GetAsync<ChannelSendStatisticsInputDto, List<ChannelFailureReasonOverviewDto>>("failure-reasons", inputDto) ?? new();
    }

    public async Task<byte[]> ExportChannelFailureReasonDetailsAsync(ChannelSendStatisticsInputDto inputDto)
    {
        return await GetAsync<ChannelSendStatisticsInputDto, byte[]>("failure-reasons/export", inputDto) ?? Array.Empty<byte>();
    }
}
