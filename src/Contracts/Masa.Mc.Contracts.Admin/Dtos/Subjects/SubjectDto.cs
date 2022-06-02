// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Subjects;

public class SubjectDto
{
    public Guid SubjectId { get; set; }

    public string Name { get; set; }

    public string? DisplayName { get; set; }

    public string? Avatar { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public SubjectTypes SubjectType { get; set; }
}
