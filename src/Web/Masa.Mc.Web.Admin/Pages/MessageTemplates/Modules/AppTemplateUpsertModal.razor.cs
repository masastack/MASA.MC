﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class AppTemplateUpsertModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private AppTemplateUpsertModel _model = new();
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private Guid _entityId;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    public async Task OpenModalAsync(MessageTemplateDto? model = null)
    {
        _entityId = model?.Id ?? default;
        _model = model?.Adapt<AppTemplateUpsertModel>() ?? new();

        if (_entityId != default)
        {
            await GetFormDataAsync();
        }
        else
        {
            await InitDataAsync();
        }

        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });

        _form?.ResetValidation();
    }

    private async Task InitDataAsync()
    {
        _model.ChannelType = ChannelTypes.App;

        if (string.IsNullOrEmpty(_model.Code))
        {
            _model.Code = $"App_{UtilConvert.GetGuidToNumber()}";
        }

        await HandleSelectChannelTypeAsync(_model.ChannelType);
    }

    private async Task GetFormDataAsync()
    {
        var dto = await MessageTemplateService.GetAsync(_entityId) ?? new();
        _model = dto.Adapt<AppTemplateUpsertModel>();
        _model.ChannelType = dto.Channel.Type;
        await HandleSelectChannelTypeAsync(_model.ChannelType);
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

        var inputDto = _model.Adapt<MessageTemplateUpsertDto>();

        if (_entityId == default)
        {
            await MessageTemplateService.CreateAsync(inputDto);
        }
        else
        {
            await MessageTemplateService.UpdateAsync(_entityId, inputDto);
        }

        Loading = false;
        await UpsertSuccessfulMessage(_entityId, T("MessageTemplate"));
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