// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class SendEmailMessageEventHandler
{
    private readonly IEmailAsyncLocal _emailAsyncLocal;
    private readonly IEmailSender _emailSender;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITemplateRenderer _templateRenderer;

    public SendEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal, IEmailSender emailSender, IServiceProvider serviceProvider, ITemplateRenderer templateRenderer)
    {
        _emailAsyncLocal = emailAsyncLocal;
        _emailSender = emailSender;
        _serviceProvider = serviceProvider;
        _templateRenderer = templateRenderer;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendEmailMessageEvent eto)
    {
        var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var unitOfWork = unitOfWorkManager.CreateDbContext();
        var _channelRepository = unitOfWork.ServiceProvider.GetRequiredService<IChannelRepository>();
        var _messageRecordRepository = unitOfWork.ServiceProvider.GetRequiredService<IMessageRecordRepository>();
        var _messageTaskHistoryRepository = unitOfWork.ServiceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new SmtpEmailOptions
        {
            Host = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Smtp)),
            Port = channel.ExtraProperties.GetProperty<int>(nameof(EmailChannelOptions.Port)),
            UserName = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName)),
            Password = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Password)),
            EnableSsl = channel.ExtraProperties.GetProperty<bool>(nameof(EmailChannelOptions.Ssl)),
            DefaultFromAddress = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName))
        };
        using (_emailAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables);
                SetUserInfo(messageRecord, item);
                TemplateRenderer(eto.MessageData, taskHistory.Variables);
                messageRecord.SetDataValue(nameof(MessageTemplate.Title), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)));
                try
                {
                    await _emailSender.SendAsync(
                        item.Email,
                        eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)),
                        eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Content))
                    );
                    messageRecord.SetResult(true, string.Empty);
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

    private async void TemplateRenderer(MessageData messageData, ExtraPropertyDictionary Variables)
    {
        messageData.SetDataValue(nameof(MessageTemplate.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), Variables));
        messageData.SetDataValue(nameof(MessageTemplate.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), Variables));
    }

    private void SetUserInfo(MessageRecord messageRecord, MessageReceiverUser item)
    {
        messageRecord.SetDataValue(nameof(item.DisplayName), item.DisplayName);
        messageRecord.SetDataValue(nameof(item.Email), item.Email);
        messageRecord.SetDataValue(nameof(item.PhoneNumber), item.PhoneNumber);
    }
}
