using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using VkData.Interface;

namespace VkData.Helpers

{
    public class FileLogger : ILogger
    {
        private string _path;

        public FileLogger()
        {
        }


        public FileLogger(string path)
        {
            Path = path;
            FileName = "exceptions";
        }

        public FileLogger(string path, string fileName = "exceptions") : this(path)
        {
            FileName = fileName;
        }

        private static string StackTrace
        {
            get { return new StackTrace().GetFrame(1).GetMethod().Name; }
        }

        public string FileName { get; }

        public string LastLoggerMessage { get; private set; }
        public bool Enabled { get; set; } = true;

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                FileUtils.CreateIfNotExist(Path);
            }
        }

        public void Log(string message) =>
            Log(message, null, StackTrace);

        public void Log(Exception e)
            => Log(e.Message, e.GetInnerExceptions(), StackTrace);

        private void Log(string message, string innerException, string method)
        {
            if (!Enabled) return;
            var _message = $"\nWhen: {DateTime.Now.ToShortTimeString()}\n\nMessage: {message}\nFrom: {method}\n{innerException}\n\n";

            LastLoggerMessage = _message;
            OnPropertyChanged(nameof(LastLoggerMessage));

            using (var writer = new StreamWriter(FileUtils.JoinAndCreate(Path, $"{FileName}.log"), true))
            {
                writer.Write(_message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}