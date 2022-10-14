// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.Subjects;

public class SubjectService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public SubjectService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/subject";
    }

    public async Task<List<SubjectDto>> GetListAsync(GetSubjectInputDto inputDto)
    {
        return await GetAsync<GetSubjectInputDto, List<SubjectDto>>(string.Empty, inputDto) ?? new();
    }
}
