using System.Runtime.InteropServices;

namespace DesktopEngine;

public class ExternalEngineApi
{
    // The extension (.dll, .dylib, .so) will be automatically added per platform
    private const string DLL_NAME = "liblibexternalengine";

    public const int SUCCESS = 0;
    public const int UNKNOWN_ERROR = 1;
    public const int ENGINE_ERROR = 2;
    public const int NOT_STARTED = 3;
    public const int DISCONNECTED = 4;
    public const int CONNECTED_IDLE = 5;
    public const int CONNECTED_RUNNING = 6;

    [DllImport(DLL_NAME)]
    public static extern int StartListening(ref EngineConfig config);

    [DllImport(DLL_NAME)]
    public static extern int GetStatus();

    [DllImport(DLL_NAME)]
    public static extern int StopListening();
}