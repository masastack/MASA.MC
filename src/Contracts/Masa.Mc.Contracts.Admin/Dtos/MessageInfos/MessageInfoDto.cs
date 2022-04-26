namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos;

public class MessageInfoDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;

    public MessageInfoDto()
    {

    }

    public MessageInfoDto(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
