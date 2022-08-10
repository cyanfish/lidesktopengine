using System.Security.Cryptography;

namespace DesktopEngine;

/// <summary>
/// User configuration as shown in the "Setup" window.
/// </summary>
public class UserConfig
{
    public static UserConfig Default()
    {
        var idGenerator = RandomNumberGenerator.Create();
        var bytes = new byte[16];
        idGenerator.GetBytes(bytes);
        var id = Convert.ToHexString(bytes);
        return new UserConfig
        {
            Id = id,
            MachineName = Environment.MachineName,
            AuthToken = null,
            UseCustomEngine = false,
            CustomEnginePath = null,
            MaxHash = 1024,
            MaxThreads = Math.Max(1, Environment.ProcessorCount - 1)
        };
    }

    public string Id { get; set; }

    public string MachineName { get; set; }

    public string AuthToken { get; set; }

    public bool UseCustomEngine { get; set; }

    public string CustomEnginePath { get; set; }

    public int MaxHash { get; set; }

    public int MaxThreads { get; set; }

    public bool IsValid => !string.IsNullOrWhiteSpace(MachineName) &&
                           !string.IsNullOrWhiteSpace(AuthToken) &&
                           (!UseCustomEngine || File.Exists(CustomEnginePath))
                           && MaxHash > 0
                           && MaxThreads > 0;
}