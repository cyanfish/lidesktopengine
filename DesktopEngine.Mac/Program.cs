using System;
using Eto.Forms;
using DesktopEngine.Ui;

namespace DesktopEngine.Mac;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		OsServiceManager.Instance = new MacServiceManager();
		if (args.Any(x => x == "--service"))
		{
			DaemonService.Run();
			return;
		}
		new Application(Eto.Platforms.Mac64).Run(new MainForm());
	}
}