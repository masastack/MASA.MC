﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class WebsiteMessageTemplateCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private MessageTemplateUpsertDto _model = new();
    private bool _visible;
    private List<ChannelDto> _channelItems = new();

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    public async Task OpenModalAsync()
    {
        _model.ChannelType = ChannelTypes.WebsiteMessage;
        if (string.IsNullOrEmpty(_model.Code))
        {
            _model.Code = $"WM_{UtilConvert.GetGuidToNumber()}";
        }
        await HandleSelectChannelTypeAsync(_model.ChannelType);
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });

        _form?.ResetValidation();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
    {
        Check.NotNull(_form, "form not found");

        _model.DisplayName = _model.Title;
        if (!_form.Validate())
        {
            return;
        }
        Loading = true;
        await MessageTemplateService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTemplateCreateMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task HandleSelectChannelTypeAsync(ChannelTypes Type)
    {
        _channelItems = await ChannelService.GetListByTypeAsync(Type);
    }
}
