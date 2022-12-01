// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public interface IChannelUserFinder
{
    Task<IEnumerable<MessageReceiverUser>> GetReceiverUsersAsync(ChannelTypes type, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers);
}