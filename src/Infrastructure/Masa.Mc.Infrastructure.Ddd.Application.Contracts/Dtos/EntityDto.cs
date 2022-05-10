// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;

public class EntityDto<TKey>
{
    public TKey Id { get; set; } = default!;
}
