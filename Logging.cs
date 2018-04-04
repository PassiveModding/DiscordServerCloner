using Serilog;

namespace DiscordServerCloner
{
    public class Logger
    {
        public static void LogInfo(string message)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information($"{message}");
        }

        public static void LogError(string message)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Error($"{message}");
        }
    }
}