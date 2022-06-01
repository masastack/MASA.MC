// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class SubjectQueryHandler
{
    readonly IAuthClient _authClient;

    public SubjectQueryHandler(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetListAsync(GetSubjectListQuery query)
    {
        var list = await _authClient.SubjectService.GetListAsync(query.Input.Filter);
        var dtos = list.Adapt<List<SubjectDto>>();
        query.Result = dtos;
    }
}
