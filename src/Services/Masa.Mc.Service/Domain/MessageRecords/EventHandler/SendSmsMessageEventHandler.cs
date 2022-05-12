// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class SendSmsMessageEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsSender _smsSender;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITemplateRenderer _templateRenderer;

    public SendSmsMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal, ISmsSender smsSender, IServiceProvider serviceProvider, ITemplateRenderer templateRenderer)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _serviceProvider = serviceProvider;
        _templateRenderer = templateRenderer;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSmsMessageEvent eto)
    {
        var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var unitOfWork = unitOfWorkManager.CreateDbContext();
        var _channelRepository = unitOfWork.ServiceProvider.GetRequiredService<IChannelRepository>();
        var _messageRecordRepository = unitOfWork.ServiceProvider.GetRequiredService<IMessageRecordRepository>();
        var _messageTaskHistoryRepository = unitOfWork.ServiceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables);
                SetUserInfo(messageRecord, item);
                messageRecord.SetDataValue(nameof(MessageTemplate.Title), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));
                var smsMessage = new SmsMessage(item.PhoneNumber, JsonSerializer.Serialize(item.Variables));
                smsMessage.Properties.Add("SignName", eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Sign)));
                smsMessage.Properties.Add("TemplateCode", eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));
                try
                {
                    var response = await _smsSender.SendAsync(smsMessage) as SmsSendResponse;
                    if (response.Success)
                    {
                        messageRecord.SetResult(true, string.Empty);
                    }
                    else
                    {
                        messageRecord.SetResult(false, response.Message);
                    }
                }
                catch (Exception ex)
                {
                    messageRecord.SetResult(false, ex.Message);
                }
                await _messageRecordRepository.AddAsync(messageRecord);
            }
            taskHistory.SetComplete();
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
        }
    }

    private void SetUserInfo(MessageRecord messageRecord, MessageReceiverUser item)
    {
        messageRecord.SetDataValue(nameof(item.DisplayName), item.DisplayName);
        messageRecord.SetDataValue(nameof(item.Email), item.Email);
        messageRecord.SetDataValue(nameof(item.PhoneNumber), item.PhoneNumber);
    }
}
