﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly ICsvExporter _exporter;
    private readonly IAuthClient _authClient;
    private readonly ITemplateRenderer _templateRenderer;

    public MessageTaskQueryHandler(IMcQueryContext context
        , ICsvExporter exporter
        , IAuthClient authClient
        , ITemplateRenderer templateRenderer)
    {
        _context = context;
        _exporter = exporter;
        _authClient = authClient;
        _templateRenderer = templateRenderer;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskQuery query)
    {
        var entity = await _context.MessageTaskQueries.Include(x => x.Channel).FirstOrDefaultAsync(x => x.Id == query.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, "MessageTask");

        query.Result = entity.Adapt<MessageTaskDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTaskListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.MessageTaskQueries.Include(x => x.Channel).GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTaskQueryModel.ModificationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageTaskDto>>();
        await FillMessageTaskListDtos(dtos);
        var result = new PaginatedListDto<MessageTaskDto>(resultList.Total, resultList.TotalPages, dtos);
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
        var receiverGroup = await _context.ReceiverGroupQueries.Include(x=>x.Items).FirstOrDefaultAsync(x => x.Id == receiverGroupId);
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

    private async Task<Expression<Func<MessageTaskQueryModel, bool>>> CreateFilteredPredicate(GetMessageTaskInputDto inputDto)
    {
        Expression<Func<MessageTaskQueryModel, bool>> condition = x => true;
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
        return await Task.FromResult(condition);
    }

    [EventHandler]
    public async Task GenerateImportTemplateAsync(GenerateReceiverImportTemplateQuery query)
    {
        var template = await _context.MessageTemplateQueries.Include(x=>x.Items).FirstOrDefaultAsync(x => x.Id == query.MessageTemplateId);
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
                var messageContent = await GetMessageContentAsync(item.EntityType, item.EntityId);
                item.Content = HtmlHelper.CutString(messageContent, 500);
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

    public async Task<string> GetMessageContentAsync(MessageEntityTypes entityType, Guid entityId)
    {
        if (entityType == MessageEntityTypes.Ordinary)
        {
            var messageInfo = await _context.MessageInfoQueries.FirstOrDefaultAsync(x => x.Id == entityId);
            return messageInfo?.Content ?? string.Empty;
        }

        if (entityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _context.MessageTemplateQueries.FirstOrDefaultAsync(x => x.Id == entityId);
            return messageTemplate?.Content ?? string.Empty;
        }

        return string.Empty;
    }
}
