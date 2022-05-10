// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve;

public interface IEmailOptions
{
    public string Host { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Domain { get; set; }

    public bool EnableSsl { get; set; }

    public bool UseDefaultCredentials { get; set; }

    public string DefaultFromAddress { get; set; }

    public string DefaultFromDisplayName { get; set; }
}