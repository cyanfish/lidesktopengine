using Newtonsoft.Json;

namespace DesktopEngine;

/// <summary>
/// Manages saving and loading of UserConfig.
/// </summary>
public static class UserConfigManager
{
    public static void Save(UserConfig config)
    {
        try
        {
            File.WriteAllText(Paths.CONFIG_FILE_PATH, JsonConvert.SerializeObject(config));
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Error loading config");
        }
        Saved?.Invoke(null, EventArgs.Empty);
    }

    public static UserConfig Load()
    {
        try
        {
            if (File.Exists(Paths.CONFIG_FILE_PATH))
            {
                return JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(Paths.CONFIG_FILE_PATH));
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Error saving config");
        }
        return UserConfig.Default();
    }

    public static event EventHandler Saved;
}