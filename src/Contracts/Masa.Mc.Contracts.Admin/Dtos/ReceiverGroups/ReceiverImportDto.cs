namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

[Importer(HeaderRowIndex = 2, IsDisableAllFilter = true)]
public class ReceiverImportDto
{
    [ImporterHeader(Name = "昵称")]
    public string DisplayName { get; set; } = string.Empty;

    [ImporterHeader(Name = "头像")]
    public string Avatar { get; set; } = string.Empty;

    [ImporterHeader(Name = "手机号")]
    public string PhoneNumber { get; set; } = string.Empty;

    [ImporterHeader(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;
}
