using Masa.Mc.Infrastructure.Common.Helper;

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateQueryHandler
{
    private readonly IMessageTemplateRepository _repository;
    private readonly ISmsSender _smsSender;
    private readonly MessageTemplateDomainService _domainService;

    public MessageTemplateQueryHandler(IMessageTemplateRepository repository, ISmsSender smsSender, MessageTemplateDomainService domainService)
    {
        _repository = repository;
        _smsSender = smsSender;
        _domainService = domainService;
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
        var smsTemplate = await _smsSender.GetSmsTemplateAsync(query.TemplateCode);
        var dto = new GetSmsTemplateDto
        {
            DisplayName = smsTemplate.TemplateName,
            TemplateId = smsTemplate.TemplateCode,
            Content = smsTemplate.TemplateContent,
            AuditStatus = GetAuditStatusBySmsTemplateStatus(smsTemplate.AuditStatus),
            AuditReason = smsTemplate.Reason
        };
        dto.Items = ParseTemplateItem(smsTemplate.TemplateContent);
        query.Result = dto;
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