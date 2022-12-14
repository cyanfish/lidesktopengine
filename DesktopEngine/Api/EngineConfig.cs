using System.Runtime.InteropServices;

namespace DesktopEngine;

/// <summary>
/// Configuration for the libexternalengine api.
/// </summary>
public struct EngineConfig
{
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string Id; // Unique persistent id
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string MachineName;
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string AuthToken;
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string EnginePath;
    public int MaxHash;
    public int MaxThreads;
}