// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class MessageRecordUserDto
{
    public string DisplayName { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public MessageRecordUserDto() { }

    public MessageRecordUserDto(string channelUserIdentity, ChannelTypes type)
    {
        switch (type)
        {
            case ChannelTypes.Sms:
                PhoneNumber = channelUserIdentity;
                break;
            case ChannelTypes.Email:
                PhoneNumber = Email;
                break;
            default:
                break;
        }
    }
}
