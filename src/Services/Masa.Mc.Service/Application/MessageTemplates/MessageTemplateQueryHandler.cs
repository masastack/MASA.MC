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
        if (!string.IsNullOrEmpty(input.Filter))
            condition = condition.And(messageTemplate => messageTemplate.DisplayName.Contains(input.Filter));
        return await Task.FromResult(condition); ;
    }
}
