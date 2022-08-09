using DesktopEngine.Ui;

namespace DesktopEngine.Wpf;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		EntryPoint.Run(args, Eto.Platforms.Wpf);
	}
}