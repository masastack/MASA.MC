namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class SmsTemplateUpdateEventHandler
{
    private readonly ISmsTemplateRepository _smsTemplateRepository;
    private readonly IChannelRepository _channelRepository;
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsTemplateService _smsTemplateService;

    public SmsTemplateUpdateEventHandler(ISmsTemplateRepository smsTemplateRepository, IChannelRepository channelRepository, IAliyunSmsAsyncLocal aliyunSmsAsyncLocal, ISmsTemplateService smsTemplateService)
    {
        _smsTemplateRepository = smsTemplateRepository;
        _channelRepository = channelRepository;
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsTemplateService = smsTemplateService;
    }

    [EventHandler]
    public async Task HandleEvent(SmsChannelChangedDomainEvent @event)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == @event.ChannelId);
        var smsTemplateList = await _smsTemplateRepository.GetListAsync(x => x.ChannelId == @event.ChannelId);
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
            foreach (var item in aliyunSmsTemplateList)
            {
                var smsTemplate = smsTemplateList.FirstOrDefault(x => x.TemplateCode == item.TemplateCode);
                if (smsTemplate == null)
                {
                    smsTemplate = new SmsTemplate(channel.Id, item.TemplateCode, item.TemplateName, AliyunSmsTemplateTypeMapToSmsTemplateType(item.TemplateType), AliyunSmsTemplateAuditStatusMapToAuditStatus(item.AuditStatus), item.TemplateContent, item.Reason.RejectInfo);
                    await _smsTemplateRepository.AddAsync(smsTemplate);
                }
                else
                {
                    smsTemplate.Update(item.TemplateName, AliyunSmsTemplateTypeMapToSmsTemplateType(item.TemplateType), AliyunSmsTemplateAuditStatusMapToAuditStatus(item.AuditStatus), item.TemplateContent, item.Reason.RejectInfo);
                    await _smsTemplateRepository.UpdateAsync(smsTemplate);
                }
            }
            var templateCodeList = aliyunSmsTemplateList.Select(x=>x.TemplateCode);
            var removeList = smsTemplateList.Where(x => !templateCodeList.Contains(x.TemplateCode)).ToList();
            await _smsTemplateRepository.RemoveRangeAsync(removeList);
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
            "AUDITSTATEPASS" => MessageTemplateAuditStatus.Adopt,
            "AUDITSTATENOT_PASS" => MessageTemplateAuditStatus.Fail,
            _ => MessageTemplateAuditStatus.WaitAudit
        };
    }
}
