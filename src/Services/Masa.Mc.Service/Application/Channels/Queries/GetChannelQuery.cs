﻿namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetChannelQuery(Guid ChannelId) : Query<ChannelDto>
{
    public override ChannelDto Result { get; set; } = new();
}
