// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class WeixinMiniProgramTemplateQueryHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IProviderAsyncLocal<IWeixinMiniProgramOptions> _asyncLocal;
    private readonly IWeixinMiniProgramTemplateProvider _templateProvider;

    public WeixinMiniProgramTemplateQueryHandler(
        IChannelRepository channelRepository,
        IProviderAsyncLocal<IWeixinMiniProgramOptions> asyncLocal,
        IWeixinMiniProgramTemplateProvider templateProvider)
    {
        _channelRepository = channelRepository;
        _asyncLocal = asyncLocal;
        _templateProvider = templateProvider;
    }

    [EventHandler]
    public async Task GetListAsync(GetWeixinMiniProgramTemplateListQuery query)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == query.ChannelId);
        if (channel == null || channel.Type.Id != ChannelType.WeixinMiniProgram.Id)
        {
            return;
        }

        var options = channel.GetWeixinMiniProgramOptions();

        using (_asyncLocal.Change(options))
        {
            var templates = await _templateProvider.GetTemplateListAsync();
            query.Result = templates.Select(x => new WeixinMiniProgramTemplateDto
            {
                ChannelId = channel.Id,
                TemplateId = x.TemplateId,
                Title = x.Title,
                Content = x.Content,
                Example = x.Example,
                TemplateType = x.Type,
                Items = x.Items.Select(item => new MessageTemplateItemDto
                {
                    Code = item.Code,
                    MappingCode = item.Code,
                    DisplayText = item.Code
                }).ToList()
            }).ToList();
        }
    }
}
