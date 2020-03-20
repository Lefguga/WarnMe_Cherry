using System;
using static WarnMe_Cherry.Datenbank.WARNME_CONFIG;

namespace WarnMe_Cherry
{
    internal static class Global
    {
        public static Datenbank.Datenbank DATA = new Datenbank.Datenbank(SAVE_CONFIG.CONFIG_FILE_NAME);
        public static void INFO(string message) =>  Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss\\.fff}] INF: {message}");
        public static void DEBUG(string message) => Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss\\.fff}] DBG: {message}");
        public static void WRITE(string message) => Console.WriteLine(message);
    }
}
