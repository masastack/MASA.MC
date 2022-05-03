// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Options.Channels;

public class EmailChannelOptions
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Smtp { get; set; } = string.Empty;
    public bool Ssl { get; set; }
    public int Port { get; set; }
}
