// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;

public class WebsiteMessageDomainService : DomainService
{
    private readonly IWebsiteMessageRepository _repository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public WebsiteMessageDomainService(IDomainEventBus eventBus, IWebsiteMessageRepository repository, IMessageRecordRepository messageRecordRepository) : base(eventBus)
    {
        _repository = repository;
        _messageRecordRepository = messageRecordRepository;
    }

    public virtual async Task<bool> CreateAsync(MessageData messageData, MessageTaskHistory taskHistory, MessageReceiverUser item)
    {
        if (await _messageRecordRepository.IsExistsAsync(taskHistory.Id, item.UserId)) return false;
        var linkUrl = messageData.GetDataValue<bool>(nameof(MessageTemplate.IsJump)) ? messageData.GetDataValue<string>(nameof(MessageTemplate.JumpUrl)) : string.Empty;
        var websiteMessage = new WebsiteMessage(taskHistory.MessageTask.ChannelId.Value, item.UserId, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), linkUrl, taskHistory.SendTime.Value);

        var messageRecord = new MessageRecord(item.UserId, websiteMessage.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), taskHistory.MessageTask.ExpectSendTime);
        SetExtraProperties(messageRecord, messageData, item);

        var perDayLimit = messageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
        var sendNum = await _messageRecordRepository.GetCountAsync(x => x.SendTime.Value.Date == DateTime.Now.Date && x.UserId == item.UserId);
        if (sendNum > perDayLimit)
        {
            messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
        }
        else
        {
            messageRecord.SetResult(true, string.Empty);
            await _repository.AddAsync(websiteMessage);
        }

        await _messageRecordRepository.AddAsync(messageRecord);

        return messageRecord.Success.Value;
    }

    private void SetExtraProperties(MessageRecord messageRecord, MessageData messageData, MessageReceiverUser item)
    {
        messageRecord.SetDataValue(nameof(item.DisplayName), item.DisplayName);
        messageRecord.SetDataValue(nameof(item.Email), item.Email);
        messageRecord.SetDataValue(nameof(item.PhoneNumber), item.PhoneNumber);
    }
}
