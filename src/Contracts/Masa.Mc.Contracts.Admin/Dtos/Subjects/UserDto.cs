// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Subjects;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? DisplayName { get; set; }

    public string Account { get; set; }

    public GenderTypes Gender { get; set; }

    public string Avatar { get; set; }

    public string? IdCard { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(Name))
        {
            return Name;
        }

        return DisplayName ?? string.Empty;
    }
}
