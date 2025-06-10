// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Commands;

public record SaveChannelVendorConfigCommand(Guid ChannelId, AppVendor Vendor, ExtraPropertyDictionary Options) : Command;
