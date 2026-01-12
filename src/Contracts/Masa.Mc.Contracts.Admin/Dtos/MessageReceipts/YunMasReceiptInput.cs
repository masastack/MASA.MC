// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class YunMasReceiptInput
{
    public List<YunMasReceiptStatusDto> Statuses { get; set; } = new();
}

public class YunMasReceiptStatusDto
{
    /// <summary>
    /// 报告状态
    /// </summary>
    [JsonPropertyName("reportStatus")]
    public string ReportStatus { get; set; } = string.Empty;

    /// <summary>
    /// 回执手机号
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 提交时间，格式：yyyyMMddHHmmss
    /// </summary>
    [JsonPropertyName("submitDate")]
    public string SubmitDate { get; set; } = string.Empty;

    /// <summary>
    /// 接收时间，格式：yyyyMMddHHmmss
    /// </summary>
    [JsonPropertyName("receiveDate")]
    public string ReceiveDate { get; set; } = string.Empty;

    /// <summary>
    /// 状态码：DELIVRD表示成功
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 消息批次号，唯一编码，与前文响应中的msgGroup对应
    /// </summary>
    [JsonPropertyName("msgGroup")]
    public string MsgGroup { get; set; } = string.Empty;
}