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
    TokenProvider _tokenProvider;
    #endregion

    public ChannelService ChannelService => _channelService ?? (_channelService = new(Caller));

    public MessageTemplateService MessageTemplateService => _messageTemplateService ?? (_messageTemplateService = new(Caller));

    public ReceiverGroupService ReceiverGroupService => _receiverGroupService ?? (_receiverGroupService = new(Caller));

    public SmsTemplateService SmsTemplateService => _smsTemplateService ?? (_smsTemplateService = new(Caller));

    public MessageTaskService MessageTaskService => _messageTaskService ?? (_messageTaskService = new(Caller));

    public MessageInfoService MessageInfoService => _messageInfoService ?? (_messageInfoService = new(Caller));

    public MessageRecordService MessageRecordService => _messageRecordService ?? (_messageRecordService = new(Caller));

    public MessageTaskHistoryService MessageTaskHistoryService => _messageTaskHistoryService ?? (_messageTaskHistoryService = new(Caller));

    public WebsiteMessageService WebsiteMessageService => _websiteMessageService ?? (_websiteMessageService = new(Caller));

    public OssService OssService => _ossService ?? (_ossService = new OssService(Caller));

    public SubjectService SubjectService => _subjectService ?? (_subjectService = new(Caller));

    public UserService UserService => _userService ?? (_userService = new(Caller));

    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public McCaller(
        IServiceProvider serviceProvider,
        TokenProvider tokenProvider,
        McApiOptions options) : base(serviceProvider)
    {
        Name = nameof(McCaller);
        BaseAddress = options.McServiceBaseAddress;
        _tokenProvider = tokenProvider;
    }

    protected override async Task ConfigHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        await base.ConfigHttpRequestMessageAsync(requestMessage);
    }
}
