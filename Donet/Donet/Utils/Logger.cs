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
        private static object locker = new object();

        private static void HandleExit(object sender, EventArgs evt)
        {
            Log(LogLevel.Error, "[Logger] unexpected termination has been detected.");
            Save();
        }

        private static string path = null;
        private static byte[] logBuf = null;
        private static int pointer = 0;

        public static void Initialize()
        {
            path = Path.Combine(Environment.CurrentDirectory, "Logs");
            logBuf = new byte[16777216];
            pointer = 0;

            AppDomain.CurrentDomain.ProcessExit += HandleExit;
        }

        public static void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit -= HandleExit;

            Save();
            path = null;
            logBuf = null;
            pointer = 0;
        }

        public static void Log(LogLevel level, string message)
        {
            if (path == null || string.IsNullOrEmpty(message))
                return;

            lock (locker)
            {
                string log = MakeMessage(level, message);
                int byteCount = Encoding.UTF8.GetByteCount(log);

                if (byteCount > logBuf.Length)
                {
                    Log(LogLevel.Warning, "log message is too large and has been skipped.");
                    return;
                }

                if (byteCount > logBuf.Length - pointer)
                    Save();
#if DEBUG
                Console.Write(log);
#endif
                pointer += Encoding.UTF8.GetBytes(log, new Span<byte>(logBuf, pointer, byteCount));
            }
        }

        public static void Save()
        {
            if (path == null || logBuf == null || logBuf.Length == 0)
                return;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string textPath = MakePath();
            using (FileStream fs = File.Create(textPath))
            {
                fs.Write(logBuf, 0, pointer);
            }
            pointer = 0;
        }

        private static string MakePath()
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy.MM.dd-HH.mm.ss");
            long ticks = now.Ticks;

            string filename = $"{timestamp}_{ticks}.txt";
            return Path.Combine(path, filename);
        }

        private static string MakeMessage(LogLevel level, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{level}] [");
            sb.Append(DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss"));
            sb.Append("] ");
            sb.Append(message);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
