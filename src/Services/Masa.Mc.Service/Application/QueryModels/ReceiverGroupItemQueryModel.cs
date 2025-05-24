// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class ReceiverGroupItemQueryModel : Entity<Guid>
{
    public Guid GroupId { get; set; }

    public Guid SubjectId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public ReceiverGroupItemTypes Type { get; set; }
}