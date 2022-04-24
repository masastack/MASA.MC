namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class SmsTemplateSynchroEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly IServiceProvider _serviceProvider;

    public SmsTemplateSynchroEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal, ISmsTemplateService smsTemplateService, IServiceProvider serviceProvider)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsTemplateService = smsTemplateService;
        _serviceProvider = serviceProvider;
    }

    [EventHandler]
    public async Task HandleEvent(SmsTemplateSynchroDomainEvent @event)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<McDbContext>();
        var channel = await dbContext.Set<Channel>().FindAsync(@event.ChannelId);
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
            var removeList = dbContext.Set<SmsTemplate>().Where(x => x.ChannelId == channel.Id).ToList();
            dbContext.Set<SmsTemplate>().RemoveRange(removeList);
            var smsTemplateList = aliyunSmsTemplateList.Select(item => new SmsTemplate(channel.Id, item.TemplateCode, item.TemplateName, AliyunSmsTemplateTypeMapToSmsTemplateType(item.TemplateType), AliyunSmsTemplateAuditStatusMapToAuditStatus(item.AuditStatus), item.TemplateContent, item.Reason.RejectInfo));
            await dbContext.Set<SmsTemplate>().AddRangeAsync(smsTemplateList);
            await dbContext.SaveChangesAsync();
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
