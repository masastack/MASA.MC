﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskReceiverDto
{
    public Guid SubjectId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public MessageTaskReceiverTypes Type { get; set; }

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public void SetChannelUserIdentity(ChannelTypes channelType, string channelUserIdentity)
    {
        switch (channelType)
        {
            case ChannelTypes.Email:
                Email = channelUserIdentity;
                break;
            case ChannelTypes.Sms:
                PhoneNumber = channelUserIdentity;
                break;
            case ChannelTypes.WebsiteMessage:
                SubjectId = new Guid(channelUserIdentity);
                break;
            default:
                break;
        }
    }
}
