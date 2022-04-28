namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly ICsvImporter _csvImporter;

    public MessageTaskQueryHandler(IMessageTaskRepository repository
        , ICsvImporter csvImporter)
    {
        _repository = repository;
        _csvImporter = csvImporter;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        query.Result = entity.Adapt<MessageTaskDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageTaskQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageTaskDto>>();
        var result = new PaginatedListDto<MessageTaskDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTask, bool>>> CreateFilteredPredicate(GetMessageTaskInput input)
    {
        Expression<Func<MessageTask, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter), x => x.DisplayName.Contains(input.Filter));
        condition = condition.And(input.EntityType.HasValue, x => x.EntityType == input.EntityType);
        condition = condition.And(input.ChannelId.HasValue, x => x.ChannelId == input.ChannelId);
        condition = condition.And(input.IsEnabled.HasValue, x => x.IsEnabled == input.IsEnabled);
        if (input.TimeType == MessageTaskTimeType.ModificationTime)
        {
            condition = condition.And(input.StartTime.HasValue, x => x.ModificationTime >= input.StartTime);
            condition = condition.And(input.EndTime.HasValue, x => x.ModificationTime <= input.EndTime);
        }
        if (input.TimeType == MessageTaskTimeType.SendTime)
        {
            condition = condition.And(input.StartTime.HasValue, x => x.SendTime >= input.StartTime);
            condition = condition.And(input.EndTime.HasValue, x => x.SendTime <= input.EndTime);
        }
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTask>> CreateFilteredQueryAsync(GetMessageTaskInput input)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(input);
        return query.Where(condition);
    }

    [EventHandler]
    public async Task GenerateImportTemplateAsync(GenerateImportTemplateQuery query)
    {
        var result = await _csvImporter.GenerateTemplateBytes<ReceiverImportDto>();
        query.Result = result;
    }
}
