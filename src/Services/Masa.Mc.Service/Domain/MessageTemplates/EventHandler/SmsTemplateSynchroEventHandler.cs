// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class SmsTemplateSyncEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly IServiceProvider _serviceProvider;

    public SmsTemplateSyncEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal, ISmsTemplateService smsTemplateService, IServiceProvider serviceProvider)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsTemplateService = smsTemplateService;
        _serviceProvider = serviceProvider;
    }

    [EventHandler]
    public async Task HandleEvent(SmsTemplateSyncDomainEvent @event)
    {
        var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var unitOfWork = unitOfWorkManager.CreateDbContext();
        var _smsTemplateRepository = unitOfWork.ServiceProvider.GetRequiredService<ISmsTemplateRepository>();
        var _channelRepository = unitOfWork.ServiceProvider.GetRequiredService<IChannelRepository>();
        var channel = await _channelRepository.FindAsync(x => x.Id == @event.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
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

    private SmsTemplateTypes AliyunSmsTemplateTypeMapToSmsTemplateType(int? templateType)
    {
        return templateType switch
        {
            2 => SmsTemplateTypes.VerificationCode,
            0 => SmsTemplateTypes.Notification,
            6 => SmsTemplateTypes.Promotion,
            7 => SmsTemplateTypes.Digital,
            _ => SmsTemplateTypes.Other
        };
    }

    private MessageTemplateAuditStatuses AliyunSmsTemplateAuditStatusMapToAuditStatus(string auditStatus)
    {
        return auditStatus switch
        {
            "AUDIT_STATE_PASS" => MessageTemplateAuditStatuses.Adopt,
            "AUDIT_STATE_NOT_PASS" => MessageTemplateAuditStatuses.Fail,
            _ => MessageTemplateAuditStatuses.WaitAudit
        };
    }
}
