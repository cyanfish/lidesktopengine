using Eto.Forms;

namespace DesktopEngine.Ui;

public static class EntryPoint
{
    public static void Run(string[] args, string platformType)
    {
        if (args.Any(x => x == "--service"))
        {
            DaemonService.Run();
            return;
        }

        var app = new Application(platformType);
        app.UnhandledException += OnUnhandledException;
        app.Run(new MainForm());
    }

    private static void OnUnhandledException(object sender, Eto.UnhandledExceptionEventArgs e)
    {
        Logger.Instance.Error(e.ExceptionObject as Exception, "Unhandled error");
    }
}