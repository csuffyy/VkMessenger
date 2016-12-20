using System;
using System.ComponentModel;

namespace VkData.Interface
{
    public interface ILogger : IFileService, INotifyPropertyChanged
    {
        string LastLoggerMessage { get; }
        bool Enabled { get; }
        void Log(string message);
        void Log(Exception e);
    }
}