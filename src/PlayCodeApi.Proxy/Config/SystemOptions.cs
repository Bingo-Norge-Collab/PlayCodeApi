namespace PlayCodeApi.Proxy.Config;

public class SystemOptions
{
    public SystemConfig[] Systems { get; set; } = [];
}

public class SystemConfig
{
    public int SystemId { get; set; }
    public string Connection { get; set; } = null!;
    public string Type { get; set; } = null!;
}