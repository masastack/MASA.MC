// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class UpdateMessageRecordUserEventHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IAuthClient _authClient;

    public UpdateMessageRecordUserEventHandler(IMessageRecordRepository repository, IAuthClient authClient)
    {
        _repository = repository;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task HandleEventAsync(UpdateMessageRecordUserEvent eto)
    {
        var messageRecords = await _repository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecords == null)
        {
            return;
        }

        var user = await GetMessageRecordsUser(messageRecords);
        if (user == null)
        {
            return;
        }
        var userInfo = ResolveUserInfo(user);
        messageRecords.SetUserInfo(user.Id, userInfo.DisplayName, userInfo.Account, userInfo.Email, userInfo.PhoneNumber);

        await _repository.UpdateAsync(messageRecords);
    }

    private MessageReceiverUser ResolveUserInfo(UserModel user)
    {
        return new MessageReceiverUser
        {
            DisplayName = user.DisplayName ?? string.Empty,
            Account = user.Account,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty
        };
    }

    private async Task<UserModel?> GetMessageRecordsUser(MessageRecord messageRecords)
    {
        if (messageRecords.Channel.Type == ChannelTypes.Sms)
        {
            var phoneNumber = messageRecords.GetDataValue<string>(nameof(MessageReceiverUser.PhoneNumber));
            return await _authClient.UserService.FindByPhoneNumberAsync(phoneNumber);
        }
        else if (messageRecords.Channel.Type == ChannelTypes.Email)
        {
            var email = messageRecords.GetDataValue<string>(nameof(MessageReceiverUser.Email));
            return await _authClient.UserService.FindByEmailAsync(email);
        }
        else
        {
            return null;
        }
    }
}
