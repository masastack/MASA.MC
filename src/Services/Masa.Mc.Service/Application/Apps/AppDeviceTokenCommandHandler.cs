// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Apps;

public class AppDeviceTokenCommandHandler
{
    private readonly IAppDeviceTokenRepository _repository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IUserContext _userContext;

    public AppDeviceTokenCommandHandler(IAppDeviceTokenRepository repository, II18n<DefaultResource> i18n, IUserContext userContext)
    {
        _repository = repository;
        _i18n = i18n;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task BindAppDeviceTokenAsync(BindAppDeviceTokenCommand command, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == Guid.Empty)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_LOGGED_IN);
        }

        var entity = await _repository.FindAsync(x => x.ChannelId == command.ChannelId && x.UserId == userId);

        if (entity is null)
        {
            entity = new AppDeviceToken(command.ChannelId, userId, command.DeviceToken, command.Platform, DateTimeOffset.UtcNow, new());
            await _repository.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.UpdateToken(command.DeviceToken, command.Platform);
            await _repository.UpdateAsync(entity, cancellationToken);
        }
    }
}
