// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class MessageRecordQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;
    private readonly II18n<DefaultResource> _i18n;

    public MessageRecordQueryHandler(IMcQueryContext context
        , IAuthClient authClient
        , II18n<DefaultResource> i18n)
    {
        _context = context;
        _authClient = authClient;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageRecordQuery query)
    {
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
        var resultList = await _context.MessageRecordQueries.Include(x => x.Channel).GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageRecordQueryModel.ModificationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageRecordDto>>();
        await FillUserInfo(dtos);
        var result = new PaginatedListDto<MessageRecordDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageRecordQueryModel, bool>>> CreateFilteredPredicate(GetMessageRecordInputDto inputDto)
    {
        Expression<Func<MessageRecordQueryModel, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.Contains(inputDto.Filter));
        condition = condition.And(inputDto.MessageTaskHistoryId.HasValue, m => m.MessageTaskHistoryId == inputDto.MessageTaskHistoryId);
        condition = condition.And(inputDto.ChannelId.HasValue, m => m.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.Success.HasValue, m => m.Success == inputDto.Success);
        condition = condition.And(inputDto.UserId.HasValue, m => m.UserId == inputDto.UserId);
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
        return await Task.FromResult(condition); ;
    }

    private async Task FillUserInfo(List<MessageRecordDto> dtos)
    {
        var userIds = dtos.Where(x => x.UserId != default && string.IsNullOrEmpty(x.User.Account)).Select(x => x.UserId).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetUsersAsync(userIds);

        foreach (var item in dtos)
        {
            if (item.UserId != default && string.IsNullOrEmpty(item.User.Account))
            {
                var user = userInfos.FirstOrDefault(x => x.Id == item.UserId);
                if (user != null)
                {
                    item.User = user.Adapt<MessageRecordUserDto>();
                }
            }
        }
    }
}
