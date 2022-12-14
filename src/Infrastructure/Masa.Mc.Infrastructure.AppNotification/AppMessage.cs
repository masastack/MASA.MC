// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class AppMessage
{
    public string ClientId { get; }

    public string Title { get; }

    public string Text { get; }

    public string Url { get; }

    public AppMessage(string clientId, string title, string text, string url)
    {
        ClientId = clientId;
        Title = title;
        Text = text;
        Url = url;
    }
}
