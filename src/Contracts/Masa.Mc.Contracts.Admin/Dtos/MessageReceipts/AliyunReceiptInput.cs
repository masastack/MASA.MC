// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class AliyunReceiptInput
{
    public List<AliyunReceiptStatusDto> Statuses { get; set; } = new();
}

public class AliyunReceiptStatusDto
{
    /// <summary>
    /// 短信接收号码
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 是否发送成功
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 发送回执ID，对应发送接口返回的BizId
    /// </summary>
    [JsonPropertyName("biz_id")]
    public string BizId { get; set; } = string.Empty;

    /// <summary>
    /// 短信条数
    /// </summary>
    [JsonPropertyName("sms_size")]
    public string SmsSize { get; set; } = string.Empty;

    /// <summary>
    /// 调用发送短信SendSms接口时传的outId
    /// </summary>
    [JsonPropertyName("out_id")]
    public string OutId { get; set; } = string.Empty;

    /// <summary>
    /// 转发给运营商的时间
    /// </summary>
    [JsonPropertyName("send_time")]
    public string SendTime { get; set; } = string.Empty;

    /// <summary>
    /// 收到运营商回执的时间
    /// </summary>
    [JsonPropertyName("report_time")]
    public string ReportTime { get; set; } = string.Empty;

    /// <summary>
    /// 错误码
    /// </summary>
    [JsonPropertyName("err_code")]
    public string ErrCode { get; set; } = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonPropertyName("err_msg")]
    public string ErrMsg { get; set; } = string.Empty;
}