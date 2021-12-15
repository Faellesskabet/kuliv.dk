using System;
using System.Globalization;
using Dikubot.DataLayer.Logic.Email;
using Dikubot.DataLayer.Logic.Email.Emails;

namespace Dikubot.DataLayer.Static
{
    public static class Logger
    {
        public static void Debug(string log)
        {
            #if DEBUG
            Console.WriteLine($"[DEBUG] {DateTime.Now.ToString(CultureInfo.CurrentCulture)} {log}");
            #endif
        }

        public static async void DebugEmail(string message)
        {
            DebugEmail debug = new DebugEmail(message);
            await EmailService.SendEmail(debug);
        }
    }
}