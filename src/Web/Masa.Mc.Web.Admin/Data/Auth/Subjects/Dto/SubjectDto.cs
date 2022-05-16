// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Auth.Subjects.Dto;

public class SubjectDto
{
    public Guid Id { get; set; }

    public MessageTaskReceiverTypes Type { get; set; }

    public Guid SubjectId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public SubjectDto()
    {

    }

    public SubjectDto(Guid id, MessageTaskReceiverTypes type, string subjectId, string displayName, string avatar, string phoneNumber, string email)
    {
        Id = id;
        Type = type;
        SubjectId = new Guid(subjectId);
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
