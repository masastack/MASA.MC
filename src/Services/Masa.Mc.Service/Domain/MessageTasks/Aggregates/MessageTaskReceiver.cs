// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskReceiver : ValueObject
{
    public Receiver Receiver { get; set; }

    public MessageTaskReceiverTypes Type { get; set; }

    public ExtraPropertyDictionary Variables { get; set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Receiver;
        yield return Type;
    }

    public MessageTaskReceiver(Receiver receiver, MessageTaskReceiverTypes type, ExtraPropertyDictionary variables)
    {
        Receiver = receiver;
        Type = type;
        Variables = variables;
    }
}