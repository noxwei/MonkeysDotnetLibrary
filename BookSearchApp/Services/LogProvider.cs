using System;
using System.Collections.Generic;

namespace BookSearchApp.Services
{
    public static class LogProvider
    {
        public static List<string> LogMessages { get; } = new List<string>();
        
        public static void AddLog(string message)
        {
            LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            
            // Keep only the last 100 log messages
            if (LogMessages.Count > 100)
            {
                LogMessages.RemoveAt(0);
            }
        }
        
        public static void ClearLogs()
        {
            LogMessages.Clear();
        }
    }
} 