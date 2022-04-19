namespace Masa.Mc.Web.Admin.ViewModel.Users;

public class UserViewModel
{
    public Guid Id { get; set; }
    public ReceiverGroupItemType Type { get; set; }

    public string DataId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserViewModel()
    {

    }

    public UserViewModel(Guid id, ReceiverGroupItemType type, string dataId, string displayName, string avatar, string phoneNumber, string email)
    {
        Id = id;
        Type = type;
        DataId = dataId;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
