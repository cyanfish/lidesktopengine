using System;
using Eto.Forms;
using NativeEngine.Ui;

namespace NativeEngine.Wpf;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Eto.Platforms.Wpf).Run(new MainForm());
	}
}