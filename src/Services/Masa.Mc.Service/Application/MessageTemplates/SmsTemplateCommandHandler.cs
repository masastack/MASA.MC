// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AlibabaCloud.SDK.Dysmsapi20170525.Models.QuerySmsTemplateListResponseBody;

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class SmsTemplateCommandHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly IChannelRepository _channelRepository;
    private readonly ISmsTemplateRepository _smsTemplateRepository;

    public SmsTemplateCommandHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsTemplateService smsTemplateService
        , IChannelRepository channelRepository
        , ISmsTemplateRepository smsTemplateRepository)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsTemplateService = smsTemplateService;
        _channelRepository = channelRepository;
        _smsTemplateRepository = smsTemplateRepository;
    }

    [EventHandler]
    public async Task SyncAsync(SyncSmsTemplateCommand command)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == command.ChannelId);
        if (channel == null)
        {
            return;
        }
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var aliyunSmsTemplateList = new List<QuerySmsTemplateListResponseBodySmsTemplateList>();
            await GetAliyunSmsTemplateList(aliyunSmsTemplateList, 1);
            var removeList = await _smsTemplateRepository.GetListAsync(x => x.ChannelId == channel.Id);
            await _smsTemplateRepository.RemoveRangeAsync(removeList);
            var smsTemplateList = aliyunSmsTemplateList.Select(item => new SmsTemplate(channel.Id, item.TemplateCode, item.TemplateName, AliyunSmsTemplateTypeMapToSmsTemplateType(item.TemplateType), AliyunSmsTemplateAuditStatusMapToAuditStatus(item.AuditStatus), item.TemplateContent, item.Reason.RejectInfo));
            await _smsTemplateRepository.AddRangeAsync(smsTemplateList);
        }
    }

    private async Task GetAliyunSmsTemplateList(List<QuerySmsTemplateListResponseBodySmsTemplateList> smsTemplateList, int page = 1)
    {
        var aliyunSmsTemplateListResponse = await _smsTemplateService.GetSmsTemplateListAsync(page, 50) as SmsTemplateListResponse;
        if (!aliyunSmsTemplateListResponse.Success)
        {
            throw new UserFriendlyException(aliyunSmsTemplateListResponse.Message);
        }
        var aliyunSmsTemplateList = aliyunSmsTemplateListResponse.Data.Body.SmsTemplateList;
        if (aliyunSmsTemplateList != null && aliyunSmsTemplateList.Any())
        {
            smsTemplateList.AddRange(aliyunSmsTemplateList);
            page++;
            await GetAliyunSmsTemplateList(smsTemplateList, page);
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
