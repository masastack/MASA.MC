// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.EventHandler;

public class ReceiverGroupUserUpdateEventHandler
{
    public ReceiverGroupUserUpdateEventHandler()
    {

    }

    [EventHandler]
    public void HandleEvent(ReceiverGroupItemChangedDomainEvent @event)
    {
        //Waiting to dock Masa Auth
    }
}
