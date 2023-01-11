﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class UpdateMessageRecordUserEventHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IAuthClient _authClient;
    private readonly ILogger<UpdateMessageRecordUserEventHandler> _logger;

    public UpdateMessageRecordUserEventHandler(IMessageRecordRepository repository
        , IAuthClient authClient
        , ILogger<UpdateMessageRecordUserEventHandler> logger)
    {
        _repository = repository;
        _authClient = authClient;
        _logger = logger;
    }

    [EventHandler]
    public async Task HandleEventAsync(UpdateMessageRecordUserEvent eto)
    {
        var messageRecord = await _repository.FindAsync(x => x.Id == eto.MessageRecordId);

        if (messageRecord == null)
        {
            _logger.LogInformation($"UpdateMessageRecordUserEventHandler:messageRecords is null");
            return;
        }

        var user = await GetMessageRecordsUser(messageRecord);

        if (messageRecord.SystemId == MasaStackConsts.MC_SYSTEM_ID && user == null)
        {
            user = await CreateExternalUserAsync(messageRecord);
        }

        if (user == null)
        {
            return;
        }

        messageRecord.SetUserId(user.Id);
        await _repository.UpdateAsync(messageRecord);
    }

    private async Task<UserModel?> GetMessageRecordsUser(MessageRecord messageRecords)
    {
        try
        {
            if (messageRecords.Channel.Type == ChannelType.Sms)
            {
                return await _authClient.UserService.FindByPhoneNumberAsync(messageRecords.ChannelUserIdentity);
            }
            else if (messageRecords.Channel.Type == ChannelType.Email)
            {
                return await _authClient.UserService.FindByEmailAsync(messageRecords.ChannelUserIdentity);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateMessageRecordUserEventHandler");
            return null;
        }
    }

    private async Task<UserModel?> CreateExternalUserAsync(MessageRecord messageRecords)
    {
        var addUserModel = new AddUserModel();
        if (messageRecords.Channel.Type == ChannelType.Sms)
        {
            var phoneNumber = messageRecords.ChannelUserIdentity;
            addUserModel.Account = phoneNumber;
            addUserModel.DisplayName = phoneNumber;
            addUserModel.PhoneNumber = phoneNumber;
        }
        else if (messageRecords.Channel.Type == ChannelType.Email)
        {
            var email = messageRecords.ChannelUserIdentity;
            addUserModel.Account = email;
            addUserModel.DisplayName = email;
            addUserModel.Email = email;
        }
        else
        {
            return null;
        }

        try
        {
            var user = await _authClient.UserService.AddAsync(addUserModel);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateMessageRecordUserEventHandler");
            return null;
        }
    }
}
