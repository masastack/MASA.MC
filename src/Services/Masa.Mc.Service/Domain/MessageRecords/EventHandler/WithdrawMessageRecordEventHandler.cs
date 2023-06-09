// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class WithdrawMessageRecordEventHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly IChannelRepository _channelRepository;
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;

    public WithdrawMessageRecordEventHandler(IMessageRecordRepository repository
        , IMessageTaskRepository messageTaskRepository
        , IAppNotificationAsyncLocal appNotificationAsyncLocal
        , IChannelRepository channelRepository
        , AppNotificationSenderFactory appNotificationSenderFactory)
    {
        _repository = repository;
        _messageTaskRepository = messageTaskRepository;
        _appNotificationAsyncLocal = appNotificationAsyncLocal;
        _channelRepository = channelRepository;
        _appNotificationSenderFactory = appNotificationSenderFactory;
    }

    [EventHandler]
    public async Task HandleEventAsync(WithdrawMessageRecordEvent eto)
    {
        var task = await _messageTaskRepository.FindAsync(x => x.Id == eto.MessageTaskId);
        var channel = await _channelRepository.FindAsync(x => x.Id == task.ChannelId);

        var messageRecords = await _repository.GetListAsync(x => x.MessageTaskHistoryId == eto.MessageTaskHistoryId);

        if (task?.ChannelType?.Id == ChannelType.App.Id)
        {
            var options = new AppOptions
            {
                AppID = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppID)),
                AppKey = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppKey)),
                AppSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppSecret)),
                MasterSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.MasterSecret))
            };

            using var change = _appNotificationAsyncLocal.Change(options);

            var provider = channel.ExtraProperties.GetProperty<int>(nameof(AppChannelOptions.Provider));
            var appNotificationSender = _appNotificationSenderFactory.GetAppNotificationSender((Providers)provider);

            foreach (var item in messageRecords)
            {
                var msgId = item.GetDataValue<string>(BusinessConsts.APP_PUSH_MSG_ID);

                if (!string.IsNullOrEmpty(msgId))
                {
                    await appNotificationSender.WithdrawnAsync(msgId);
                }

                item.SetWithdraw();
            }
        }
        else
        {
            foreach (var item in messageRecords)
            {
                item.SetWithdraw();
            }
        }

        await _repository.UpdateManyAsync(messageRecords, true);
    }
}
