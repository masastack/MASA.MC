// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Events;

public record SendSmsMessageEvent : SendMessageEvent
{
    public List<MessageRecord> MessageRecords { get; set; } = new();

    public MessageTemplate MessageTemplate { get; set; } = default!;

    public List<string> PhoneNumbers { get; set; }= new ();

    public List<ExtraPropertyDictionary> Variables { get; set; } = new();

    public string Sign { get; set; }

    public SendSmsMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        : base(channelId, messageData, messageTaskHistory)
    {

    }
}