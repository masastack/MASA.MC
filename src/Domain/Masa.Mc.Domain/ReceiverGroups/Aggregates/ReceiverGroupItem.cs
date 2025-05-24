// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroupItem : Entity<Guid>
{
    public Guid GroupId { get; protected set; }

    public ReceiverGroupItemTypes Type { get; protected set; }

    public Guid SubjectId { get; protected set; }

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    private ReceiverGroupItem() { }

    public ReceiverGroupItem(Guid groupId, ReceiverGroupItemTypes type, Guid subjectId, string displayName, string avatar)
    {
        GroupId = groupId;
        Type = type;
        SubjectId = subjectId;
        DisplayName = displayName;
        Avatar = avatar;
    }
}
