// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class Receiver : ValueObject
{
    public Guid SubjectId { get; protected set; }

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    public string PhoneNumber { get; protected set; } = string.Empty;

    public string Email { get; protected set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return SubjectId;
    }
    
    public Receiver(Guid subjectId, string displayName, string avatar, string phoneNumber, string email)
    {
        SubjectId = subjectId;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public string GetChannelUserIdentity(ChannelTypes channelType)
    {
        switch (channelType)
        {
            case ChannelTypes.Sms:
                return PhoneNumber;
            case ChannelTypes.Email:
                return Email;
            case ChannelTypes.WebsiteMessage:
                return SubjectId.ToString();
            default:
                return string.Empty;
        }
    }
}
