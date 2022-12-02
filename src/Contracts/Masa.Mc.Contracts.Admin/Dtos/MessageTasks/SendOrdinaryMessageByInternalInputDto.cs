// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendOrdinaryMessageByInternalInputDto
{
    public string ChannelCode { get; set; } = string.Empty;

    public ChannelTypes? ChannelType { get; set; }

    public ReceiverTypes ReceiverType { get; set; }

    public List<InternalReceiverDto> Receivers { get; set; } = new();

    public SendRuleDto SendRules { get; set; } = new();

    public MessageInfoUpsertDto MessageInfo { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public Guid OperatorId { get; set; } = default;

    public static implicit operator MessageTaskUpsertDto(SendOrdinaryMessageByInternalInputDto dto)
    {
        return new MessageTaskUpsertDto
        {
            ChannelType = dto.ChannelType,
            EntityType = MessageEntityTypes.Ordinary,
            IsDraft = false,
            IsEnabled = true,
            ReceiverType = dto.ReceiverType,
            SelectReceiverType = MessageTaskSelectReceiverTypes.ManualSelection,
            Receivers = dto.Receivers.Select(x => (MessageTaskReceiverDto)x).ToList(),
            SendRules = dto.SendRules,
            MessageInfo = dto.MessageInfo,
            Variables = dto.Variables,
            Source = MessageTaskSources.Sdk,
            OperatorId = dto.OperatorId,
        };
    }
}
