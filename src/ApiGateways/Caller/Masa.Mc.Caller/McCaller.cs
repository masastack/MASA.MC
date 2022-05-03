// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Caller;

public class McCaller : HttpClientCallerBase
{
    #region Field
    ChannelService? _channelService;
    MessageTemplateService? _messageTemplateService;
    ReceiverGroupService? _receiverGroupService;
    SmsTemplateService? _smsTemplateService;
    MessageTaskService? _messageTaskService;
    MessageInfoService? _messageInfoService;
    #endregion

    public ChannelService ChannelService => _channelService ?? (_channelService = new(CallerProvider));

    public MessageTemplateService MessageTemplateService => _messageTemplateService ?? (_messageTemplateService = new(CallerProvider));

    public ReceiverGroupService ReceiverGroupService => _receiverGroupService ?? (_receiverGroupService = new(CallerProvider));

    public SmsTemplateService SmsTemplateService => _smsTemplateService ?? (_smsTemplateService = new(CallerProvider));

    public MessageTaskService MessageTaskService => _messageTaskService ?? (_messageTaskService = new(CallerProvider));

    public MessageInfoService MessageInfoService => _messageInfoService ?? (_messageInfoService = new(CallerProvider));

    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public McCaller(IServiceProvider serviceProvider, McApiOptions options) : base(serviceProvider)
    {
        Name = nameof(McCaller);
        BaseAddress = options.McServiceBaseAddress;
    }

    protected override IHttpClientBuilder UseHttpClient()
    {
        return base.UseHttpClient().AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
    }
}
