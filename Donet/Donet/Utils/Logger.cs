using System;
using System.IO;
using System.Text;

namespace Donet.Utils
{
    public enum LogLevel
    {
        Notify,
        Warning,
        Error
    }

    public static class Logger
    {
        private static string path = null;
        private static byte[] logBuf = null;
        private static int pointer = 0;

        public static void Initialize()
        {
            path = System.Reflection.Assembly.GetEntryAssembly().Location + "\\Logs";
            logBuf = new byte[16777216];
            pointer = 0;
        }

        public static void Dispose()
        {
            Save();
            path = null;
            logBuf = null;
            pointer = 0;
        }

        public static void Log(LogLevel level, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(level.ToString());
            sb.Append(']');
            AddDate(sb);
            sb.Append(": ");
            sb.Append(message);
            sb.AppendLine();

            string log = sb.ToString();
            int byteCount = Encoding.UTF8.GetByteCount(log);

            if (byteCount > logBuf.Length - pointer)
            {
                Save();
            }

            pointer += Encoding.UTF8.GetBytes(log, new Span<byte>(logBuf, pointer, byteCount));
        }

        public static void Save()
        {
            if (logBuf == null || logBuf.Length == 0) return;

            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            sb.Append('\\');
            AddDate(sb);
            sb.Append(".txt");
            using (FileStream fs = File.OpenWrite(sb.ToString()))
            {
                fs.Write(logBuf, 0, pointer);
            }
            pointer = 0;
        }

        private static void AddDate(StringBuilder sb)
        {
            sb.Append(DateTime.Now.Year.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Month.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Day.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Hour.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Minute.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Second.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Ticks.ToString());
        }
    }
}
