// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceivers
{
    [Parameter]
    public ReceiverTypes ReceiverType { get; set; } = new();

    [Parameter]
    public EventCallback<ReceiverTypes> ReceiverTypeChanged { get; set; }

    [Parameter]
    public List<MessageTaskReceiverDto> SelectReceivers { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> SelectReceiversChanged { get; set; }

    [Parameter]
    public List<MessageTaskReceiverDto> ImportReceivers { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ImportReceiversChanged { get; set; }

    [Parameter]
    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; } = MessageTaskSelectReceiverTypes.ManualSelection;

    [Parameter]
    public EventCallback<MessageTaskSelectReceiverTypes> SelectReceiverTypeChanged { get; set; }

    [Parameter]
    public ChannelTypes? ChannelType { get; set; }

    [Parameter]
    public EventCallback<ChannelTypes?> ChannelTypeChanged { get; set; }

    [Parameter]
    public Guid? MessageTemplatesId { get; set; }

    [Parameter]
    public EventCallback<Guid?> MessageTemplatesIdChanged { get; set; }
}
