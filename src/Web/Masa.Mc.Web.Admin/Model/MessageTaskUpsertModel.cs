﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Model;

public class MessageTaskUpsertModel
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid? ChannelId { get; set; }

    public ChannelTypes? ChannelType { get; set; }

    public MessageEntityTypes EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public ReceiverTypes ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; } = MessageTaskSelectReceiverTypes.ManualSelection;

    public string Sign { get; set; } = string.Empty;

    public List<MessageTaskReceiverDto> Receivers { get; set; } = new();

    public SendRuleDto SendRules { get; set; } = new();

    public MessageInfoUpsertDto MessageInfo { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public int Step { get; set; } = 1;

    public string SystemId { get; set; } = string.Empty;

    public AppMessageOptions ExtraProperties { get; set; } = new();

    public bool ComputedJumpUrlShow
    {
        get
        {
            return ChannelType == ChannelTypes.WebsiteMessage || (ChannelType == ChannelTypes.App && ExtraProperties.IsWebsiteMessage) || (ChannelType == ChannelTypes.WeixinWork && MessageInfo.Type == (int)WeixinWorkTemplateTypes.TextCard);
        }
    }

    public bool ComputedTitleShow
    {
        get
        {
            return !(ChannelType == ChannelTypes.Sms || (ChannelType == ChannelTypes.WeixinWork && MessageInfo.Type == (int)WeixinWorkTemplateTypes.Text) || ChannelType == ChannelTypes.WeixinWorkWebhook);
        }
    }

    public bool ComputedJumpUrlRequired
    {
        get
        {
            return ChannelType == ChannelTypes.WeixinWork && MessageInfo.Type == (int)WeixinWorkTemplateTypes.TextCard;
        }
    }

    public bool ComputedMarkdown
    {
        get
        {
            return ChannelType != ChannelTypes.App && ChannelType != ChannelTypes.WeixinWork && ChannelType != ChannelTypes.WeixinWorkWebhook;
        }
    }

    public bool ComputedBroadcast
    {
        get
        {
            return ChannelType == ChannelTypes.WebsiteMessage || ChannelType == ChannelTypes.App || ChannelType == ChannelTypes.WeixinWork || ChannelType == ChannelTypes.WeixinWorkWebhook;
        }
    }
}
