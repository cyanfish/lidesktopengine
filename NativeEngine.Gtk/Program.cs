﻿using System;
using Eto.Forms;
using NativeEngine.Ui;

namespace NativeEngine.Gtk;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Eto.Platforms.Gtk).Run(new MainForm());
	}
}