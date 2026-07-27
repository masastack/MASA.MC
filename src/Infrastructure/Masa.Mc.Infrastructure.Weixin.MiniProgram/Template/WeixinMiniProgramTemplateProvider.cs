// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Template;

public class WeixinMiniProgramTemplateProvider : IWeixinMiniProgramTemplateProvider
{
    private static readonly Regex TemplateItemRegex = new(@"\{\{(?<code>[^}.]+)(?:\.DATA)?\}\}", RegexOptions.Compiled);

    private readonly IOptionsResolver<IWeixinMiniProgramOptions> _optionsResolver;

    public WeixinMiniProgramTemplateProvider(IOptionsResolver<IWeixinMiniProgramOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<List<WeixinMiniProgramTemplate>> GetTemplateListAsync(CancellationToken cancellationToken = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        // 稳定版接口(stable_token): 同一 AppId 跨项目/实例返回同一 token 且刷新不使别处失效，
        // 避免普通 access_token 互相顶掉导致的 40001 invalid credential。
        var accessTokenResult = await Senparc.Weixin.MP.CommonAPIs.CommonApi.GetStableAccessTokenAsync(options.AppId, options.AppSecret);
        var accessToken = accessTokenResult.access_token;
        cancellationToken.ThrowIfCancellationRequested();

        var response = await Senparc.Weixin.Open.WxaAPIs.NewTmpl.NewTmplApi.GetTemplateListAsync(accessToken);
        if (response.errcode != Senparc.Weixin.ReturnCode.请求成功)
        {
            throw new InvalidOperationException(response.errmsg);
        }

        return (response.data ?? new())
            .Select(x =>
            {
                var content = x.content ?? string.Empty;
                return new WeixinMiniProgramTemplate
                {
                    TemplateId = x.priTmplId ?? string.Empty,
                    Title = x.title ?? string.Empty,
                    Content = content,
                    Example = x.example ?? string.Empty,
                    Type = x.type,
                    Items = ParseTemplateItems(content)
                };
            })
            .ToList();
    }

    private static List<WeixinMiniProgramTemplateItem> ParseTemplateItems(string content)
    {
        return TemplateItemRegex.Matches(content)
            .Select(match => match.Groups["code"].Value)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct()
            .Select(code => new WeixinMiniProgramTemplateItem
            {
                Code = code
            })
            .ToList();
    }
}
