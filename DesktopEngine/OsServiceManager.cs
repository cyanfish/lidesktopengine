namespace DesktopEngine;

/// <summary>
/// Abstraction for OS-specific "run on startup" registration logic.
/// </summary>
public abstract class OsServiceManager
{
    public static OsServiceManager Instance { get; set; }
    
    public abstract bool IsRegistered { get; }

    public abstract void Register();

    public abstract void Unregister();
}