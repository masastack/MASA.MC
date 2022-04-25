namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageInfoDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public MessageInfoDto(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
