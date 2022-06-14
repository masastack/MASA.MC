// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Subjects;

public class GetSubjectInputDto
{
    public string Filter { get; set; } = string.Empty;

    public GetSubjectInputDto(string filter)
    {
        Filter = filter;
    }
}
