// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Ddd.Application.Contracts.Dtos;

public class AuditEntityDto<TKey, TUserId>: EntityDto<TKey>
{
    public TUserId Creator { get; set; } = default!;

    public DateTimeOffset CreationTime { get; set; }

    public TUserId Modifier { get; set; } = default!;

    public DateTimeOffset ModificationTime { get; set; }
}
