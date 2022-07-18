// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly ICsvExporter _exporter;
    private readonly MessageTaskDomainService _domainService;
    private readonly IAuthClient _authClient;
    private readonly IReceiverGroupRepository _receiverGroupRepository;

    public MessageTaskQueryHandler(IMessageTaskRepository repository
        , IMessageTemplateRepository messageTemplateRepository
        , ICsvExporter exporter
        , MessageTaskDomainService domainService
        , IAuthClient authClient
        , IReceiverGroupRepository receiverGroupRepository)
    {
        _repository = repository;
        _messageTemplateRepository = messageTemplateRepository;
        _exporter = exporter;
        _domainService = domainService;
        _authClient = authClient;
        _receiverGroupRepository = receiverGroupRepository;
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
    public async Task GetListAsync(GetMessageTaskListQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageTaskDto>>();
        await FillMessageTaskListDtos(entityDtos);
        var result = new PaginatedListDto<MessageTaskDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task ResolveReceiversCountAsync(ResolveReceiversCountQuery query)
    {
        var totalCount = query.Receivers.LongCount(x => x.Type == MessageTaskReceiverTypes.User);
        foreach (var item in query.Receivers.Where(x => x.Type != MessageTaskReceiverTypes.User))
        {
            switch (item.Type)
            {
                case MessageTaskReceiverTypes.Organization:
                    totalCount += await ResolveAuthUsersCount(ReceiverGroupItemTypes.Organization, item.SubjectId);
                    break;
                case MessageTaskReceiverTypes.Role:
                    totalCount += await ResolveAuthUsersCount(ReceiverGroupItemTypes.Role, item.SubjectId);
                    break;
                case MessageTaskReceiverTypes.Team:
                    totalCount += await ResolveAuthUsersCount(ReceiverGroupItemTypes.Team, item.SubjectId);
                    break;
                case MessageTaskReceiverTypes.Group:
                    totalCount += await ResolveReceiverGroupCount(item.SubjectId);
                    break;
                default:
                    break;
            }
        }
        query.Result = totalCount;
    }

    private async Task<long> ResolveReceiverGroupCount(Guid receiverGroupId)
    {
        long receiverGroupCount = 0;
        var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
        if (receiverGroup == null)
        {
            return receiverGroupCount;
        }
        receiverGroupCount = receiverGroup.Items.Count(x => x.Type == ReceiverGroupItemTypes.User);
        foreach (var item in receiverGroup.Items.Where(x => x.Type != ReceiverGroupItemTypes.User))
        {
            receiverGroupCount += await ResolveAuthUsersCount(item.Type, item.SubjectId);
        }
        return receiverGroupCount;
    }

    private async Task<long> ResolveAuthUsersCount(ReceiverGroupItemTypes type, Guid subjectId)
    {
        long count = 0;
        switch (type)
        {
            case ReceiverGroupItemTypes.Organization:
                count = await _authClient.UserService.GetTotalByDepartmentAsync(subjectId);
                break;
            case ReceiverGroupItemTypes.Role:
                count = await _authClient.UserService.GetTotalByRoleAsync(subjectId);
                break;
            case ReceiverGroupItemTypes.Team:
                count = await _authClient.UserService.GetTotalByTeamAsync(subjectId);
                break;
            default:
                break;
        }
        return count;
    }

    private async Task<Expression<Func<MessageTask, bool>>> CreateFilteredPredicate(GetMessageTaskInputDto inputDto)
    {
        Expression<Func<MessageTask, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.Contains(inputDto.Filter));
        condition = condition.And(inputDto.EntityType.HasValue, x => x.EntityType == inputDto.EntityType);
        condition = condition.And(inputDto.ChannelId.HasValue, x => x.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.IsEnabled.HasValue, x => x.IsEnabled == inputDto.IsEnabled);
        condition = condition.And(inputDto.IsDraft.HasValue, x => x.IsDraft == inputDto.IsDraft);
        condition = condition.And(inputDto.Status.HasValue, x => x.Status == inputDto.Status);
        condition = condition.And(inputDto.Source.HasValue, x => x.Source == inputDto.Source);
        if (inputDto.TimeType == MessageTaskTimeTypes.ModificationTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, x => x.ModificationTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, x => x.ModificationTime <= inputDto.EndTime);
        }
        if (inputDto.TimeType == MessageTaskTimeTypes.SendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, x => x.SendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, x => x.SendTime <= inputDto.EndTime);
        }
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTask>> CreateFilteredQueryAsync(GetMessageTaskInputDto inputDto)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }

    [EventHandler]
    public async Task GenerateImportTemplateAsync(GenerateReceiverImportTemplateQuery query)
    {
        var template = await _messageTemplateRepository.FindAsync(x => x.Id == query.MessageTemplateId);
        var record = new ExpandoObject();
        var properties = GetReceiverImportDtoType(query.ChannelType).GetProperties();
        foreach (var prop in properties)
        {
            var name = prop.Name;
            var importAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
            if (importAttribute != null)
            {
                name = importAttribute.Name ?? prop.GetDisplayName() ?? prop.Name;
            }
            record.TryAdd(name, string.Empty);
        }
        if (template != null)
        {
            foreach (var item in template.Items)
            {
                record.TryAdd(item.Code, string.Empty);
            }
        }
        var result = await _exporter.ExportDynamicHeaderAsByteArray(record);
        query.Result = result;
    }

    private async Task FillMessageTaskListDtos(List<MessageTaskDto> dtos)
    {
        var modifierUserIds = dtos.Where(x => x.Modifier != default).Select(x => x.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetUserPortraitsAsync(modifierUserIds);
        foreach (var item in dtos)
        {
            if (item.EntityId != default)
            {
                var messageData = await _domainService.GetMessageDataAsync(item.EntityType, item.EntityId, item.Variables);
                item.Content = HtmlHelper.CutString(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), 280);
            }

            item.ModifierName = userInfos.FirstOrDefault(x => x.Id == item.Modifier)?.DisplayName ?? string.Empty;
        }
    }

    private Type GetReceiverImportDtoType(ChannelTypes channelType)
    {
        switch (channelType)
        {
            case ChannelTypes.Sms:
                return typeof(SmsReceiverImportDto);
            case ChannelTypes.Email:
                return typeof(EmailReceiverImportDto);
            case ChannelTypes.WebsiteMessage:
                return typeof(WebsiteMessageReceiverImportDto);
            default:
                throw new UserFriendlyException("Unknown channel type");
        }
    }
}
