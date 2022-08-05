using System;
using Eto.Forms;
using DesktopEngine.Ui;

namespace DesktopEngine.Gtk;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Eto.Platforms.Gtk).Run(new MainForm());
	}
}