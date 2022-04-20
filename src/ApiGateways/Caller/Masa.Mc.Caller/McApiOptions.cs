namespace Masa.Mc.Caller;

public class McApiOptions
{
    public string McServiceBaseAddress { get; set; }

    public McApiOptions(string mcServiceBaseAddress)
    {
        McServiceBaseAddress = mcServiceBaseAddress;
    }
}
