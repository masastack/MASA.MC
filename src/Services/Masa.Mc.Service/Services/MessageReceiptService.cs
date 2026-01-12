// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageReceiptService : ServiceBase
{
    IEventBus _eventBus => GetRequiredService<IEventBus>();

    public MessageReceiptService() : base("api/message-receipt")
    {

    }

    [RoutePattern("huawei", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<HuaweiReceiptResultDto> ReceiveHuaweiReceiptAsync([FromBody] HuaweiReceiptInput input)
    {
        var command = new ReceiveHuaweiReceiptCommand(input);
        await _eventBus.PublishAsync(command);
        return command.Result;
    }

    [RoutePattern("honor", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<HonorReceiptResultDto> ReceiveHonorReceiptAsync([FromBody] HonorReceiptInput input)
    {
        var command = new ReceiveHonorReceiptCommand(input);
        await _eventBus.PublishAsync(command);
        return command.Result;
    }

    [RoutePattern("xiaomi", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task ReceiveXiaomiReceiptAsync(HttpRequest request)
    {
        var form = await request.ReadFormAsync();
        var data = Uri.UnescapeDataString(form["data"]);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var receiptData = JsonSerializer.Deserialize<Dictionary<string, MiReceiptStatusDto>>(data, options);

        var command = new ReceiveXiaomiReceiptCommand(receiptData);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("oppo", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task ReceiveOppoReceiptAsync([FromBody] List<OppoReceiptInput> input)
    {
        var command = new ReceiveOppoReceiptCommand(input);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("vivo", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task ReceiveVivoReceiptAsync([FromBody] Dictionary<string, VivoReceiptStatusDto> input)
    {
        var command = new ReceiveVivoReceiptCommand(input);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("yunmas", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<YunMasReceiptResultDto> ReceiveYunMasReceiptAsync([FromBody] YunMasReceiptStatusDto status)
    {
        // 云MAS推送的是单个状态报告对象，包装成YunMasReceiptInput
        var input = new YunMasReceiptInput { Statuses = new List<YunMasReceiptStatusDto> { status } };
        var command = new ReceiveYunMasReceiptCommand(input);
        await _eventBus.PublishAsync(command);
        return command.Result;
    }
}
