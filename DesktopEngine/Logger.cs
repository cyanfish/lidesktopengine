using NLog;
using NLog.Config;
using NLog.Targets;

namespace DesktopEngine;

public static class Logger
{
    public static NLog.Logger Instance { get; }
    
    static Logger()
    {
        var config = new LoggingConfiguration();
        var target = new FileTarget
        {
            FileName = Path.Combine(Paths.LOG_FILE_PATH),
            Layout = "${longdate} ${processid} ${message} ${exception:format=tostring}",
            ArchiveAboveSize = 100000,
            MaxArchiveFiles = 5
        };
        config.AddTarget("logfile", target);
        var rule = new LoggingRule("*", LogLevel.Debug, target);
        config.LoggingRules.Add(rule);
        LogManager.Configuration = config;
        Instance = LogManager.GetLogger("DesktopEngine");
    }
}