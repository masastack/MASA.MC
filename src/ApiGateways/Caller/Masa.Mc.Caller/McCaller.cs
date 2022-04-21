namespace Masa.Mc.Caller;

public class McCaller : HttpClientCallerBase
{
    #region Field
    ChannelService? _channelService;
    MessageTemplateService? _messageTemplateService;
    ReceiverGroupService? _receiverGroupService;
    SmsTemplateService? _smsTemplateService;
    #endregion

    public ChannelService ChannelService => _channelService ?? (_channelService = new(CallerProvider));

    public MessageTemplateService MessageTemplateService => _messageTemplateService ?? (_messageTemplateService = new(CallerProvider));

    public ReceiverGroupService ReceiverGroupService => _receiverGroupService ?? (_receiverGroupService = new(CallerProvider));

    public SmsTemplateService SmsTemplateService => _smsTemplateService ?? (_smsTemplateService = new(CallerProvider));

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
