using System.Diagnostics;
using System.IO.Pipes;

namespace DesktopEngine;

/// <summary>
/// Provides functionality to start a process for DaemonService and communicate with it.
/// </summary>
public static class DaemonServiceClient
{
    // The timeout is small since pipe connections should be on the local machine only.
    private const int TIMEOUT = 500;

    public static bool SendKillMessage()
    {
        try
        {
            SendMessage(DaemonService.MSG_KILL, false);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Could not send terminate message to daemon");
            return false;
        }
    }

    public static int SendGetStatusMessage()
    {
        try
        {
            return Int32.Parse(SendMessage(DaemonService.MSG_GET_STATUS, true));
        }
        catch (Exception)
        {
            return -1;
        }
    }

    private static string SendMessage(string message, bool expectResponse)
    {
        using var pipeClient = new NamedPipeClientStream(".", DaemonService.PIPE_NAME, PipeDirection.InOut);
        pipeClient.Connect(TIMEOUT);
        var streamString = new StreamString(pipeClient);
        streamString.WriteString(message);
        if (!expectResponse)
        {
            return null;
        }
        return streamString.ReadString();
    }

    public static Process StartServiceProcess()
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--service"
            });
            if (process == null)
            {
                Logger.Instance.Error("Could not start daemon process");
            }
            return process;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Could not start daemon process");
            return null;
        }
    }
}