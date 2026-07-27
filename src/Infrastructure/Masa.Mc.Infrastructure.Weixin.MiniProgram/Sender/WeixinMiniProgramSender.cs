// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Sender;

public class WeixinMiniProgramSender : IWeixinMiniProgramSender
{
    private static readonly Regex CharacterStringRegex = new(@"^[\x21-\x7E]{1,32}$", RegexOptions.Compiled);

    private readonly IOptionsResolver<IWeixinMiniProgramOptions> _optionsResolver;

    public WeixinMiniProgramSender(IOptionsResolver<IWeixinMiniProgramOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<WeixinMiniProgramMessageResponse> SendSubscribeMessageAsync(WeixinMiniProgramSubscribeMessage message)
    {
        var options = await _optionsResolver.ResolveAsync();
        // 稳定版接口(stable_token): 同一 AppId 跨项目/实例返回同一 token 且刷新不使别处失效，
        // 避免普通 access_token 互相顶掉导致的 40001 invalid credential。
        var accessTokenResult = await Senparc.Weixin.MP.CommonAPIs.CommonApi.GetStableAccessTokenAsync(options.AppId, options.AppSecret);
        var accessToken = accessTokenResult.access_token;
        var data = new TemplateMessageData(message.Data.ToDictionary(x => x.Key, x => new TemplateMessageDataValue(x.Value)));
        var result = await MessageApi.SendSubscribeAsync(
            accessToken,
            message.ToUser,
            message.TemplateId,
            data,
            message.Page,
            options.MiniprogramState,
            options.Lang);

        return new WeixinMiniProgramMessageResponse
        {
            errcode = result.errcode,
            errmsg = result.errmsg ?? string.Empty
        };
    }
}
