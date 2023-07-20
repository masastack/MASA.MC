// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTaskHistorys;

public class MessageTaskHistoryQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IAuthClient _authClient;

    public MessageTaskHistoryQueryHandler(IMcQueryContext context, II18n<DefaultResource> i18n, IAuthClient authClient)
    {
        _context = context;
        _i18n = i18n;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskHistoryQuery query)
    {
        var entity = await _context.MessageTaskHistoryQueries.FirstOrDefaultAsync(x => x.Id == query.MessageTaskHistoryId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTaskHistory"));

        query.Result = entity.Adapt<MessageTaskHistoryDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTaskHistoryListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.MessageTaskHistoryQueries.GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTaskHistoryQueryModel.SendTime)] = true
            }
        });

        var dtos = resultList.Result.Adapt<List<MessageTaskHistoryDto>>();
        var result = new PaginatedListDto<MessageTaskHistoryDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task GetReceiverUsersAsync(GetMessageTaskHistoryReceiverUsersQuery query)
    {
        var messageTaskHistory = await _context.MessageTaskHistoryQueries.Include(x => x.MessageTask).FirstOrDefaultAsync(x=>x.Id == query.MessageTaskHistoryId);
        MasaArgumentException.ThrowIfNull(messageTaskHistory, _i18n.T("MessageTaskHistory"));
        MasaArgumentException.ThrowIfNull(messageTaskHistory.MessageTask.ChannelType, _i18n.T("ChannelType"));

        var list = await _context.MessageReceiverUserQueries.Where(x => x.MessageTaskHistoryId == query.MessageTaskHistoryId).ToListAsync();

        var userIds = list.Select(x => x.UserId);
        var users = await _authClient.UserService.GetListByIdsAsync(userIds.ToArray());

        var dtos = new List<MessageTaskReceiverDto>();

        foreach (var item in list)
        {
            if (item.UserId == default)
            {
                var receiverDto = new MessageTaskReceiverDto() { Type = MessageTaskReceiverTypes.User };
                receiverDto.SetChannelUserIdentity(messageTaskHistory.MessageTask.ChannelType.Value, item.ChannelUserIdentity);
                dtos.Add(receiverDto);
                continue;
            }

            var user = users.FirstOrDefault(x=>x.Id == item.UserId);
            if (user == null)
                continue;

            var dto = new MessageTaskReceiverDto
            {
                Type = MessageTaskReceiverTypes.User,
                SubjectId = item.UserId,
                DisplayName = user.DisplayName,
                Avatar = user.Avatar,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user?.Email ?? string.Empty,
            };
            dtos.Add(dto);
        }

        query.Result = dtos;
    }

    private async Task<Expression<Func<MessageTaskHistoryQueryModel, bool>>> CreateFilteredPredicate(GetMessageTaskHistoryInputDto inputDto)
    {
        Expression<Func<MessageTaskHistoryQueryModel, bool>> condition = x => !x.IsTest;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.TaskHistoryNo.Contains(inputDto.Filter));
        condition = condition.And(inputDto.MessageTaskId.HasValue, x => x.MessageTaskId == inputDto.MessageTaskId);
        condition = condition.And(inputDto.Status.HasValue, x => x.Status == inputDto.Status);
        condition = condition.And(inputDto.StartTime.HasValue, x => x.SendTime >= inputDto.StartTime);
        condition = condition.And(inputDto.EndTime.HasValue, x => x.SendTime <= inputDto.EndTime);
        return await Task.FromResult(condition);
    }
}