namespace MASA.MC.Contracts.Admin.Options.Channels;

public class EmailChannelOptions
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Smtp { get; set; } = string.Empty;
    public bool Ssl { get; set; }
    public int Port { get; set; }
}
