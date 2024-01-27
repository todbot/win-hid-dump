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
}
