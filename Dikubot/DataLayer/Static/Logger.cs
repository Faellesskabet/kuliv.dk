using System;
using System.Globalization;

namespace Dikubot.DataLayer.Static
{
    public static class Logger
    {
        public static void Debug(string log)
        {
            if (!main.IS_DEV)
            {
                return;
            }

            Console.WriteLine($"[DEBUG] {DateTime.Now.ToString(CultureInfo.CurrentCulture)} {log}");
        }
    }
}