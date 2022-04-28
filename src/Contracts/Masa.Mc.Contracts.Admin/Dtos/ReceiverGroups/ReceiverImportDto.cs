namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

[Importer(HeaderRowIndex = 2, IsDisableAllFilter = true)]
public class ReceiverImportDto
{
    /// <summary>
    ///  昵称
    /// </summary>
    [ImporterHeader(Name = "昵称")]
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>
    ///  头像
    /// </summary>
    [ImporterHeader(Name = "头像")]
    public string Avatar { get; set; } = string.Empty;
    /// <summary>
    ///  头像
    /// </summary>
    [ImporterHeader(Name = "手机号")]
    public string PhoneNumber { get; set; } = string.Empty;
    /// <summary>
    ///  邮箱
    /// </summary>
    [ImporterHeader(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;
}
