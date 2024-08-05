// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Sender;

public class WeixinWorkWebhookSender : IWeixinWorkWebhookSender
{
    private readonly IOptionsResolver<IWeixinWorkWebhookOptions> _optionsResolver;

    public WeixinWorkWebhookSender(IOptionsResolver<IWeixinWorkWebhookOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<WeixinWorkMessageResponseBase> SendTextAsync(WeixinWorkTextMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();
        var result = await WebhookApi.SendTextAsync(options.Key, message.Content, message.UserId.ToArray());

        return ConvertToMessageResponse(result);
    }

    private WeixinWorkMessageResponse ConvertToMessageResponse(WorkJsonResult result)
    {
        return new WeixinWorkMessageResponse
        {
            ErrCode = result.ErrorCodeValue,
            ErrMsg = result.errmsg
        };
    }
}
