using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VkData.Interface;

namespace VkData.Helpers
{
    public class ConsoleLogger : ILogger
    {
        static ConsoleLogger()
        {
            AllocConsole();
        }

        public string Path { get; set; }
        public string LastLoggerMessage { get; private set; }
        public bool Enabled { get; set; } = true;

        public void Log(string message)
        {
            if (!Enabled) return;
            LastLoggerMessage = message;
            OnPropertyChanged(nameof(LastLoggerMessage));
            Console.WriteLine(message);
        }

        public void Log(Exception e)
        {
            var message = $"\nWhen: {DateTime.Now.ToShortTimeString()}" +
                          $"\n\nMessage: {e.Message}" +
                          $"\nFrom: {new StackTrace().GetFrame(1).GetMethod().Name}" +
                          $"\n{e.GetInnerExceptions()}\n\n";
            Log(message);
        }

        [DllImport("Kernel32")]
        protected static extern void AllocConsole();

        [DllImport("Kernel32")]
        protected static extern void FreeConsole();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}