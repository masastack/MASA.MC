// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Apps;

public class AppNotificationSubscribeCommandHandler
{
    private readonly IAppVendorConfigRepository _appVendorConfigRepository;
    private readonly ILogger<AppNotificationSubscribeCommandHandler> _logger;
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;

    public AppNotificationSubscribeCommandHandler(IAppVendorConfigRepository appVendorConfigRepository, ILogger<AppNotificationSubscribeCommandHandler> logger, AppNotificationSenderFactory appNotificationSenderFactory)
    {
        _appVendorConfigRepository = appVendorConfigRepository;
        _logger = logger;
        _appNotificationSenderFactory = appNotificationSenderFactory;
    }

    [EventHandler]
    public async Task AppNotificationSubscribeAsync(AppNotificationSubscribeCommand command, CancellationToken cancellationToken = default)
    {
        var vendorConfig = await _appVendorConfigRepository.FindAsync(x => x.ChannelId == command.ChannelId && x.Vendor == (AppVendor)command.Platform);

        if (vendorConfig == null)
        {
            _logger.LogWarning("No vendor config found for platform: {Platform}", command.Platform);
            return;
        }

        var appSenderProvider = (AppPushProviders)command.Platform;
        var options = _appNotificationSenderFactory.GetOptions(appSenderProvider, vendorConfig.Options);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(appSenderProvider);

        using (asyncLocal.Change(options))
        {
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(appSenderProvider);
            await sender.SubscribeAsync(AppNotificationConstants.BroadcastTag, command.DeviceToken, cancellationToken);
        }
    }

    [EventHandler]
    public async Task AppNotificationUnsubscribeAsync(AppNotificationUnsubscribeCommand command, CancellationToken cancellationToken = default)
    {
        var vendorConfig = await _appVendorConfigRepository.FindAsync(x => x.ChannelId == command.ChannelId && x.Vendor == (AppVendor)command.Platform);

        if (vendorConfig == null)
        {
            _logger.LogWarning("No vendor config found for platform: {Platform}", command.Platform);
            return;
        }

        var appSenderProvider = (AppPushProviders)command.Platform;
        var options = _appNotificationSenderFactory.GetOptions(appSenderProvider, vendorConfig.Options);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(appSenderProvider);

        using (asyncLocal.Change(options))
        {
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(appSenderProvider);
            await sender.UnsubscribeAsync(AppNotificationConstants.BroadcastTag, command.DeviceToken, cancellationToken);
        }
    }
}
