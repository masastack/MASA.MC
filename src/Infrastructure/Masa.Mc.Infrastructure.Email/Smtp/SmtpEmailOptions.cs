// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Smtp;

public class SmtpEmailOptions : IEmailOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public bool EnableSsl { get; set; }

    public bool UseDefaultCredentials { get; set; }

    public string DefaultFromAddress { get; set; } = string.Empty;

    public string DefaultFromDisplayName { get; set; } = string.Empty;
}
