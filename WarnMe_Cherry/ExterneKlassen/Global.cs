using System;

namespace WarnMe_Cherry
{
    class Global
    {
        public static void INFO(string message) =>  Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss\\.fff}] INF: {message}");
        public static void DEBUG(string message) => Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss\\.fff}] DBG: {message}");
        public static void WRITE(string message) => Console.WriteLine(message);
    }
}
