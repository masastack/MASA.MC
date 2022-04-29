namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public class MessageTemplateItemChangedEventArgs
{
    public string OldCode { get; set; } = string.Empty;

    public string NewCode { get; set; } = string.Empty;

    public MessageTemplateItemChangedEventArgs(string oldCode, string newCode)
    {
        OldCode = oldCode;
        NewCode = newCode;
    }
}
