using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;

namespace winhiddump
{
    internal class HidDeviceDesciptorParser
    {
        private enum StackAction
        {
            push, pop, none
        }

        private static readonly Dictionary<int, string> commands = new Dictionary<int, string>()
        {
            { 0x0600FF, "Usage page (Vendor defined)" },
            { 0x0A3802, "Usage (AC Pan)" },
            { 0xA100, "Collection (Phisical)" },
            { 0xA101, "Collection (Application)" },
            { 0xA102, "Collection (Logical)" },
            { 0x0501, "Usage page (Generic Desktop)" },
            { 0x0507, "Usage page (Keyboard)" },
            { 0x0508, "Usage page (LEDs)" },
            { 0x0509, "Usage page (Button)" },
            { 0x050C, "Usage page (Consumer Devices)" },
            { 0x050D, "Usage page (Digitizers)" },
            { 0x0901, "Usage (Pointer)" },
            { 0x0902, "Usage (Mouse)" },
            { 0x0906, "Usage (Keyboard)" },
            { 0x0922, "Usage (Finger)" },
            { 0x0942, "Usage (Tip switch)" },
            { 0x0947, "Usage (Confidence)" },
            { 0x0930, "Usage (X)" },
            { 0x0931, "Usage (Y)" },
            { 0x09B5, "Usage (Scan Next Track)" },
            { 0x09B6, "Usage (Scan Previous Track)" },
            { 0x09B7, "Usage (Stop)" },
            { 0x09CD, "Usage (Play/Pause)" },
            { 0x09E2, "Usage (Mute)" },
            { 0x09E9, "Usage (Volume Up)" },
            { 0x09EA, "Usage (Volume Down)" },
            { 0x6500, "Unit (None)" },
            { 0x6511, "Unit (System: SI Linear, Length: Centimeter)" },
            { 0x8101, "Input (Cnst,Ary,Abs)" },
            { 0x8102, "Input (Data,Var,Abs)" },
            { 0x8103, "Input (Cnst,Var,Abs)" },
            { 0x8106, "Input (Data,Var,Rel)" },
            { 0x9102, "Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)" },
            { 0x9103, "Output (Const,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)" },
            { 0xC100, "End collection"},
            { 0x09, "Usage (0x{0:X2})" },
            { 0x85, "Report ID ({0})" },
            { 0x95, "Report count ({0})" },
            { 0x75, "Report size ({0})" },
            { 0x15, "Logical minimum ({0})" },
            { 0x19, "Usage minimum ({0})" },
            { 0x25, "Logical maximum ({0})" },
            { 0x26, "Logical maximum ({0})" },
            { 0x29, "Usage maximum ({0})" },
            { 0x35, "Phisical minimum ({0})" },
            { 0x45, "Phisical maximum ({0})" }, // two bytes version
            { 0x46, "Phisical maximum ({0})" }, // three bytes version
            { 0x55, "Unit Exponent ({0})" },
            { 0xc0, "End collection" },
        };

        private static readonly HashSet<byte> threeBytesDesciptors = new HashSet<byte>() { 0x06, 0x0A, 0x26, 0x46 };

        private static bool isThreeBytesDescriptor(byte command)
        {
            return threeBytesDesciptors.Contains(command);
        }

        private static StackAction toStackAction(byte command)
        {
            if (command == 0xA1)
            {
                return StackAction.push;
            }
            else if (command == 0xA0 || command == 0xC1)
            {
                return StackAction.pop;
            }
            else
            {
                return StackAction.none;
            }
        }

        private static bool isOneByteDescriptor(byte command)
        {
            return command == 0xc0;
        }

        private static string explainCommand(List<byte> bytes)
        {
            int lookupValue;
            string commandDesciption;

            if (isThreeBytesDescriptor(bytes[0]))
            {
                int descriptorValue = (bytes[2] << 8) + bytes[1];
                if (!commands.TryGetValue(bytes[0], out commandDesciption))
                {
                    lookupValue = (bytes[0] << 16) + (bytes[1] << 8) + bytes[2];
                    commands.TryGetValue(lookupValue, out commandDesciption);
                    return commandDesciption ?? "";
                }
                return string.Format(commandDesciption ?? "", descriptorValue);

            }

            lookupValue = (bytes[0] << 8) + bytes[1];
            if (!commands.TryGetValue(lookupValue, out commandDesciption))
            {
                if (commands.TryGetValue(bytes[0], out commandDesciption))
                {
                    commandDesciption = string.Format(commandDesciption, bytes[1]);
                }
            }
            return commandDesciption;
        }

        private static IEnumerable<(byte[], string, StackAction)> sliceDescriptorCommands(byte[] deviceDesciptor)
        {
            if (deviceDesciptor == null)
            {
                throw new ArgumentException("Incorrect device descriptor");
            }

            for (var i = 0; i < deviceDesciptor.Length;)
            {
                List<byte> desciptor = new List<byte>();
                desciptor.Add(deviceDesciptor[i++]);

                if (!isOneByteDescriptor(desciptor.First()))
                {
                    desciptor.Add(deviceDesciptor[i++]);
                }

                if (isThreeBytesDescriptor(desciptor.First()))
                {
                    desciptor.Add(deviceDesciptor[i++]);
                }

                yield return (desciptor.ToArray(), explainCommand(desciptor), toStackAction(desciptor.First()));
            }
        }

        public static void parseAndPrint(byte[] deviceDesciptor)
        {
            int stackShift = 0;
            foreach (var command in sliceDescriptorCommands(deviceDesciptor))
            {
                if (command.Item3 == StackAction.push)
                {
                    stackShift++;
                }
                else if (command.Item3 == StackAction.pop)
                {
                    stackShift--;
                }
                var bytes = string.Join(", ", command.Item1.Select(x => string.Format("0x{0:X2}", x)));
                Console.WriteLine("{0,-19} # {1}{2}", bytes, new string(' ', stackShift), command.Item2);
            }
        }
    }
}
