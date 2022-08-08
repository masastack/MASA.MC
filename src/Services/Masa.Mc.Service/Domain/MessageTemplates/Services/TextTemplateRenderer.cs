// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public class TextTemplateRenderer : ITemplateRenderer
{
    public Task<string> RenderAsync(string context, ExtraPropertyDictionary model, string startstr = "{{", string endstr = "}}")
    {
        foreach (var item in model)
        {
            context = context.Replace($"{startstr}{item.Key}{endstr}", item.Value?.ToString() ?? string.Empty);
        }
        return Task.FromResult(context);
    }
}
