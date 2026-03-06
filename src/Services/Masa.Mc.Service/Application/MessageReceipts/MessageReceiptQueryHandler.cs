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
        Expression<Func<SmsInboundQueryModel, bool>> condition = x => x.ChannelId == input.ChannelId;

        condition = condition.And(!string.IsNullOrWhiteSpace(input.Mobile), x => x.Mobile == input.Mobile);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.AddSerial), x => x.AddSerial == input.AddSerial);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.SmsContent), x => x.SmsContent.Contains(input.SmsContent));
        condition = condition.And(input.StartTime.HasValue, x => x.SendTime >= input.StartTime!.Value);
        condition = condition.And(input.EndTime.HasValue, x => x.SendTime <= input.EndTime!.Value);

        return condition;
    }
}
