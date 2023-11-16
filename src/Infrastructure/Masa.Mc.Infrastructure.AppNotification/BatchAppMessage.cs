// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class BatchAppMessage : AppMessage
{
    public List<string> ClientIds { get; }

    public BatchAppMessage(List<string> clientIds, string title, string text, string url, ConcurrentDictionary<string, object> transmissionContent, bool isApnsProduction = true) : base(title, text, url, transmissionContent, isApnsProduction)
    {
        ClientIds = clientIds;
    }
}
