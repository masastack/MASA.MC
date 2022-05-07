// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Events;

//public record CreateMessageEvent : DomainEvent
//{
//    public Guid ChannelId { get; set; }
//    public Guid MessageTaskId { get; set; }
//    public Guid MessageTaskHistoryId { get; set; }
//    public IEnumerable<Guid> UserIds { get; set; }

//    public CreateMessageEvent(Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, IEnumerable<Guid> userIds)
//    {
//        ChannelId = channelId;
//        MessageTaskId = messageTaskId;
//        MessageTaskHistoryId = messageTaskHistoryId;
//        UserIds = userIds;
//    }

//    public CreateMessageEvent(Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, Guid userId)
//    {
//        ChannelId = channelId;
//        MessageTaskId = messageTaskId;
//        MessageTaskHistoryId = messageTaskHistoryId;
//        UserIds = new List<Guid> { userId };
//    }
//}
public record CreateMessageEvent(Guid ChannelId, MessageData MessageData, MessageTaskHistory MessageTaskHistory) : DomainEvent
{

}