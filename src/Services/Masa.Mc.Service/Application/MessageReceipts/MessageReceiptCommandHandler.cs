// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class MessageReceiptCommandHandler
{
    private readonly IMessageRecordRepository _repository;

    public MessageReceiptCommandHandler(IMessageRecordRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task ReceiveHuaweiReceiptAsync(ReceiveHuaweiReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var requestIds = command.Input.Statuses.Select(x => x.RequestId);
        var records = await _repository.GetListAsync(x => requestIds.Contains(x.MessageId));

        foreach (var record in records)
        {
            var status = command.Input.Statuses.FirstOrDefault(x => x.RequestId == record.MessageId && x.Token == record.ChannelUserIdentity);
            if (status != null) 
            {
                var success = status.DeliveryStatus.Result == HuaweiDeliveryResult.Success;
                var failureReason = status.DeliveryStatus.Result.GetDescription();
                record.UpdateResult(success, failureReason);
            }
        }

        await _repository.UpdateRangeAsync(records, cancellationToken);
    }

    [EventHandler]
    public async Task ReceiveHonorReceiptAsync(ReceiveHonorReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var requestIds = command.Input.Statuses.Select(x => x.RequestId);
        var records = await _repository.GetListAsync(x => requestIds.Contains(x.MessageId));

        foreach (var record in records)
        {
            var status = command.Input.Statuses.FirstOrDefault(x => x.RequestId == record.MessageId && x.Token == record.ChannelUserIdentity);
            if (status != null)
            {
                var success = status.Status == HonorReceiptStatus.Success;
                var failureReason = status.Status.GetDescription();
                record.UpdateResult(success, failureReason);
            }
        }

        await _repository.UpdateRangeAsync(records, cancellationToken);
    }

    [EventHandler]
    public async Task ReceiveXiaomiReceiptAsync(ReceiveXiaomiReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var msgIds = command.Input.Keys.ToList();
        var records = await _repository.GetListAsync(x => msgIds.Contains(x.MessageId));

        foreach (var record in records)
        {
            if (!command.Input.TryGetValue(record.MessageId, out var status))
                continue;

            var success = status.Type == MiReceiptType.Delivered || status.Type == MiReceiptType.Clicked;
            var failureReason = status.Type.GetDescription();

            record.UpdateResult(success, failureReason);
        }

        await _repository.UpdateRangeAsync(records, cancellationToken);
    }

    [EventHandler]
    public async Task ReceiveOppoReceiptAsync(ReceiveOppoReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var messageIds = command.Input.Select(x => x.MessageId);
        var records = await _repository.GetListAsync(x => messageIds.Contains(x.MessageId));

        foreach (var record in records)
        {
            var status = command.Input.FirstOrDefault(x =>
                x.MessageId == record.MessageId &&
                x.RegistrationIds?.Split(',').Contains(record.ChannelUserIdentity) == true);

            if (status == null) continue;

            switch (status.EventType)
            {
                case "push_arrive":
                    record.UpdateResult(true, OppoReceiptEventType.PushArrive.GetDescription());
                    break;
                case "regid_invalid":
                    record.UpdateResult(false, OppoReceiptEventType.RegidInvalid.GetDisplayName());
                    break;
                case "user_daily_limit":
                    record.UpdateResult(false, OppoReceiptEventType.UserDailyLimit.GetDescription());
                    break;
                default:
                    record.UpdateResult(false, string.Empty);
                    break;
            }
        }

        await _repository.UpdateRangeAsync(records);
    }

    [EventHandler]
    public async Task ReceiveVivoReceiptAsync(ReceiveVivoReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var taskIds = command.Input.Keys.ToList();
        var records = await _repository.GetListAsync(x => taskIds.Contains(x.MessageId));

        foreach (var record in records)
        {
            if (!command.Input.TryGetValue(record.MessageId, out var status))
                continue;

   
            var success = string.IsNullOrEmpty(status.AckType) || status.AckType == "0";
            var failureReason = GetVivoAckTypeDescription(status.AckType, status.SubAckType);

            record.UpdateResult(success, failureReason);
        }

        await _repository.UpdateRangeAsync(records, cancellationToken);
    }

    private string GetVivoAckTypeDescription(string? ackType, string? subAckType)
    {
        return ackType switch
        {
            "0" or null => "到达",
            "100" => subAckType switch
            {
                "11001" => "推送量级超限",
                "12000" => "单应用单用户频次限制",
                "10012" => "消息过期",
                "14002" => "在线直推时设备离线丢弃",
                _ => "管控其他"
            },
            "101" => "消息审核不通过",
            "102" => subAckType switch
            {
                "20001" => "用户未订阅",
                "20002" => "用户不存在",
                "20006" => "设备不活跃",
                _ => "不匹配其他"
            },
            "103" => "离线消息被覆盖",
            "104" => subAckType switch
            {
                "1017" => "夜间不展示",
                "2124" => "内容和标题完全相同的运营消息",
                "2144" => "消息已过了有效期",
                "2158" => "deeplink跳转参数错误",
                "2161" => "被拉起activity非导出",
                "2162" => "被拉起activity未找到",
                "2183" => "regid失效",
                "2333" => "非官方渠道安装默认关闭运营消息权限",
                _ => "未展示其他"
            },
            _ => "未知"
        };
    }

}
