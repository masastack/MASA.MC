﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates;

public class MessageContent : ValueObject
{
    public string Title { get; protected set; } = string.Empty;
    public string Content { get; protected set; } = string.Empty;
    public string Markdown { get; protected set; } = string.Empty;
    public bool IsJump { get; protected set; }
    public string JumpUrl { get; protected set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Title;
        yield return Content;
        yield return Markdown;
        yield return IsJump;
        yield return JumpUrl;
    }

    public MessageContent(string title, string content, string markdown, bool isJump, string jumpUrl)
    {
        Title = title;
        Content = content;
        Markdown = markdown;
        JumpUrl = jumpUrl;
    }

    public string GetJumpUrl()
    {
        return IsJump ? JumpUrl : string.Empty;
    }
}
