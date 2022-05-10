// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroupItem : Entity<Guid>
{
    public Guid GroupId { get; protected set; }

    public Guid SubjectId { get; protected set; }

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    public string PhoneNumber { get; protected set; } = string.Empty;

    public string Email { get; protected set; } = string.Empty;

    public ReceiverGroupItemTypes Type { get; protected set; }

    public ReceiverGroupItem(Guid groupId, Guid subjectId, ReceiverGroupItemTypes type, string displayName, string avatar = "", string phoneNumber = "", string email = "")
    {
        GroupId = groupId;
        SubjectId = subjectId;
        Type = type;

        SetContent(displayName, avatar, phoneNumber, email);
    }

    public void SetContent(string displayName, string avatar, string phoneNumber, string email)
    {
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
