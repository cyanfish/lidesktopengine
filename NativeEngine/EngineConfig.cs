using System.Runtime.InteropServices;

namespace NativeEngine;

public struct EngineConfig
{
    [MarshalAs(UnmanagedType.LPStr)]
    public string AuthToken;
    [MarshalAs(UnmanagedType.LPStr)]
    public string EnginePath;
    public int MaxHash;
    public int MaxThreads;
}