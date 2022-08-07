using System;
using Eto.Forms;
using DesktopEngine.Ui;

namespace DesktopEngine.Wpf;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		if (args.Any(x => x == "--service"))
		{
			DaemonService.Run();
			return;
		}
		new Application(Eto.Platforms.Wpf).Run(new MainForm());
	}
}