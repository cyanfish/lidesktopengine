using System.Runtime.InteropServices;

namespace DesktopEngine;

public struct EngineConfig
{
    [MarshalAs(UnmanagedType.LPStr)]
    public string Id; // Unique persistent id
    [MarshalAs(UnmanagedType.LPStr)]
    public string MachineName;
    [MarshalAs(UnmanagedType.LPStr)]
    public string AuthToken;
    [MarshalAs(UnmanagedType.LPStr)]
    public string EnginePath;
    public int MaxHash;
    public int MaxThreads;
}