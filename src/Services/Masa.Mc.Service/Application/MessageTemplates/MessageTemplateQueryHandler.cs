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
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTemplateId);
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        query.Result = entity.Adapt<MessageTemplateDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageTemplateQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredDetailQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        options.Sorting = "messageTemplate.creationTime desc";
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

    [EventHandler]
    public async Task GetSmsTemplateAsync(GetSmsTemplateQuery query)
    {
        var dto = new GetSmsTemplateDto
        {
            DisplayName = "验证码模板",
            TemplateId = query.TemplateCode,
            Content = "验证码：{{code}}，请尽快完成验证！",
            AuditStatus = MessageTemplateAuditStatus.Adopt,
            AuditReason = "无审批备注",
            Items = new List<MessageTemplateItemDto> {
                new MessageTemplateItemDto{
                    Code="code",
                    MappingCode="code",
                    DisplayText="验证码"
                }
            }
        };
        query.Result = dto;
        await Task.CompletedTask;
    }

    private async Task<Expression<Func<MessageTemplateWithDetail, bool>>> CreateFilteredPredicate(GetMessageTemplateInput input)
    {
        Expression<Func<MessageTemplateWithDetail, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter), x => x.MessageTemplate.DisplayName.Contains(input.Filter));
        condition = condition.And(input.ChannelType.HasValue, x => x.MessageTemplate.ChannelType == input.ChannelType);
        condition = condition.And(input.Status.HasValue, x => x.MessageTemplate.Status == input.Status);
        condition = condition.And(input.AuditStatus.HasValue, x => x.MessageTemplate.AuditStatus == input.AuditStatus);
        condition = condition.And(input.ChannelId.HasValue, x => x.MessageTemplate.ChannelId == input.ChannelId);
        condition = condition.And(input.StartTime.HasValue, x => x.MessageTemplate.ModificationTime >= input.StartTime);
        condition = condition.And(input.EndTime.HasValue, x => x.MessageTemplate.ModificationTime <= input.EndTime);
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTemplateWithDetail>> CreateFilteredDetailQueryAsync(GetMessageTemplateInput input)
    {
        var query = await _repository.GetWithDetailQueryAsync()!;
        var condition = await CreateFilteredPredicate(input);
        return query.Where(condition);
    }
}
