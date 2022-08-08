namespace DesktopEngine.Mac;

public class MacServiceManager : OsServiceManager
{
    private const string PLIST_FILE_NAME = "org.lichess.DesktopEngine.plist";

    private static string PlistPath
    {
        get
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = Path.Combine(userProfile, "Library", "LaunchAgents", PLIST_FILE_NAME);
            return path;
        }
    }

    public override bool IsRegistered => File.Exists(PlistPath);

    public override void Register()
    {
        // See https://www.launchd.info/ for help with configuration
        var serviceDef = $"""
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
	<dict>
		<key>Label</key>
		<string>org.lichess.DesktopEngine</string>
		<key>Program</key>
		<string>{ Environment.ProcessPath}</string>
		<key>ProgramArguments</key>
		<array>
			<string>{ Environment.ProcessPath }</string>
			<string>--service</string>
		</array>
		<key>RunAtLoad</key>
		<true/>
	</dict>
</plist>
""";
        File.WriteAllText(PlistPath, serviceDef);
    }

    public override void Unregister()
    {
        File.Delete(PlistPath);
    }
}