// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

internal sealed record MessageTemplateLookupItem(
    Guid Id,
    string Code,
    string DisplayName,
    string Content);
