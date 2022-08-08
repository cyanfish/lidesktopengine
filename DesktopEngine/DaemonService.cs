using System.Diagnostics;
using System.IO.Pipes;

namespace DesktopEngine;

public static class DaemonService
{
    public static string PIPE_NAME = "lidesktopengine1";
    public static string MSG_GET_STATUS = "getstatus";
    public static string MSG_KILL = "kill";
    // The timeout is small since pipe connections should be on the local machine only.
    private const int TIMEOUT = 1000;

    private static int _demoStatus = 4;
    
    public static void Run()
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
                streamString.WriteString(_demoStatus.ToString());
            }
            pipeServer.Disconnect();
        }
    }

    public static void SendKillMessage()
    {
        SendMessage(MSG_KILL, false);
    }

    public static int SendGetStatusMessage()
    {
        try
        {
            return int.Parse(SendMessage(MSG_GET_STATUS, true));
        }
        catch (Exception)
        {
            return -1;
        }
    }

    private static string SendMessage(string message, bool expectResponse)
    {
        using var pipeClient = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.InOut);
        pipeClient.Connect(TIMEOUT);
        var streamString = new StreamString(pipeClient);
        streamString.WriteString(message);
        if (!expectResponse)
        {
            return null;
        }
        return streamString.ReadString();
    }

    public static void StartServiceProcess()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Environment.ProcessPath!,
            Arguments = "--service"
        });
    }
}