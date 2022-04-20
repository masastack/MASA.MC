namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroupItem : Entity<Guid>
{
    public Guid GroupId { get; protected set; }

    public string DataId { get; protected set; }

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    public string PhoneNumber { get; protected set; } = string.Empty;

    public string Email { get; protected set; } = string.Empty;

    public ReceiverGroupItemType Type { get; protected set; }

    public ReceiverGroupItem(Guid groupId, string dataId, ReceiverGroupItemType type, string displayName, string avatar = "", string phoneNumber = "", string email = "")
    {
        GroupId = groupId;
        DataId = dataId;
        Type = type;

        SetContent(displayName, avatar, phoneNumber, email);
    }

    public void SetContent(string displayName, string avatar, string phoneNumber, string email)
    {
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
