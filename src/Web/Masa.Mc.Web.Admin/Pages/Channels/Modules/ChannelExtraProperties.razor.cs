// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ChannelTypes Type { get; set; }

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

    [Parameter]
    public bool PasswordView { get; set; }

    [Parameter]
    public bool IsAdd { get; set; }

    private ChannelEmailExtraProperties? _emailExtraPropertiesRef;
    private ChannelSmsExtraProperties? _smsExtraPropertiesRef;
    private ChannelAppExtraProperties? _appExtraPropertiesRef;
    private ChannelWeixinWorkExtraProperties? _weixinWorkExtraPropertiesRef;

    public void HandleChangeAsync(string value, string key)
    {
        Value[key] = value;
    }

    public async Task UpdateExtraPropertiesAsync()
    {
        if (Type == ChannelTypes.Email && _emailExtraPropertiesRef != null)
        {
            await _emailExtraPropertiesRef.HandleChangeAsync();
        }
        if (Type == ChannelTypes.Sms && _smsExtraPropertiesRef != null)
        {
            await _smsExtraPropertiesRef.HandleChangeAsync();
        }
        if (Type == ChannelTypes.App && _appExtraPropertiesRef != null)
        {
            await _appExtraPropertiesRef.HandleChangeAsync();
        }
        if (Type == ChannelTypes.WeixinWork && _weixinWorkExtraPropertiesRef != null)
        {
            await _weixinWorkExtraPropertiesRef.HandleChangeAsync();
        }
        await ValueChanged.InvokeAsync(Value);
    }

    public bool Validate()
    {
        if (Type == ChannelTypes.Email && _emailExtraPropertiesRef != null)
        {
            return _emailExtraPropertiesRef.Form.Validate();
        }
        if (Type == ChannelTypes.Sms && _smsExtraPropertiesRef != null)
        {
            return _smsExtraPropertiesRef.Form.Validate();
        }
        if (Type == ChannelTypes.App && _appExtraPropertiesRef != null && _appExtraPropertiesRef.Form != null)
        {
            return _appExtraPropertiesRef.Form.Validate();
        }
        if (Type == ChannelTypes.WeixinWork && _weixinWorkExtraPropertiesRef != null && _weixinWorkExtraPropertiesRef.Form != null)
        {
            return _weixinWorkExtraPropertiesRef.Form.Validate();
        }
        return true;
    }
}
