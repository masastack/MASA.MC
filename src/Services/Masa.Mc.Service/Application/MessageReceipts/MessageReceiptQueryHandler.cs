// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class MessageReceiptQueryHandler
{
    private readonly IMcQueryContext _context;

    public MessageReceiptQueryHandler(IMcQueryContext context)
    {
        _context = context;
    }

    [EventHandler]
    public async Task GetListSmsInboundAsync(GetListSmsInboundQuery query)
    {
        var input = query.Input;
        if (!input.ChannelId.HasValue && !string.IsNullOrWhiteSpace(input.ChannelCode))
        {
            var channelCode = input.ChannelCode.Trim();
            input.ChannelId = (await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Code == channelCode))?.Id;
        }

        if (!input.ChannelId.HasValue)
        {
            query.Result = new PaginatedListDto<SmsInboundDto>(0, 0, new List<SmsInboundDto>());
            return;
        }

        var condition = CreateSmsInboundPredicate(input);
        var resultList = await _context.SmsInboundQueries.GetPaginatedListAsync(condition, new()
        {
            Page = input.Page,
            PageSize = input.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(SmsInboundQueryModel.SendTime)] = true
            }
        });

        query.Result = new PaginatedListDto<SmsInboundDto>(resultList.Total, resultList.TotalPages, resultList.Result.Adapt<List<SmsInboundDto>>());
    }

    private static Expression<Func<SmsInboundQueryModel, bool>> CreateSmsInboundPredicate(GetSmsInboundInputDto input)
    {
        Expression<Func<SmsInboundQueryModel, bool>> condition = x => x.ChannelId == input.ChannelId!.Value;

        condition = condition.And(!string.IsNullOrWhiteSpace(input.Mobile), x => x.Mobile == input.Mobile);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.AddSerial), x => x.AddSerial == input.AddSerial);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.SmsContent), x => x.SmsContent.Contains(input.SmsContent));
        condition = condition.And(input.Provider.HasValue, x => x.Provider == input.Provider!.Value);
        condition = condition.And(input.StartTime.HasValue, x => x.SendTime >= input.StartTime!.Value);
        condition = condition.And(input.EndTime.HasValue, x => x.SendTime <= input.EndTime!.Value);

        return condition;
    }
}
