// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Editor;

public partial class DefaultEditor : AdminCompontentBase
{
    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string ContentClass { get; set; }

    [Parameter]
    public string ContentStyle { get; set; }

    [Parameter]
    public string Placeholder { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private OssService OssService => McCaller.OssService;

    private MEditor Ref { get; set; }

    private IJSObjectReference VditorHelper;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (IsDisposed || !firstRender)
        {
            return;
        }
        VditorHelper = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Mc.Web.Admin/js/vditor/quill-helper.js");
    }

    private async Task<bool> HandleUploadAsync(List<EditorUploadFileItem> flist)
    {
        var paramter = await OssService.GetSecurityTokenAsync();
        await VditorHelper.InvokeVoidAsync("upload", Ref.Ref, paramter);
        return true;
    }
}
