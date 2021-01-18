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
            Console.WriteLine("WinHIDdump:");
            Console.WriteLine("[{0}]", string.Join(", ", args));

            string vidpid = GetArgument(args, "--vidpid");
//            Console.WriteLine("Got vidpid: " + vidpid);

            var list = DeviceList.Local;
            foreach (var hidDevice in list.GetHidDevices())
            {
                Console.WriteLine(hidDevice.ToString() + " @ " + hidDevice.DevicePath);
                byte[] rawReportDescriptor = hidDevice.GetRawReportDescriptor();
                Console.WriteLine("Report Descriptor:");
                Console.WriteLine("  {0} ({1} bytes)", string.Join(" ", rawReportDescriptor.Select(d => d.ToString("X2"))), rawReportDescriptor.Length);
            }
//            Console.WriteLine("Press any key to exit");
//            Console.ReadKey();
        }
    }
}
