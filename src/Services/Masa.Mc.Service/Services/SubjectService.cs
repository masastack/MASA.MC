// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class SubjectService : ServiceBase
{
    public SubjectService(IServiceCollection services) : base("api/subject")
    {

    }

    [RoutePattern("", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<SubjectDto>> GetListAsync(IEventBus eventbus, [FromQuery] string filter = "")
    {
        var inputDto = new GetSubjectInputDto(filter);
        var query = new GetSubjectListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }
}
