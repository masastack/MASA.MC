// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Channels.Aggregates;

public class ChannelType : Enumeration
{
    public static ChannelType Sms = new SmsChannel();

    public static ChannelType Email = new EmailChannel();

    public static ChannelType WebsiteMessage = new WebsiteMessageChannel();

    public static ChannelType App = new AppsChannel();

    public static ChannelType WeixinWork = new WeixinWorkChannel();

    public static ChannelType WeixinWorkWebhook = new WeixinWorkWebhookChannel();

    public ChannelType(int id, string name) : base(id, name)
    {
    }

    public virtual SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
    {
        throw new NotImplementedException();
    }

    public virtual SendSimpleMessageEvent GetSendSimpleMessageEvent(string channelUserIdentity, string channelCode, MessageData messageData, ExtraPropertyDictionary variables, ExtraPropertyDictionary originalVariables, string systemId)
    {
        throw new NotImplementedException();
    }

    public virtual RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
    {
        throw new NotImplementedException();
    }

    public virtual string GetChannelUserIdentity(UserModel user)
    {
        throw new NotImplementedException();
    }

    private class SmsChannel : ChannelType
    {
        public SmsChannel() : base(1, nameof(Sms)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendSmsMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override SendSimpleMessageEvent GetSendSimpleMessageEvent(string channelUserIdentity, string channelCode, MessageData messageData, ExtraPropertyDictionary variables, ExtraPropertyDictionary originalVariables, string systemId)
        {
            return new SendSimpleSmsMessageEvent(channelUserIdentity, channelCode, messageData)
            {
                Variables = variables,
                OriginalVariables = originalVariables,
                SystemId = systemId
            };
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetrySmsMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return user?.PhoneNumber ?? string.Empty;
        }
    }

    private class EmailChannel : ChannelType
    {
        public EmailChannel() : base(2, nameof(Email)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendEmailMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetryEmailMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return user?.Email ?? string.Empty;
        }
    }

    private class WebsiteMessageChannel : ChannelType
    {
        public WebsiteMessageChannel() : base(3, nameof(WebsiteMessage)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendWebsiteMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetryWebsiteMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return user.Id.ToString();
        }
    }

    public class AppsChannel : ChannelType
    {
        public AppsChannel() : base(4, nameof(App)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendAppMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetryAppMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return string.Empty;
        }

        public ExtraPropertyDictionary GetMessageTransmissionContent(MessageContent messageContent)
        {
            var extraProperties = new ExtraPropertyDictionary(messageContent.ExtraProperties);
            if (messageContent.IsJump && !extraProperties.ContainsKey("url"))
            {
                extraProperties.TryAdd("url", messageContent.JumpUrl);
            }
            return extraProperties;
        }
    }

    private class WeixinWorkChannel : ChannelType
    {
        public WeixinWorkChannel() : base(5, nameof(WeixinWork)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendWeixinWorkMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetryWeixinWorkMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return string.Empty;
        }
    }

    private class WeixinWorkWebhookChannel : ChannelType
    {
        public WeixinWorkWebhookChannel() : base(6, nameof(WeixinWorkWebhook)) { }

        public override SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        {
            return new SendWeixinWorkWebhookMessageEvent(channelId, messageData, messageTaskHistory);
        }

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetryWeixinWorkWebhookMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(UserModel user)
        {
            return string.Empty;
        }
    }
}