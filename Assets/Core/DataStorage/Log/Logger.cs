using System;
using UnityEngine;

namespace Core.DataStorage.Log
{
    internal static class Logger
    {
        public static void LogError(string message)
        {
            Debug.LogError($"[DataStorage] {message}");
        }

        public static void LogError(Exception exception)
        {
            Debug.LogError($"[DataStorage] {exception}");
        }
    }
}