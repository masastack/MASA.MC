// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;
    private readonly II18n<DefaultResource> _i18n;

    public MessageTemplateQueryHandler(IMcQueryContext context
        , IAuthClient authClient
        , II18n<DefaultResource> i18n)
    {
        _context = context;
        _authClient = authClient;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTemplateQuery query)
    {
        var entity = await _context.MessageTemplateQueries.Include(x => x.Channel).Include(x=>x.Items).FirstOrDefaultAsync(x => x.Id == query.MessageTemplateId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTemplate"));

        var dto = entity.Adapt<MessageTemplateDto>();
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTemplateListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.MessageTemplateQueries.Include(x=>x.Channel).Include(x => x.Items).GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTemplateQueryModel.ModificationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageTemplateDto>>();
        await FillMessageTemplateDtos(dtos);
        var result = new PaginatedListDto<MessageTemplateDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTemplateQueryModel, bool>>> CreateFilteredPredicate(GetMessageTemplateInputDto inputDto)
    {
        Expression<Func<MessageTemplateQueryModel, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.Contains(inputDto.Filter) || x.TemplateId.Contains(inputDto.Filter));
        condition = condition.And(inputDto.ChannelType.HasValue, x => x.Channel.Type == inputDto.ChannelType);
        condition = condition.And(inputDto.Status.HasValue, x => x.Status == inputDto.Status);
        condition = condition.And(inputDto.AuditStatus.HasValue, x => x.AuditStatus == inputDto.AuditStatus);
        condition = condition.And(inputDto.ChannelId.HasValue, x => x.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.StartTime.HasValue, x => x.ModificationTime >= inputDto.StartTime);
        condition = condition.And(inputDto.EndTime.HasValue, x => x.ModificationTime <= inputDto.EndTime);
        condition = condition.And(inputDto.TemplateType > 0, x => x.TemplateType == inputDto.TemplateType);
        return await Task.FromResult(condition); ;
    }

    private async Task FillMessageTemplateDtos(List<MessageTemplateDto> dtos)
    {
        var modifierUserIds = dtos.Where(x => x.Modifier != default).Select(x => x.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetListByIdsAsync(modifierUserIds);
        foreach (var item in dtos)
        {
            item.ModifierName = userInfos.FirstOrDefault(x => x.Id == item.Modifier)?.StaffDislpayName ?? string.Empty;
        }
    }
}