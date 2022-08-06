using System.Security.Cryptography;
using Newtonsoft.Json;

namespace DesktopEngine;

public class UserConfig
{
    private static readonly string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string CONFIG_FILE_PATH = Path.Combine(APP_DATA, "lidesktopengine", "config.json");

    public static void Save(UserConfig config)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CONFIG_FILE_PATH)!);
            File.WriteAllText(CONFIG_FILE_PATH, JsonConvert.SerializeObject(config));
            Saved?.Invoke(null, EventArgs.Empty);
        }
        catch (Exception)
        {
            // TODO: Handle exceptions
        }
    }

    public static UserConfig Load()
    {
        try
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                return JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(CONFIG_FILE_PATH));
            }
        }
        catch (Exception)
        {
            // TODO: Handle exceptions
        }

        return Default();
    }

    private static UserConfig Default()
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

    public static event EventHandler Saved;

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