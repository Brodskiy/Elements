using System;
using UnityEngine;

namespace Core.DI.Log
{
    internal static class Logger
    {
        public static void LogError(string message)
        {
            Debug.LogError($"[DIContainer] {message}");
        }
        
        public static void LogError(Exception exception)
        {
            Debug.LogError($"[DIContainer] {exception}");
        }
    }
}