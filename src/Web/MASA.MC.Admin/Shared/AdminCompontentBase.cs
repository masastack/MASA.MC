namespace MASA.MC.Admin;

public abstract class AdminCompontentBase : BDomComponentBase
{
    private I18n? _i18n;
    private GlobalConfig? _globalConfig;
    private NavigationManager? _navigationManager;

    [Inject]
    public IPopupService PopupService { get; set; } = default!;

    [Inject]
    public Mapper Mapper { get; set; } = default!;

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

    public bool Loading
    {
        get => GlobalConfig.Loading;
        set => GlobalConfig.Loading = value;
    }

    public string T(string key) => I18n.T(key);

    public async Task ConfirmAsync(string messgae, Func<PopupOkEventArgs, Task> callback, AlertTypes type = AlertTypes.Warning)
    {
        await PopupService.ConfirmAsync(
        I18n.T("Operation confirmation"),
        messgae,
        type,
        callback);
    }

    public async Task SuccessMessageAsync(string message)
    {
        await PopupService.AlertAsync(message, AlertTypes.Success);
    }

    public async Task WarningAsync(string message)
    {
        await PopupService.AlertAsync(message, AlertTypes.Warning);
    }
}
