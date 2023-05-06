// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class AppMessage
{
    public string Title { get; }

    public string Text { get; }

    public string Url { get; }

    public ConcurrentDictionary<string, object> TransmissionContent { get; }

    public AppMessage(string title, string text, string url, ConcurrentDictionary<string, object> transmissionContent)
    {
        Title = title;
        Text = text;
        Url = url;
        TransmissionContent = transmissionContent;
    }
}