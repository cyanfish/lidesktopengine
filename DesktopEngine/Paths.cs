namespace DesktopEngine;

/// <summary>
/// Helper to get paths for app configuration and logging files.
/// </summary>
public class Paths
{
    private static readonly string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string DATA_FOLDER = Path.Combine(APP_DATA, "lidesktopengine");
    public static readonly string CONFIG_FILE_PATH = Path.Combine(DATA_FOLDER, "config.json");
    public static readonly string LOG_FILE_PATH = Path.Combine(DATA_FOLDER, "log.txt");

    static Paths()
    {
        try
        {
            Directory.CreateDirectory(DATA_FOLDER);
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Error creating data folder");
        }
    }
}