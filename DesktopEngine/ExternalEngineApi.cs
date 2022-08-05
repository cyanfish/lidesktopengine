using System.Runtime.InteropServices;

namespace DesktopEngine;

public class ExternalEngineApi
{
// #if WINDOWS
//     private const string DLL_NAME = "external_engine.dll";
// #elif MACOS
     private const string DLL_NAME = "/Users/ben/Devel/external-engine/target/debug/liblibexternalengine.dylib";
// #else
    // private const string DLL_NAME = "external_engine.so";
// #endif

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