namespace DesktopEngine;

public abstract class OsServiceManager
{
    public static OsServiceManager Instance { get; set; }
    
    public abstract bool IsRegistered { get; }

    public abstract void Register();

    public abstract void Unregister();
}