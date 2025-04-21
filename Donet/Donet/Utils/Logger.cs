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

    public class Logger
    {
        private static volatile Atomic<Logger> instance = null;

        public static void Log(LogLevel level, string message)
        {
            if (instance == null)
                return;

            using var local = instance.Lock();
            local.Value.LocalLog(level, message);
        }

        public static void Initialize()
        {
            instance = new Atomic<Logger>(new Logger());
            using var local = instance.Lock();
            local.Value.Create();

            AppDomain.CurrentDomain.ProcessExit += HandleExit;
        }

        public static void Dispose()
        {
            if (instance == null)
                return;

            AppDomain.CurrentDomain.ProcessExit -= HandleExit;

            using var local = instance.Lock();
            local.Value.Delete();
            local.Set(null);
            instance = null;
        }

        private static void HandleExit(object sender, EventArgs evt)
        {
            Log(LogLevel.Error, "[Logger] unexpected termination has been detected.");
            using var local = instance.Lock();
            local.Value.Save();
        }

        private string path = null;
        private byte[] logBuf = null;
        private int pointer = 0;

        public void Create()
        {
            path = Path.Combine(Environment.CurrentDirectory, "Logs");
            logBuf = new byte[16777216];
            pointer = 0;
        }

        public void Delete()
        {
            Save();
            path = null;
            logBuf = null;
            pointer = 0;
        }

        public void LocalLog(LogLevel level, string message)
        {
            string log = MakeMessage(level, message);
            int byteCount = Encoding.UTF8.GetByteCount(log);

            if (byteCount > logBuf.Length)
            {
                LocalLog(LogLevel.Warning, "log message is too large and has been skipped.");
                return;
            }

            if (byteCount > logBuf.Length - pointer)
                Save();
#if DEBUG
            Console.Write(log);
#endif
            pointer += Encoding.UTF8.GetBytes(log, new Span<byte>(logBuf, pointer, byteCount));
        }

        public void Save()
        {
            if (logBuf == null || logBuf.Length == 0) return;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string textPath = MakePath();
            using (FileStream fs = File.Create(textPath))
            {
                fs.Write(logBuf, 0, pointer);
            }
            pointer = 0;
        }

        private string MakePath()
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy.MM.dd-HH.mm.ss");
            long ticks = now.Ticks;

            string filename = $"{timestamp}_{ticks}.txt";
            return Path.Combine(path, filename);
        }

        private string MakeMessage(LogLevel level, string message)
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
