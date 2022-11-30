// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageReceiverUser : ValueObject
{
    public Receiver Receiver { get; protected set; } = default!;

    public ExtraPropertyDictionary Variables { get; set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Receiver;
    }

    private MessageReceiverUser() { }

    public MessageReceiverUser(Receiver receiver, ExtraPropertyDictionary variables)
    {
        Receiver = receiver;
        Variables = variables;
    }
}
