using Serilog;

namespace Core.Library.Clean.AdditionalService
{
    public static class Logger
    {
        public static void LogInformation(string[] logs, bool shouldConsole = false, bool shouldWrite = false)
        {
            string logText = "log";

            foreach (string log in logs)
            {
                logText += ", " + log;
            }

            if (shouldConsole)
            {
                Console.WriteLine(logText);
            }

            if (shouldWrite)
            {
                Log.Information(logText);
            }
        }
    }
}
