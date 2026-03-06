// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class GetSmsInboundInputDto : PaginatedOptionsDto
{
    public Guid ChannelId { get; set; }

    public string Mobile { get; set; } = string.Empty;

    public string AddSerial { get; set; } = string.Empty;

    public string SmsContent { get; set; } = string.Empty;


    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public GetSmsInboundInputDto()
    {
    }

    public GetSmsInboundInputDto(Guid channelId, string mobile, string addSerial, string smsContent, DateTimeOffset? startTime, DateTimeOffset? endTime, string sorting, int page, int pageSize)
        : base(sorting, page, pageSize)
    {
        ChannelId = channelId;
        Mobile = mobile;
        AddSerial = addSerial;
        SmsContent = smsContent;
        StartTime = startTime;
        EndTime = endTime;
    }
}
