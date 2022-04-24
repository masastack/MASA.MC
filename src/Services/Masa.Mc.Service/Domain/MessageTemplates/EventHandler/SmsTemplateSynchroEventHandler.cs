namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class SmsTemplateSynchroEventHandler
{
    private readonly ISmsTemplateRepository _smsTemplateRepository;
    private readonly IChannelRepository _channelRepository;
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsTemplateService _smsTemplateService;

    public SmsTemplateSynchroEventHandler(ISmsTemplateRepository smsTemplateRepository, IChannelRepository channelRepository, IAliyunSmsAsyncLocal aliyunSmsAsyncLocal, ISmsTemplateService smsTemplateService)
    {
        _smsTemplateRepository = smsTemplateRepository;
        _channelRepository = channelRepository;
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsTemplateService = smsTemplateService;
    }

    [EventHandler]
    public async Task HandleEvent(SmsTemplateSynchroDomainEvent @event)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == @event.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue(nameof(SmsChannelOptions.AccessKeyId)).ToString(),
            AccessKeySecret = channel.GetDataValue(nameof(SmsChannelOptions.AccessKeySecret)).ToString()
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var aliyunSmsTemplateListResponse = await _smsTemplateService.GetSmsTemplateListAsync() as SmsTemplateListResponse;
            if (!aliyunSmsTemplateListResponse.Success)
            {
                throw new UserFriendlyException(aliyunSmsTemplateListResponse.Message);
            }
            var aliyunSmsTemplateList = aliyunSmsTemplateListResponse.Data.Body.SmsTemplateList;
            await _smsTemplateRepository.RemoveAsync(x => x.ChannelId == channel.Id);
            var smsTemplateList = aliyunSmsTemplateList.Select(item => new SmsTemplate(channel.Id, item.TemplateCode, item.TemplateName, AliyunSmsTemplateTypeMapToSmsTemplateType(item.TemplateType), AliyunSmsTemplateAuditStatusMapToAuditStatus(item.AuditStatus), item.TemplateContent, item.Reason.RejectInfo));
            await _smsTemplateRepository.AddRangeAsync(smsTemplateList);
        }
    }

    private SmsTemplateType AliyunSmsTemplateTypeMapToSmsTemplateType(int? templateType)
    {
        return templateType switch
        {
            2 => SmsTemplateType.VerificationCode,
            0 => SmsTemplateType.Notification,
            6 => SmsTemplateType.Promotion,
            7 => SmsTemplateType.Digital,
            _ => SmsTemplateType.Other
        };
    }

    private MessageTemplateAuditStatus AliyunSmsTemplateAuditStatusMapToAuditStatus(string auditStatus)
    {
        return auditStatus switch
        {
            "AUDIT_STATE_PASS" => MessageTemplateAuditStatus.Adopt,
            "AUDIT_STATE_NOT_PASS" => MessageTemplateAuditStatus.Fail,
            _ => MessageTemplateAuditStatus.WaitAudit
        };
    }
}
