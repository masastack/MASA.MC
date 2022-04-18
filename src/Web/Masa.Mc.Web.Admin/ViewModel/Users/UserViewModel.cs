namespace Masa.Mc.Web.Admin.ViewModel.Users;

public class UserViewModel
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Group { get; set; }

    public UserViewModel(string displayName, string avatar, string phoneNumber, string email, string group = "用户")
    {
        Id = Guid.NewGuid();
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        Group = group;
    }
}
