// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work;

public class WeixinWorkSender : IWeixinWorkSender
{
    private readonly IWeixinWorkOptionsResolver _optionsResolver;

    public WeixinWorkSender(IWeixinWorkOptionsResolver optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<WeixinWorkMessageResponseBase> SendTextAsync(WeixinWorkTextMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();

        var accessToken = await AccessTokenContainer.GetTokenAsync(options.CorpId, options.CorpSecret);

        var result = await Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextAsync(accessToken, options.AgentId, message.Content, message.ToUser);

        return ConvertToMessageResponse(result);
    }

    public async Task<WeixinWorkMessageResponseBase> SendTextCardAsync(WeixinWorkTextCardMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();

        var accessToken = await AccessTokenContainer.GetTokenAsync(options.CorpId, options.CorpSecret);

        var result = await Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextCardAsync(accessToken, options.AgentId, message.Title, message.Description, message.Url, null, message.ToUser);

        return ConvertToMessageResponse(result);
    }

    private WeixinWorkMessageResponseBase ConvertToMessageResponse(MassResult result)
    {
        return new WeixinWorkMessageResponseBase
        {
            ErrCode = result.ErrorCodeValue,
            ErrMsg = result.errmsg,
            InvalidUser = result.invaliduser,
            InvalidParty = result.invalidparty,
            InvalidTag = result.invalidtag,
            UnlicensedUser = result.unlicenseduser,
            MsgId = result.msgid,
            ResponseCode = result.response_code,
        };
    }
}
