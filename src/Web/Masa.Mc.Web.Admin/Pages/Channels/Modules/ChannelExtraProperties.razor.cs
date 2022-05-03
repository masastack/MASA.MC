﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ChannelType Type { get; set; }

    [Parameter]
    public ExtraPropertyDictionary Value
    {
        get
        {
            return GetValue<ExtraPropertyDictionary>();
        }
        set
        {
            SetValue(value);
        }
    }

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    private ChannelEmailExtraProperties _emailExtraPropertiesRef = default!;
    private ChannelSmsExtraProperties _smsExtraPropertiesRef = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Watcher
            .Watch<ExtraPropertyDictionary>(nameof(Value), async val =>
             {
                 await ValueChanged.InvokeAsync(Value);
             });
    }

    public void HandleChangeAsync(string value, string key)
    {
        Value[key] = value;
    }

    public async Task UpdateExtraPropertiesAsync()
    {
        if (Type == ChannelType.Email) await _emailExtraPropertiesRef.HandleChangeAsync();
        if (Type == ChannelType.Sms) await _smsExtraPropertiesRef.HandleChangeAsync();
        await ValueChanged.InvokeAsync(Value);
    }

}
