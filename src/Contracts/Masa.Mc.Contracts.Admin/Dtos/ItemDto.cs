// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos;

public class ItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public ItemDto()
    {

    }

    public ItemDto(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
