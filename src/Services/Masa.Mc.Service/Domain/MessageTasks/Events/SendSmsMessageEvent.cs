// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Events;

public record SendSmsMessageEvent : SendMessageEvent
{
    public List<MessageRecord> MessageRecords { get; set; } = new();

    public MessageTemplate MessageTemplate { get; set; } = default!;

    public List<Dictionary<string, ExtraPropertyDictionary>> PhoneNumberVariables = new();

    public string Sign { get; set; }

    public SendSmsMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        : base(channelId, messageData, messageTaskHistory)
    {

    }

    public void AddPhoneNumberVariable(string key, ExtraPropertyDictionary value)
    {
        bool keyExistsInAllDictionaries = true;

        foreach (var dict in PhoneNumberVariables)
        {
            if (!dict.ContainsKey(key))
            {
                dict[key] = value;
                keyExistsInAllDictionaries = false;
                break;
            }
        }

        if (keyExistsInAllDictionaries)
        {
            var newDict = new Dictionary<string, ExtraPropertyDictionary>
            {
                { key, value }
            };
            PhoneNumberVariables.Add(newDict);
        }
    }
}