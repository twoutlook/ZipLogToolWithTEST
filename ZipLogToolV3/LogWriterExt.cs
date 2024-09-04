using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipLogTool
{

    public class LogWriterExt
    {
        private readonly int _minLogLevel;  // Minimum log level to write the logs
        private readonly StreamWriter _logWriter;

        public LogWriterExt(string logFilePath, int minLogLevel)
        {
            _minLogLevel = minLogLevel;
            _logWriter = new StreamWriter(logFilePath, true, Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        public void WriteLine(int logLevel, string message)
        {
            if (logLevel >= _minLogLevel)
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [Level {logLevel}] {message}";
                Console.WriteLine(logMessage);  // Optionally write to console
                _logWriter.WriteLine(logMessage);  // Write to log file
            }
        }

        public void WriteLine(int logLevel, string format, params object[] args)
        {
            if (logLevel >= _minLogLevel)
            {
                string message = string.Format(format, args);
                WriteLine(logLevel, message);
            }
        }

        public void Close()
        {
            _logWriter.Close();
        }
    }
}
