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
        foreach (var item in command.Input.Statuses)
        {
            var messageId = item.RequestId;
            var record = await _repository.FindAsync(x => x.MessageId == messageId && x.ChannelUserIdentity == item.Token, cancellationToken);
            if (record == null) continue;

            var success = item.DeliveryStatus.Result == HuaweiDeliveryResult.Success;
            var failureReason = item.DeliveryStatus.Result.GetDescription();
            record.UpdateResult(success, failureReason);
            await _repository.UpdateAsync(record, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveHonorReceiptAsync(ReceiveHonorReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input.Statuses)
        {
            var messageId = item.RequestId;
            var record = await _repository.FindAsync(x => x.MessageId == messageId && x.ChannelUserIdentity == item.Token, cancellationToken);
            if (record == null) continue;

            var success = item.Status == HonorReceiptStatus.Success;
            var failureReason = item.Status.GetDescription();
            record.UpdateResult(success, failureReason);
            await _repository.UpdateAsync(record, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveXiaomiReceiptAsync(ReceiveXiaomiReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input)
        {
            var messageId = item.Key;
            var targets = item.Value.Targets?.Split(',') ?? Array.Empty<string>();
            var records = await _repository.GetListAsync(x => x.MessageId == messageId && targets.Contains(x.ChannelUserIdentity), cancellationToken);
            if (!records.Any()) continue;

            var success = item.Value.Type == MiReceiptType.Delivered || item.Value.Type == MiReceiptType.Clicked;
            var failureReason = item.Value.Type.GetDescription();

            foreach (var record in records)
            {
                record.UpdateResult(success, failureReason);
            }

            await _repository.UpdateRangeAsync(records, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveOppoReceiptAsync(ReceiveOppoReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input)
        {
            var messageId = item.MessageId;
            var targets = item.RegistrationIds?.Split(',') ?? Array.Empty<string>();
            var records = await _repository.GetListAsync(x => x.MessageId == messageId && targets.Contains(x.ChannelUserIdentity), cancellationToken);
            if (!records.Any()) continue;

            var eventType = item.GetEventTypeEnum();
            var success = eventType == OppoReceiptEventType.PushArrive;
            var failureReason = eventType?.GetDescription() ?? string.Empty;

            foreach (var record in records)
            {
                record.UpdateResult(success, failureReason);
            }

            await _repository.UpdateRangeAsync(records, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveVivoReceiptAsync(ReceiveVivoReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input)
        {
            var messageId = item.Key;
            var targets = item.Value.Targets?.Split(',') ?? Array.Empty<string>();
            var records = await _repository.GetListAsync(x=>x.MessageId == messageId && targets.Contains(x.ChannelUserIdentity), cancellationToken);
            if (!records.Any()) continue;

            var success = string.IsNullOrEmpty(item.Value.AckType) || item.Value.AckType == "0";
            var failureReason = GetVivoAckTypeDescription(item.Value.AckType, item.Value.SubAckType);

            foreach (var record in records)
            {
                record.UpdateResult(success, failureReason);
            }

            await _repository.UpdateRangeAsync(records, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveAliyunReceiptAsync(ReceiveAliyunReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input.Statuses)
        {
            // 阿里云回执通过BizId（对应发送时的BizId）和手机号来匹配记录
            var messageId = item.BizId;
            var phoneNumber = item.PhoneNumber;
            var record = await _repository.FindAsync(x => x.MessageId == messageId && x.ChannelUserIdentity == phoneNumber, cancellationToken);
            if (record == null) continue;

            // 使用Success字段（Boolean类型，官方格式）
            var success = item.Success;
            var failureReason = success
                ? string.Empty
                : (string.IsNullOrEmpty(item.ErrMsg) ? $"失败({item.ErrCode})" : item.ErrMsg);
            record.UpdateResult(success, failureReason);
            await _repository.UpdateAsync(record, cancellationToken);
        }
    }

    [EventHandler]
    public async Task ReceiveYunMasReceiptAsync(ReceiveYunMasReceiptCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Input.Statuses)
        {
            // 移动云mas回执通过MsgGroup（批次号）和手机号来匹配记录
            var msgGroup = item.MsgGroup;
            var phoneNumber = item.Mobile;
            var record = await _repository.FindAsync(x => x.MessageId == msgGroup && x.ChannelUserIdentity == phoneNumber, cancellationToken);
            if (record == null) continue;

            // errorCode == "DELIVRD" 表示成功，否则失败
            var success = item.ErrorCode == "DELIVRD";
            var failureReason = success 
                ? string.Empty 
                : (!string.IsNullOrEmpty(item.ErrorCode) ? item.ErrorCode : "发送失败");
            
            record.UpdateResult(success, failureReason);
            await _repository.UpdateAsync(record, cancellationToken);
        }

        // 设置返回结果
        command.Result = new YunMasReceiptResultDto(0, "接收成功");
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
