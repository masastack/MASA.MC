// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendTemplateMessageByInternalInputDto
{
    public string ChannelCode { get; set; } = string.Empty;

    public ChannelTypes? ChannelType { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public ReceiverTypes ReceiverType { get; set; }

    public string Sign { get; set; } = string.Empty;

    public List<InternalReceiverDto> Receivers { get; set; } = new();

    public SendRuleDto SendRules { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public Guid OperatorId { get; set; } = default;

    public string SystemId { get; set; } = string.Empty;

    public static implicit operator MessageTaskUpsertDto(SendTemplateMessageByInternalInputDto dto)
    {
        return new MessageTaskUpsertDto
        {
            ChannelType = dto.ChannelType,
            EntityType = MessageEntityTypes.Template,
            IsDraft = false,
            IsEnabled = true,
            ReceiverType = dto.ReceiverType,
            SelectReceiverType = MessageTaskSelectReceiverTypes.ManualSelection,
            Sign = dto.Sign,
            Receivers = dto.Receivers.Select(x => (MessageTaskReceiverDto)x).ToList(),
            SendRules = dto.SendRules,
            Variables = dto.Variables,
            Source = MessageTaskSources.Sdk,
            OperatorId = dto.OperatorId,
            SystemId = dto.SystemId
        };
    }
}
