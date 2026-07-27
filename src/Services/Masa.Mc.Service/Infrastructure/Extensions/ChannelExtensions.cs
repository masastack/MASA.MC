// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Extensions;

public static class ChannelExtensions
{
    public static WeixinMiniProgramOptions GetWeixinMiniProgramOptions(this Channel channel)
    {
        return new WeixinMiniProgramOptions
        {
            AppId = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.AppId)) ?? string.Empty,
            AppSecret = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.AppSecret)) ?? string.Empty,
            Token = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.Token)) ?? string.Empty,
            EncodingAESKey = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.EncodingAESKey)) ?? string.Empty,
            MiniprogramState = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.MiniprogramState)) ?? "formal",
            Lang = channel.ExtraProperties.GetProperty<string>(nameof(WeixinMiniProgramChannelOptions.Lang)) ?? "zh_CN",
        };
    }

    public static WeixinWorkOptions GetWeixinWorkOptions(this Channel? channel)
    {
        return new WeixinWorkOptions
        {
            CorpId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpId)) ?? string.Empty,
            CorpSecret = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpSecret)) ?? string.Empty,
            AgentId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.AgentId)) ?? string.Empty,
        };
    }

    public static WeixinWorkWebhookOptions GetWeixinWorkWebhookOptions(this Channel? channel)
    {
        return new WeixinWorkWebhookOptions
        {
            Key = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkWebhookOptions.Key)) ?? string.Empty
        };
    }

    public static SmtpEmailOptions GetSmtpEmailOptions(this Channel channel)
    {
        return new SmtpEmailOptions
        {
            Host = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Smtp)) ?? string.Empty,
            Port = channel.ExtraProperties.GetProperty<int>(nameof(EmailChannelOptions.Port)),
            UserName = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName)) ?? string.Empty,
            Password = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Password)) ?? string.Empty,
            EnableSsl = channel.ExtraProperties.GetProperty<bool>(nameof(EmailChannelOptions.Ssl)),
            DefaultFromAddress = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName)) ?? string.Empty
        };
    }
}
