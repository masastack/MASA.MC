// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class SmsTemplateCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTemplateUpsertDto _model = new();
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private List<SmsTemplateDto> _templateItems = new();
    private ChannelTypes _channelType;

    ChannelService ChannelService => McCaller.ChannelService;

    SmsTemplateService SmsTemplateService => McCaller.SmsTemplateService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        HubConnection.On(SignalRMethodConsts.GET_SMS_TEMPLATE, async () =>
        {
            _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
            await InvokeAsync(StateHasChanged);
        });
    }

    public async Task OpenModalAsync(ChannelTypes? channelType)
    {
        _model.ChannelType = ChannelTypes.Sms;
        if (channelType.HasValue)
        {
            _channelType = channelType.Value;
            await HandleSelectChannelTypeAsync(_channelType);
        }
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
    {
        if (!await _form.ValidateAsync())
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
        if (_channelItems.Count == 1)
        {
            _model.ChannelId = _channelItems[0].Id;
            await HandleChannelChangeAsync();
        }
    }

    private void HandleTemplateSelected(SmsTemplateDto smsTemplate)
    {
        _model.DisplayName = smsTemplate.TemplateName;
        _model.Content = smsTemplate.TemplateContent;
        _model.AuditStatus = smsTemplate.AuditStatus;
        _model.AuditReason = smsTemplate.AuditReason;
        _model.TemplateType = (int)smsTemplate.TemplateType;
        _model.Items = ParseTemplateItem(smsTemplate.TemplateContent);
    }

    private async Task HandleChannelChangeAsync()
    {
        _model.DisplayName = string.Empty;
        _model.Content = string.Empty;
        _model.TemplateId = string.Empty;
        _model.Items = new();
        _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
    }

    private List<MessageTemplateItemDto> ParseTemplateItem(string content)
    {
        string startstr = "\\${";
        string endstr = "}";
        var paramList = UtilHelper.MidStrEx(content, startstr, endstr);
        return paramList.Select(x => new MessageTemplateItemDto { Code = x, MappingCode = x }).ToList();
    }

    private async Task SyncAsync()
    {
        Loading = true;
        await SmsTemplateService.SyncAsync(new SmsTemplateSyncInputDto(_model.ChannelId));
        _templateItems = await SmsTemplateService.GetListByChannelIdAsync(_model.ChannelId);
        Loading = false;
    }

    private void HandleTemplateItemChanged(MessageTemplateItemChangedEventArgs args)
    {
        string startstr = "${";
        string endstr = "}";
        _model.Title = _model.Title.Replace($"{startstr}{args.OldCode}{endstr}", $"{startstr}{args.NewCode}{endstr}");
        _model.Content = _model.Content.Replace($"{startstr}{args.OldCode}{endstr}", $"{startstr}{args.NewCode}{endstr}");
    }
}
