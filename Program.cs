using System;
using System.Collections.Generic;
using System.Linq;

using HidSharp;

namespace winhiddump
{
    public static class HidDeviceExtensions
    {
        public static string GetManufacturerOrDefault(this HidDevice device, string defaultValue)
        {
            try
            {
                return device.GetManufacturer();
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetProductNameOrDefault(this HidDevice device, string defaultValue)
        {
            try
            {
                return device.GetProductName();
            }
            catch
            {
                return defaultValue;
            }
        }
    }

    class Program
    {
        static string GetArgument(IEnumerable<string> args, string option) => args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();

        static void Main(string[] args)
        {
            int.TryParse(GetArgument(args, "--vid"), System.Globalization.NumberStyles.HexNumber, null, out int vid);
            int.TryParse(GetArgument(args, "--pid"), System.Globalization.NumberStyles.HexNumber, null, out int pid);

            var list = DeviceList.Local;
            foreach (var dev in list.GetHidDevices())
            {
                if ((vid != 0 && vid != dev.VendorID) ||
                    (pid != 0 && pid != dev.ProductID))
                {
                    continue;
                }

                string manufacturer = dev.GetManufacturerOrDefault("(unnamed manufacturer)");
                string productName = dev.GetProductNameOrDefault("(unnamed product)");
                try
                {
                    byte[] rawReportDescriptor = new byte[] { };
                    rawReportDescriptor = dev.GetRawReportDescriptor();
                    Console.Write(string.Format("{0:X4}:{1:X4}: {2} - {3}\nPATH:{4}\n",
                        dev.VendorID,
                        dev.ProductID,
                        manufacturer,
                        productName,
                        dev.DevicePath));
                    Console.Write("DESCRIPTOR:\n  ");
                    for (int i = 0; i < rawReportDescriptor.Length; i++)
                    {
                        Console.Write(rawReportDescriptor[i].ToString("X2") + " ");
                        Console.Write((i % 16 == 15) ? "\n  " : " ");
                    }
                    Console.WriteLine("\n  ({0} bytes)", rawReportDescriptor.Length);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to parse HID Report: {0}: {1}", e.GetType().Name, e.Message);
                }
                finally
                {
                    Console.WriteLine(new string('-', Console.WindowWidth - 1));
                }
            }
        }
    }
}
