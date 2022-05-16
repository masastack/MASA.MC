// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceivers : AdminCompontentBase
{
    [Parameter]
    public List<MessageTaskReceiverDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public ChannelTypes? Type { get; set; }
    private StringNumber _pageTab;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _pageTab = T("ManualSelection");
    }

    public async Task RemoveValue(MessageTaskReceiverDto item)
    {
        Value.RemoveAll(x => x.SubjectId == item.SubjectId && x.Type == item.Type);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleAddAsync(List<MessageTaskReceiverDto> receivers)
    {
        foreach (var receiver in receivers)
        {
            if (Type == ChannelTypes.Sms && Value.Any(x => x.PhoneNumber == receiver.PhoneNumber)) continue;
            if (Type == ChannelTypes.Email && Value.Any(x => x.Email == receiver.Email)) continue;
            if (Type == ChannelTypes.WebsiteMessage && Value.Any(x => x.SubjectId == receiver.SubjectId)) continue;
            Value.Insert(0, receiver);
        }
        await ValueChanged.InvokeAsync(Value);
    }
}
