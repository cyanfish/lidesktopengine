﻿using System;
using Eto.Forms;
using NativeEngine.Ui;

namespace NativeEngine.Mac;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Eto.Platforms.Mac64).Run(new MainForm());
	}
}