using System;
using System.Collections.Generic;
using System.Linq;

using HidSharp;

namespace winhiddump
{
    class Program
    {
        static string GetArgument(IEnumerable<string> args, string option) => args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();

        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World Tod!");

            string vidpid = GetArgument(args, "--vidpid");
            Console.WriteLine("Got vidpid: " + vidpid);

            var list = DeviceList.Local;
            foreach (var hidDevice in list.GetHidDevices())
            {
                Console.WriteLine(hidDevice.ToString() + " @ " + hidDevice.DevicePath);
                byte[] rawReportDescriptor = hidDevice.GetRawReportDescriptor();
                Console.WriteLine("Report Descriptor:");
                Console.WriteLine("  {0} ({1} bytes)", string.Join(" ", rawReportDescriptor.Select(d => d.ToString("X2"))), rawReportDescriptor.Length);
            }
//            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
