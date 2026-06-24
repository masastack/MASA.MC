// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class MessageRecordQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IDataFilter _dataFilter;
    private readonly ITemplateRenderer _templateRenderer;

    public MessageRecordQueryHandler(IMcQueryContext context
        , IAuthClient authClient
        , II18n<DefaultResource> i18n
        , IDataFilter dataFilter
        , ITemplateRenderer templateRenderer)
    {
        _context = context;
        _authClient = authClient;
        _i18n = i18n;
        _dataFilter = dataFilter;
        _templateRenderer = templateRenderer;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageRecordQuery query)
    {
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var entity = await _context.MessageRecordQueries.Include(x => x.Channel).FirstOrDefaultAsync(x => x.Id == query.MessageRecordId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageRecord"));

        var dto = entity.Adapt<MessageRecordDto>();
        await FillUserInfo(new List<MessageRecordDto> { dto });
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageRecordQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        var resultList = await _context.MessageRecordQueries.Include(x => x.Channel).GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageRecordQueryModel.CreationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageRecordDto>>();
        await FillUserInfo(dtos);
        var result = new PaginatedListDto<MessageRecordDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task GetSmsRecordsByMobileAsync(GetSmsRecordsByMobileQuery query)
    {
        var mobile = query.Mobile?.Trim();
        if (string.IsNullOrWhiteSpace(mobile))
        {
            query.Result = new PaginatedListDto<SmsRecordDto>(0, 0, new());
            return;
        }

        using var dataFilter = _dataFilter.Disable<ISoftDelete>();
        Guid? channelId = null;
        if (!string.IsNullOrWhiteSpace(query.ChannelCode))
        {
            var channelCode = query.ChannelCode.Trim();
            channelId = await _context.ChannelQueryQueries
                .AsNoTracking()
                .Where(x => x.Type == ChannelTypes.Sms && x.Code == channelCode)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();
            if (!channelId.HasValue)
            {
                query.Result = new PaginatedListDto<SmsRecordDto>(0, 0, new());
                return;
            }
        }

        var condition = CreateSmsOutboundRecordPredicate(mobile, channelId);
        var resultList = await _context.MessageRecordQueries
            .AsNoTracking()
            .GetPaginatedListAsync(condition, new()
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Sorting = new Dictionary<string, bool>
                {
                    [nameof(MessageRecordQueryModel.SendTime)] = true
                }
            });
        if (!resultList.Result.Any())
        {
            query.Result = new PaginatedListDto<SmsRecordDto>(0, 0, new());
            return;
        }

        var records = resultList.Result
            .Select(x => new
            {
                ChannelId = x.ChannelId!.Value,
                x.SendTime,
                x.MessageEntityId,
                x.Variables,
                x.DisplayName
            })
            .ToList();

        var templateIds = records
            .Select(x => x.MessageEntityId)
            .Distinct()
            .ToList();
        var templateContents = templateIds.Any()
            ? await _context.MessageTemplateQueries
                .AsNoTracking()
                .Where(x => templateIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Content)
            : new Dictionary<Guid, string>();

        var items = records
            .Select(x => new SmsRecordDto
            {
                ChannelId = x.ChannelId,
                SmsContent = ResolveSmsContent(
                    x.MessageEntityId,
                    x.Variables,
                    x.DisplayName,
                    templateContents),
                SendTime = x.SendTime
            })
            .ToList();
        query.Result = new PaginatedListDto<SmsRecordDto>(resultList.Total, resultList.TotalPages, items);
    }

    private static Expression<Func<MessageRecordQueryModel, bool>> CreateSmsOutboundRecordPredicate(
        string mobile,
        Guid? channelId)
    {
        Expression<Func<MessageRecordQueryModel, bool>> condition = x =>
            x.ChannelUserIdentity == mobile &&
            x.MessageEntityType == MessageEntityTypes.Template &&
            x.ChannelId.HasValue;
        condition = condition.And(channelId.HasValue, x => x.ChannelId == channelId!.Value);
        return condition;
    }

    private async Task<Expression<Func<MessageRecordQueryModel, bool>>> CreateFilteredPredicate(GetMessageRecordInputDto inputDto)
    {
        Expression<Func<MessageRecordQueryModel, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.ToLower().Contains(inputDto.Filter.ToLower()));
        condition = condition.And(inputDto.MessageTaskHistoryId.HasValue, m => m.MessageTaskHistoryId == inputDto.MessageTaskHistoryId);
        condition = condition.And(inputDto.ChannelId.HasValue, m => m.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.Success.HasValue, m => m.Success == inputDto.Success);
        condition = condition.And(inputDto.UserId.HasValue, m => m.UserId == inputDto.UserId);
        condition = condition.And(inputDto.MessageTemplateId.HasValue, m => m.MessageEntityId == inputDto.MessageTemplateId && m.MessageEntityType == MessageEntityTypes.Template);
        if (inputDto.TimeType == MessageRecordTimeTypes.ExpectSendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, m => m.ExpectSendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, m => m.ExpectSendTime <= inputDto.EndTime);
        }
        if (inputDto.TimeType == MessageRecordTimeTypes.SendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, m => m.SendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, m => m.SendTime <= inputDto.EndTime);
        }
        condition = condition.And(!string.IsNullOrEmpty(inputDto.SystemId), x => x.SystemId == inputDto.SystemId);
        condition = condition.And(!string.IsNullOrEmpty(inputDto.ChannelUserIdentity), x => x.ChannelUserIdentity == inputDto.ChannelUserIdentity);
        return await Task.FromResult(condition); ;
    }

    private async Task FillUserInfo(List<MessageRecordDto> dtos)
    {
        var userIds = dtos.Where(x => x.UserId != default && string.IsNullOrEmpty(x.User.Account)).Select(x => x.UserId).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetListByIdsAsync(userIds);

        foreach (var item in dtos)  
        {
            if (item.UserId != default)
            {
                var user = userInfos.FirstOrDefault(x => x.Id == item.UserId);
                if (user != null)
                {
                    item.User = user.Adapt<MessageRecordUserDto>();
                }
            }

            item.User.FillChannelUserIdentity(item.ChannelUserIdentity, item.Channel.Type);
        }
    }

    private string ResolveSmsContent(
        Guid messageEntityId,
        ExtraPropertyDictionary? variables,
        string displayName,
        IReadOnlyDictionary<Guid, string> templateContents)
    {
        if (templateContents.TryGetValue(messageEntityId, out var templateContent) &&
            !string.IsNullOrWhiteSpace(templateContent))
        {
            return _templateRenderer.Render(templateContent, variables ?? new());
        }

        return displayName;
    }
}
