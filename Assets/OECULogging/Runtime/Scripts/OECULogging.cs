using System;
using UnityEngine;
using com.naosv.OECULogging.Core;

public static class OECULogging
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void RuntimeInitialize()
    {
        LogWriter.RuntimeInitialize();
        Log("Logging Client initialized.", "OECULogging");
    }

    public static void Log(string message, string logType = "INFO")
    {
        LogWriter.Log(message, logType);
    }

    public static void LogError(string message)
    {
        Log(message, "ERROR");
    }

    public static void LogWarning(string message)
    {
        Log(message, "WARNING");
    }

    public static void LogException(string message)
    {
        Log(message, "EXCEPTION");
    }

    public static void LogInfo(string message)
    {
        Log(message, "INFO");
    }

    public static void LogDebug(string message)
    {
        Log(message, "DEBUG");
    }

    public static void SetAutoCatchUnhandledExceptions(bool enable)
    {
        ErrorCatcher.catchUnhandledExceptions = enable;
        Log($"Auto catching of unhandled exceptions {(enable ? "enabled" : "disabled")}.", "OECULogging");
    }

    public static void SetAutoCatchUnityLogErrors(bool enable)
    {
        ErrorCatcher.catchLogErrors = enable;
        Log($"Auto catching of Unity log errors {(enable ? "enabled" : "disabled")}.", "OECULogging");
    }

    public static void SetAutoCatchErrors(bool enable)
    {
        SetAutoCatchUnhandledExceptions(enable);
        SetAutoCatchUnityLogErrors(enable);
    }

    public static void SetAutoCatchUnityLogWarnings(bool enable)
    {
        ErrorCatcher.catchLogWarnings = enable;
        Log($"Auto catching of Unity log warnings {(enable ? "enabled" : "disabled")}.", "OECULogging");
    }

    public static void SetAutoCatchUnityLogInfos(bool enable)
    {
        ErrorCatcher.catchLogInfos = enable;
        Log($"Auto catching of Unity log infos {(enable ? "enabled" : "disabled")}.", "OECULogging");
    }

    public static void GameStart()
    {
        Log("Game started at " + DateTime.Now, "GAME_START");
    }

    public static void GameEnd()
    {
        Log("Game ended at " + DateTime.Now, "GAME_END");
    }

    public static void GameRevert()
    {
        Log("Game reverted at " + DateTime.Now, "GAME_REVERT");
    }

    public static void EnableWebhook(string url)
    {
        LogWriter.EnableWebhook(url);
    }

    public static void DisableWebhook()
    {
        LogWriter.DisableWebhook();
    }

    public static void AddCustomWebhook(string type)
    {
        LogWriter.AddCustomWebhook(type);
    }

    public static void RemoveCustomWebhook(string type)
    {
        LogWriter.RemoveCustomWebhook(type);
    }

}