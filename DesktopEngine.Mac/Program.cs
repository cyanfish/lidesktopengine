using DesktopEngine.Ui;

namespace DesktopEngine.Mac;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		OsServiceManager.Instance = new MacServiceManager();
		EntryPoint.Run(args, Eto.Platforms.Mac64);
	}
}