// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public class BatchSmsMessage
{
    public List<string> PhoneNumbers { get; }

    public string Text { get; }

    public IDictionary<string, object> Properties { get; }

    public BatchSmsMessage(List<string> phoneNumbers, string text)
    {
        PhoneNumbers = phoneNumbers;
        Text = text;
        Properties = new Dictionary<string, object>();
    }
}
