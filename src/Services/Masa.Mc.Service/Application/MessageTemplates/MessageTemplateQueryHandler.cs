namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateQueryHandler
{
    private readonly IMessageTemplateRepository _repository;
    private readonly IChannelRepository _channelRepository;
    private readonly ISmsSender _smsSender;
    private readonly MessageTemplateDomainService _domainService;

    public MessageTemplateQueryHandler(IMessageTemplateRepository repository, IChannelRepository channelRepository, ISmsSender smsSender, MessageTemplateDomainService domainService)
    {
        _repository = repository;
        _smsSender = smsSender;
        _domainService = domainService;
        _channelRepository = channelRepository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTemplateQuery query)
    {

        var entity = await (await _repository.GetWithDetailQueryAsync()).FirstOrDefaultAsync(x => x.MessageTemplate.Id == query.MessageTemplateId);
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        var dto= entity.MessageTemplate.Adapt<MessageTemplateDto>();
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
        if(string.IsNullOrEmpty(options.Sorting)) options.Sorting = "messageTemplate.creationTime desc";
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
        var channel = await _channelRepository.FindAsync(x=>x.Id== query.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue(nameof(SmsChannelOptions.AccessKeyId)).ToString(),
            AccessKeySecret = channel.GetDataValue(nameof(SmsChannelOptions.AccessKeySecret)).ToString()
        };
        _smsSender.SetOptions(options);
        var smsTemplateResponse = await _smsSender.GetSmsTemplateAsync(query.TemplateCode) as SmsTemplateResponse;
        var smsTemplate = smsTemplateResponse.Data.Body;
        var dto = new GetSmsTemplateDto
        {
            DisplayName = smsTemplate.TemplateName,
            TemplateId = smsTemplate.TemplateCode,
            Content = smsTemplate.TemplateContent,
            AuditStatus = GetAuditStatusBySmsTemplateStatus(smsTemplate.TemplateStatus),
            AuditReason = smsTemplate.Reason
        };
        dto.Items = ParseTemplateItem(smsTemplate.TemplateContent);
        query.Result = dto;
    }

    private async Task<Expression<Func<MessageTemplateWithDetail, bool>>> CreateFilteredPredicate(GetMessageTemplateInput input)
    {
        Expression<Func<MessageTemplateWithDetail, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter), x => x.MessageTemplate.DisplayName.Contains(input.Filter));
        condition = condition.And(input.ChannelType.HasValue, x => x.Channel.Type == input.ChannelType);
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

    private MessageTemplateAuditStatus GetAuditStatusBySmsTemplateStatus(int? status)
    {
        switch (status)
        {
            case 1:
                return MessageTemplateAuditStatus.Adopt;
            case 2:
                return MessageTemplateAuditStatus.Fail;
            default:
                return MessageTemplateAuditStatus.WaitAudit;
        }
    }

    private List<MessageTemplateItemDto> ParseTemplateItem(string content)
    {
        string startstr = "\\${";
        string endstr = "}";
        var paramList = UtilHelper.MidStrEx(content, startstr, endstr);
        return paramList.Select(x => new MessageTemplateItemDto { Code = x, MappingCode = x }).ToList();
    }
}