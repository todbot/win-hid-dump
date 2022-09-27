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
            int vid = 0;
            int pid = 0;

            int.TryParse(GetArgument(args, "--vid"), System.Globalization.NumberStyles.HexNumber, null, out vid);
            int.TryParse(GetArgument(args, "--pid"), System.Globalization.NumberStyles.HexNumber, null, out pid);

            var list = DeviceList.Local;
            foreach (var dev in list.GetHidDevices())
            {
                if ((vid != 0 && vid != dev.VendorID) ||
                    (pid != 0 && pid != dev.ProductID))
                {
                    continue;
                }

                string manufacturer = "(unnamed manufacturer)";
                try { manufacturer = dev.GetManufacturer(); } catch { }

                string productName = "(unnamed product)";
                try { productName = dev.GetProductName(); } catch { }

                Console.Write(string.Format("{0:X4}:{1:X4}: {2} - {3}\nPATH:{4}\n",
                    dev.VendorID, dev.ProductID, manufacturer, productName, dev.DevicePath));
                byte[] rawReportDescriptor = new byte[] { };
                try
                {
                    rawReportDescriptor = dev.GetRawReportDescriptor();
                } catch (Exception e){ 
                    Console.Write("Unable to parse HID Report: {0}", e);
                }
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
