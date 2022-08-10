using System.IO.Pipes;

namespace DesktopEngine;

/// <summary>
/// Daemon service that runs libexternalengine in a background process and communicates with the frontend process over
/// named pipes.
/// </summary>
public static class DaemonService
{
    public static string PIPE_NAME = "lidesktopengine1";
    public static string MSG_GET_STATUS = "getstatus";
    public static string MSG_KILL = "kill";

    private static int _demoStatus = 4;
    
    public static void Run()
    {
        // TODO: Validate and use config
        try
        {
            new Timer(_ => _demoStatus = 5, null, 1500, 0);
            using var pipeServer = new NamedPipeServerStream(PIPE_NAME, PipeDirection.InOut);
            while (true)
            {
                pipeServer.WaitForConnection();
                var streamString = new StreamString(pipeServer);
                var msg = streamString.ReadString();
                if (msg == MSG_KILL)
                {
                    break;
                }
                if (msg == MSG_GET_STATUS)
                {
                    // streamString.WriteString(_demoStatus.ToString());
                    streamString.WriteString(ExternalEngineApi.GetStatus().ToString());
                }
                pipeServer.Disconnect();
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.Error(ex, "Error running daemon server pipe");
        }
    }
}