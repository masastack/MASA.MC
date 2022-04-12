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
        var entity = await _repository.FindAsync(x=>x.Id==query.MessageTemplateId);
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        query.Result = entity.Adapt<MessageTemplateDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageTemplateQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTemplate.CreationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageTemplateDto>>();
        var result = new PaginatedListDto<MessageTemplateDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTemplate, bool>>> CreateFilteredPredicate(GetMessageTemplateInput input)
    {
        Expression<Func<MessageTemplate, bool>> condition = messageTemplate => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter),messageTemplate => messageTemplate.DisplayName.Contains(input.Filter));
        condition = condition.And(input.ChannelType.HasValue, messageTemplate => messageTemplate.ChannelType == input.ChannelType);
        condition = condition.And(input.Status.HasValue, messageTemplate => messageTemplate.Status== input.Status);
        condition = condition.And(input.AuditStatus.HasValue, messageTemplate => messageTemplate.AuditStatus == input.AuditStatus);
        condition = condition.And(input.ChannelId.HasValue, messageTemplate => messageTemplate.ChannelId == input.ChannelId);
        condition = condition.And(input.StartTime.HasValue, messageTemplate => messageTemplate.ModificationTime >= input.StartTime);
        condition = condition.And(input.EndTime.HasValue, messageTemplate => messageTemplate.ModificationTime <= input.EndTime);
        return await Task.FromResult(condition); ;
    }
}
