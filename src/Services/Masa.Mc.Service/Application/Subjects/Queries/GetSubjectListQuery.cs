// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Subjects.Queries;

public record GetSubjectListQuery(GetSubjectInputDto Input) : Query<List<SubjectDto>>
{
    public override List<SubjectDto> Result { get; set; } = default!;

}
