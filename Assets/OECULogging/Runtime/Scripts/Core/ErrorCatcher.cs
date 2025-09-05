using System;
using System.Threading.Tasks;
using UnityEngine;

namespace com.naosv.OECULogging.Core
{
    internal static class ErrorCatcher
    {
        public static bool catchUnhandledExceptions = true;
        public static bool catchLogErrors = true;
        public static bool catchLogWarnings = false;
        public static bool catchLogInfos = false;

        internal static async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!catchUnhandledExceptions)
            {
                return;
            }
            // Debug.LogError("OECULogging: Unhandled exception caught.");
            Exception exception = e.ExceptionObject as Exception;
            // Debug.LogError($"Exception: {exception.Message}\nStack Trace: {exception.StackTrace}");
            await SafeWriteAsync($"Unhandled exception: {exception.Message}\n{exception.StackTrace}", "EXCEPTION");
        }

        internal static async void HandleLogError(string condition, string stackTrace, LogType type)
        {
            if (!catchLogErrors)
            {
                return;
            }
            if (type == LogType.Error)
            {
                await SafeWriteAsync($"Log error: {condition}\n{stackTrace}", "ERROR");
                // Debug.Log($"OECULogging: Log error caught. Type: {type}, Condition: {condition}");
            }
            else if (type == LogType.Exception)
            {
                await SafeWriteAsync($"Log exception: {condition}\n{stackTrace}", "EXCEPTION");
            }
            else if (type == LogType.Warning && catchLogWarnings)
            {
                await SafeWriteAsync($"Log warning: {condition}\n{stackTrace}", "WARNING");
            }
            else if (type == LogType.Log && catchLogInfos)
            {
                await SafeWriteAsync($"Log info: {condition}\n{stackTrace}", "INFO");
            }
        }

        internal static Task SafeWriteAsync(string msg, string logType)
        {
            return LogWriter.Log(msg, logType);
        }
    }
}