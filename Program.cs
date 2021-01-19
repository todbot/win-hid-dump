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

            string vidpid = GetArgument(args, "--vidpid");
//            Console.WriteLine("Got vidpid: " + vidpid);

            var list = DeviceList.Local;
            foreach (var dev in list.GetHidDevices())
            {
                Console.Write(string.Format("{0:X4}:{1:X4}: {2} - {3}\nPATH:{4}\n", 
                    dev.VendorID, dev.ProductID, dev.GetManufacturer(), dev.GetProductName(), dev.DevicePath));
//                Console.WriteLine(dev.ToString() + " @ " + dev.DevicePath);
                byte[] rawReportDescriptor = dev.GetRawReportDescriptor();
                Console.Write("DESCRIPTOR:\n  ");
                for( int i=0; i< rawReportDescriptor.Length; i++)
                {
                    Console.Write(rawReportDescriptor[i].ToString("X2") + " ");
                    Console.Write((i % 16 == 15) ? "\n  " : " ");
                }
                Console.WriteLine("\n  ({0} bytes)", rawReportDescriptor.Length);
//                Console.WriteLine("  {0} ({1} bytes)", string.Join(" ", rawReportDescriptor.Select(d => d.ToString("X2"))), rawReportDescriptor.Length);
            }
//            Console.WriteLine("Press any key to exit");
//            Console.ReadKey();
        }
    }
}
