// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Channels.Aggregates;

public class ChannelType : Enumeration
{
    public static ChannelType Sms = new SmsChannel();

    public static ChannelType Email = new EmailChannel();

    public static ChannelType WebsiteMessage = new WebsiteMessageChannel();

    public static ChannelType App = new AppsChannel();

    public ChannelType(int id, string name) : base(id, name)
    {
    }

    public virtual SendMessageEvent GetSendMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
    {
        throw new NotImplementedException();
    }

    public virtual RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
    {
        throw new NotImplementedException();
    }

    public virtual string GetChannelUserIdentity(Receiver receiver)
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

        public override RetryMessageEvent GetRetryMessageEvent(Guid messageRecordId)
        {
            return new RetrySmsMessageEvent(messageRecordId);
        }

        public override string GetChannelUserIdentity(Receiver receiver)
        {
            return receiver.PhoneNumber;
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

        public override string GetChannelUserIdentity(Receiver receiver)
        {
            return receiver.Email;
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

        public override string GetChannelUserIdentity(Receiver receiver)
        {
            return receiver.SubjectId.ToString();
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

        public override string GetChannelUserIdentity(Receiver receiver)
        {
            return string.Empty;
        }

        public ExtraPropertyDictionary GetMessageTransmissionContent(MessageContent messageContent)
        {
            if (messageContent.IsJump && !messageContent.ExtraProperties.Any(x => x.Key == "url"))
            {
                messageContent.ExtraProperties.TryAdd("url", messageContent.JumpUrl);
            }

            return messageContent.ExtraProperties;
            //return JsonSerializer.Serialize(messageContent.ExtraProperties);
        }
    }
}