// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public class SmsMessage
{
    public string PhoneNumber { get; }

    public string Text { get; }

    public IDictionary<string, object> Properties { get; }

    public SmsMessage(string phoneNumber, string text)
    {
        PhoneNumber = phoneNumber;
        Text = text;
        Properties = new Dictionary<string, object>();
    }
}
