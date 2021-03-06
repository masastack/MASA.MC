// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller;

public class McCaller : HttpClientCallerBase
{
    #region Field
    ChannelService? _channelService;
    MessageTemplateService? _messageTemplateService;
    ReceiverGroupService? _receiverGroupService;
    SmsTemplateService? _smsTemplateService;
    MessageTaskService? _messageTaskService;
    MessageInfoService? _messageInfoService;
    MessageRecordService? _messageRecordService;
    MessageTaskHistoryService? _messageTaskHistoryService;
    WebsiteMessageService? _websiteMessageService;
    OssService? _ossService;
    SubjectService? _subjectService;
    UserService? _userService;
    #endregion

    public ChannelService ChannelService => _channelService ?? (_channelService = new(CallerProvider));

    public MessageTemplateService MessageTemplateService => _messageTemplateService ?? (_messageTemplateService = new(CallerProvider));

    public ReceiverGroupService ReceiverGroupService => _receiverGroupService ?? (_receiverGroupService = new(CallerProvider));

    public SmsTemplateService SmsTemplateService => _smsTemplateService ?? (_smsTemplateService = new(CallerProvider));

    public MessageTaskService MessageTaskService => _messageTaskService ?? (_messageTaskService = new(CallerProvider));

    public MessageInfoService MessageInfoService => _messageInfoService ?? (_messageInfoService = new(CallerProvider));

    public MessageRecordService MessageRecordService => _messageRecordService ?? (_messageRecordService = new(CallerProvider));

    public MessageTaskHistoryService MessageTaskHistoryService => _messageTaskHistoryService ?? (_messageTaskHistoryService = new(CallerProvider));

    public WebsiteMessageService WebsiteMessageService => _websiteMessageService ?? (_websiteMessageService = new(CallerProvider));

    public OssService OssService => _ossService ?? (_ossService = new OssService(CallerProvider));

    public SubjectService SubjectService => _subjectService ?? (_subjectService = new(CallerProvider));

    public UserService UserService => _userService ?? (_userService = new(CallerProvider));

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
