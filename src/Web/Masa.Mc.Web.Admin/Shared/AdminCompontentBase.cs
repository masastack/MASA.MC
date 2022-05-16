﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin;

public abstract class AdminCompontentBase : BDomComponentBase
{
    private I18n? _i18n;
    private GlobalConfig? _globalConfig;
    private NavigationManager? _navigationManager;
    private McCaller? _mcCaller;

    [Inject]
    public McCaller McCaller
    {
        get
        {
            return _mcCaller ?? throw new Exception("please Inject McCaller!");
        }
        set
        {
            _mcCaller = value;
        }
    }

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    [Inject]
    public I18n I18n
    {
        get
        {
            return _i18n ?? throw new Exception("please Inject I18n!");
        }
        set
        {
            _i18n = value;
        }
    }

    [Inject]
    public GlobalConfig GlobalConfig
    {
        get
        {
            return _globalConfig ?? throw new Exception("please Inject GlobalConfig!");
        }
        set
        {
            _globalConfig = value;
        }
    }

    [Inject]
    public NavigationManager NavigationManager
    {
        get
        {
            return _navigationManager ?? throw new Exception("please Inject NavigationManager!");
        }
        set
        {
            _navigationManager = value;
        }
    }

    [Inject]
    public McApiOptions McApiOptions { get; set; } = default!;

    public bool Loading
    {
        get => GlobalConfig.Loading;
        set => GlobalConfig.Loading = value;
    }

    public string T(string key) => I18n.T(key);

    public async Task ConfirmAsync(string messgae, Func<Task> callback, AlertTypes type = AlertTypes.Warning)
    {
        if (await PopupService.ConfirmAsync(I18n.T("OperationConfirmation"), messgae, type)) await callback.Invoke();
    }

    public async Task SuccessMessageAsync(string message)
    {
        await PopupService.AlertAsync(message, AlertTypes.Success);
    }

    public async Task WarningAsync(string message)
    {
        await PopupService.AlertAsync(message, AlertTypes.Warning);
    }

    public async Task ErrorMessageAsync(string message)
    {
        await PopupService.AlertAsync(alert =>
        {
            alert.Top = true;
            alert.Type = AlertTypes.Error;
            alert.Content = message;
        });
    }

    public static List<TEnum> GetEnumList<TEnum>() where TEnum : struct, Enum
    {
        return EnumHelper.GetEnumList<TEnum>();
    }

    protected async Task HandleErrorAsync(Exception exception)
    {
        await InvokeAsync(async () =>
        {
            await ErrorMessageAsync(exception.Message);
            Loading = false;
            StateHasChanged();
        });
    }

    public List<KeyValuePair<string, bool>> GetBooleanMap()
    {
        return new()
        {
            new(T("Enable"), true),
            new(T("Disabled"), false)
        };
    }
}
