﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Caller;

public class McApiOptions
{
    public string McServiceBaseAddress { get; set; }

    public McApiOptions(string mcServiceBaseAddress)
    {
        McServiceBaseAddress = mcServiceBaseAddress;
    }
}
