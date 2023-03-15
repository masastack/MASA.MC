// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class SingleAppMessage: AppMessage
{
    public string ClientId { get; }

    public SingleAppMessage(string clientId,string title, string text, string transmissionContent) : base(title, text, transmissionContent)
    {
        ClientId = clientId;
    }
}
