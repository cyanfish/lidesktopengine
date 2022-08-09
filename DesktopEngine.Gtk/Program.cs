using DesktopEngine.Ui;

namespace DesktopEngine.Gtk;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		EntryPoint.Run(args, Eto.Platforms.Gtk);
	}
}