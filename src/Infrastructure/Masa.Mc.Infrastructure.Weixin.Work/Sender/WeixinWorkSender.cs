// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Sender;

public class WeixinWorkSender : IWeixinWorkSender
{
    private readonly IOptionsResolver<IWeixinWorkOptions> _optionsResolver;

    public WeixinWorkSender(IOptionsResolver<IWeixinWorkOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<WeixinWorkMessageResponse> SendTextAsync(WeixinWorkTextMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();

        var accessToken = await AccessTokenContainer.GetTokenAsync(options.CorpId, options.CorpSecret);

        var result = await Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextAsync(accessToken, options.AgentId, message.Content, string.Join("|", message.UserId));

        return ConvertToMessageResponse(result);
    }

    public async Task<WeixinWorkMessageResponse> SendTextCardAsync(WeixinWorkTextCardMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();

        var accessToken = await AccessTokenContainer.GetTokenAsync(options.CorpId, options.CorpSecret);

        var result = await Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendTextCardAsync(accessToken, options.AgentId, message.Title, message.Description, message.Url, null, string.Join("|", message.UserId));

        return ConvertToMessageResponse(result);
    }

    private WeixinWorkMessageResponse ConvertToMessageResponse(MassResult result)
    {
        return new WeixinWorkMessageResponse
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
