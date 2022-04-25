namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageInfoCreateUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;
}
