// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Markdown;

public partial class DefaultMarkdown: AdminCompontentBase
{
    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public string Html { get; set; } = string.Empty;

    [Parameter]
    public bool Reaonly { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<string> HtmlChanged { get; set; }

    private OssService OssService => McCaller.OssService;

    private MMarkdown Ref { get; set; }

    private IJSObjectReference VditorHelper;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (IsDisposed || !firstRender)
        {
            return;
        }
        VditorHelper = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Mc.Web.Admin/js/vditor/vditor-helper.js");
    }

    private async Task HandleUploadAsync()
    {
        var paramter = await OssService.GetSecurityTokenAsync();
        await VditorHelper.InvokeVoidAsync("upload", Ref.Ref, paramter);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (VditorHelper != null)
            {
                await VditorHelper.DisposeAsync();
            }
        }
        catch (Exception)
        {
        }
    }
}
