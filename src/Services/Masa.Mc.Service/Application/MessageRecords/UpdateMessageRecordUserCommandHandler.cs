// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class UpdateMessageRecordUserCommandHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IAuthClient _authClient;
    private readonly ILogger<UpdateMessageRecordUserCommandHandler> _logger;

    public UpdateMessageRecordUserCommandHandler(IMessageRecordRepository repository
        , IAuthClient authClient
        , ILogger<UpdateMessageRecordUserCommandHandler> logger)
    {
        _repository = repository;
        _authClient = authClient;
        _logger = logger;
    }

    [EventHandler]
    public async Task HandleEventAsync(UpdateMessageRecordUserEvent eto)
    {
        var messageRecords = await _repository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecords == null)
        {
            Console.WriteLine($"UpdateMessageRecordUserEventHandler:messageRecords is null");
            _logger.LogInformation($"UpdateMessageRecordUserEventHandler:messageRecords is null");
            return;
        }

        var user = await GetMessageRecordsUser(messageRecords);
        if (user == null)
        {
            Console.WriteLine($"UpdateMessageRecordUserEventHandler:user is null");
            _logger.LogInformation($"UpdateMessageRecordUserEventHandler:user is null");
            return;
        }

        var userInfo = ResolveUserInfo(user);
        messageRecords.SetUserInfo(user.Id, userInfo.DisplayName, userInfo.Account, userInfo.Email, userInfo.PhoneNumber);
        Console.WriteLine($"UpdateMessageRecordUserEventHandler:messageRecords:{JsonSerializer.Serialize(messageRecords)}");
        _logger.LogInformation($"UpdateMessageRecordUserEventHandler:messageRecords:{JsonSerializer.Serialize(messageRecords)}");
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
