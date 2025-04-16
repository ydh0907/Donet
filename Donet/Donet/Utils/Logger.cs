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
        private static Atomic<Logger> instance = null;

        public static void Log(LogLevel level, string message)
        {
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
            AppDomain.CurrentDomain.ProcessExit -= HandleExit;

            using var local = instance.Lock();
            local.Value.Delete();
            local.Set(null);
            instance = null;
        }

        private static void HandleExit(object sender, EventArgs evt)
        {
            Log(LogLevel.Error, "[Server] unexpected termination has been detected.");
            using var local = instance.Lock();
            local.Value.Save();
        }

        private string path = null;
        private byte[] logBuf = null;
        private int pointer = 0;

        public void Create()
        {
            path = Environment.CurrentDirectory + "\\Logs";
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
            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            sb.Append('\\');
            AddDate(sb);
            sb.Append(".txt");
            return sb.ToString();
        }

        private string MakeMessage(LogLevel level, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{level}] [");
            AddDate(sb);
            sb.Append("] ");
            sb.Append(message);
            sb.AppendLine();

            return sb.ToString();
        }

        private void AddDate(StringBuilder sb)
        {
            sb.Append(DateTime.Now.Year.ToString());
            sb.Append('.');
            sb.Append(DateTime.Now.Month.ToString());
            sb.Append('.');
            sb.Append(DateTime.Now.Day.ToString());
            sb.Append('-');
            sb.Append(DateTime.Now.Hour.ToString());
            sb.Append('.');
            sb.Append(DateTime.Now.Minute.ToString());
            sb.Append('.');
            sb.Append(DateTime.Now.Second.ToString());
        }
    }
}
