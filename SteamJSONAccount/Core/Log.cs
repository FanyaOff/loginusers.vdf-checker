using System;
using System.IO;


namespace SteamJSONAccount.Core
{
    internal enum LogType { INFO, WARNING, ERROR }
    internal class Log
    {
        public const string logFile = "latest.log";
        public static void checkLogFile(string projectName)
        {
            if (!File.Exists(logFile))
                File.WriteAllText(logFile, $"-------- {projectName} --------\n");
        }
        public static void write(string logText, LogType logType, bool timestamp)
        {
            string msg;
            if (timestamp)
                msg = $"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} / {logType}] {logText}";
            else
                msg = $"[{logType}] {logText}";
            using (StreamWriter w = File.AppendText(logFile))
            {
                w.WriteLine(msg, "\n");
            }
        }
    }
}
