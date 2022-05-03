// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateQueryHandler
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateQueryHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTemplateQuery query)
    {
        var entity = await (await _repository.GetWithDetailQueryAsync()).FirstOrDefaultAsync(x => x.MessageTemplate.Id == query.MessageTemplateId);
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        var dto = entity.MessageTemplate.Adapt<MessageTemplateDto>();
        dto.Channel = entity.Channel.Adapt<ChannelDto>();
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageTemplateQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredDetailQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "messageTemplate.modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Select(x =>
        {
            var dto = x.MessageTemplate.Adapt<MessageTemplateDto>();
            dto.Channel = x.Channel.Adapt<ChannelDto>();
            return dto;
        }).ToList();
        var result = new PaginatedListDto<MessageTemplateDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTemplateWithDetail, bool>>> CreateFilteredPredicate(GetMessageTemplateInput input)
    {
        Expression<Func<MessageTemplateWithDetail, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter), x => x.MessageTemplate.DisplayName.Contains(input.Filter) || x.MessageTemplate.TemplateId.Contains(input.Filter));
        condition = condition.And(input.ChannelType.HasValue, x => x.Channel.Type == input.ChannelType);
        condition = condition.And(input.Status.HasValue, x => x.MessageTemplate.Status == input.Status);
        condition = condition.And(input.AuditStatus.HasValue, x => x.MessageTemplate.AuditStatus == input.AuditStatus);
        condition = condition.And(input.ChannelId.HasValue, x => x.MessageTemplate.ChannelId == input.ChannelId);
        condition = condition.And(input.StartTime.HasValue, x => x.MessageTemplate.ModificationTime >= input.StartTime);
        condition = condition.And(input.EndTime.HasValue, x => x.MessageTemplate.ModificationTime <= input.EndTime);
        condition = condition.And(input.TemplateType > 0, x => x.MessageTemplate.TemplateType == input.TemplateType);
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTemplateWithDetail>> CreateFilteredDetailQueryAsync(GetMessageTemplateInput input)
    {
        var query = await _repository.GetWithDetailQueryAsync()!;
        var condition = await CreateFilteredPredicate(input);
        return query.Where(condition);
    }
}