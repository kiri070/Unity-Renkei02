using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OECULogging.Runtime")]
namespace com.naosv.OECULogging.Core
{
    internal static class LogWriter
    {
        private static HttpLoggingClient _client;
        private static CancellationTokenSource _cts;

        public static void RuntimeInitialize()
        {
            Application.quitting += OnApplicationQuit;
            AppDomain.CurrentDomain.UnhandledException += ErrorCatcher.OnUnhandledException;
            Application.logMessageReceived += ErrorCatcher.HandleLogError;

            _cts = new CancellationTokenSource();

            string productName = Application.productName;
            string persistentPath = Application.persistentDataPath;

            _client = new HttpLoggingClient(productName, persistentPath, TimeSpan.FromSeconds(1));
            _client.Start(_cts.Token);

            _ = Task.Run(() => BootstrapAsync(_cts.Token));
        }

        private static void OnApplicationQuit()
        {
            Log("Application Quit.", "OECULogging");

            _cts?.Cancel();
            _client?.StopAndFlush(TimeSpan.FromSeconds(3));

            Application.quitting -= OnApplicationQuit;
            AppDomain.CurrentDomain.UnhandledException -= ErrorCatcher.OnUnhandledException;
            Application.logMessageReceived -= ErrorCatcher.HandleLogError;
        }

        private static async Task BootstrapAsync(CancellationToken tk)
        {
            await FetchServerAddress.FetchWithRetryAsync(tk);

            var url = $"http://{FetchServerAddress.Host}:{FetchServerAddress.Port}/api/oeculogging.php";
            _client.SetEndPoint(url);

            await Log("Address Fetched.", "OECULogging");
        }

        public static Task Log(string message, string logType = "INFO")
        {
            _client.Enqueue(message, logType);
            return Task.CompletedTask;
        }

        // Webhook API
        public static void EnableWebhook(string url)
        {
            _client?.EnableWebhook(url);
            _ = Log($"Webhook enabled.", "OECULogging");
        }

        public static void DisableWebhook()
        {
            _client?.DisableWebhook();
            _ = Log("Webhook disabled.", "OECULogging");
        }

        public static void AddCustomWebhook(string type)
        {
            _client?.AddWebhookType(type);
            _ = Log($"Webhook type added: {type}", "OECULogging");
        }

        public static void RemoveCustomWebhook(string type)
        {
            _client?.RemoveWebhookType(type);
            _ = Log($"Webhook type removed: {type}", "OECULogging");
        }


    }
}
