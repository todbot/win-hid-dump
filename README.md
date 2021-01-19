# WinHIDdump

`WinHIDdump` is a small command-line app to output the __HID Report Descriptors__ of all connected HID devices.  Think of it as sort of a Windows version of [`usbhid-dump`](https://github.com/DIGImend/usbhid-dump). It is designed to inspect HID devices for use with [`hidapi`](https://github.com/libusb/hidapi/), [`node-hid`](https://github.com/node-hid/node-hid), or similar HID libraries.  The output can be parsed directly by the [USB Descriptor Parser](https://eleccelerator.com/usbdescreqparser/).


`WinHIDDump` is based on the very nice [HIDSharp library](https://www.nuget.org/packages/HidSharp/). It does the hard work of reconstructing the HID Report Descriptor from the Windows HID library, as the actual HID Report Descriptor apparently isn't available.

## Usage:

To use `WinHIDdump`, download the zip file from the Releases page, unzip it, and run it.

When run, its output looks like the below.

```
PS C:\Users\todbot\Downloads\winhiddump> .\winhiddump.exe
WinHIDdump:
16C0:0486: (unnamed manufacturer) - Emulated Arduino Serial
PATH: \\?\hid#vid_16c0&pid_0486&mi_01#b&358fe38b&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}
DESCRIPTOR:
  06 C9 FF 09 04 A1 5C 09 75 15 00 25 FF 35 00 45
  00 65 00 55 00 75 08 95 40 81 02 09 76 95 20 91
  02 09 76 95 04 B1 02 C1 00
  (41 bytes)
16C0:0486: (unnamed manufacturer) Teensyduino RawHID
PATH: \\?\hid#vid_16c0&pid_0486&mi_00#b&1dcc5746&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}
DESCRIPTOR:
  06 AB FF 0A 00 02 A1 01 09 01 15 00 25 FF 35 00
  45 00 65 00 55 00 75 08 95 40 81 02 09 02 91 02
  C1 00
  (34 bytes)
27B8:01ED: ThingM - blink(1) mk3
PATH: \\?\hid#vid_27b8&pid_01ed#a&27977118&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}
DESCRIPTOR:
  06 AB FF 0A 00 20 A1 01 85 01 09 00 15 00 25 FF
  35 00 45 00 65 00 55 00 75 08 95 08 B2 02 01 25
  01 45 01 75 01 96 A0 01 B1 03 85 02 09 00 25 FF
  45 00 75 08 95 3C B2 02 01 C1 00
  (59 bytes)
PS C:\Users\todbot\Downloads\winhiddump>

```
This example shows a
[Teensy RawHID](https://www.pjrc.com/teensy/rawhid.html) and a [blink(1) USB LED](https://blink1.thingm.com/).  The Teensy RawHID descriptor shows a simple "RawHID" example of a single 64-byte report using no reportIDs for both Input and Output. The blink(1) descriptor shows an example that defines two reportID-based Feature reports: an 8-byte one and a 60-byte one.

## Limitations

The HID Report Descriptors displayed are reconstructed by the HIDSharp library from Windows HID data, and do not represent the exact descriptor sent by the device.

For instance, the Teensy RawHID HID Report Descriptor as sent by the device is:
```
0x06, 0xAB, 0xFF,  // Usage Page (Vendor Defined 0xFFAB)
0x0A, 0x00, 0x02,  // Usage (0x0200)
0xA1, 0x01,        // Collection (Application)
0x75, 0x08,        //   Report Size (8)
0x15, 0x00,        //   Logical Minimum (0)
0x26, 0xFF, 0x00,  //   Logical Maximum (255)
0x95, 0x40,        //   Report Count (64)
0x09, 0x01,        //   Usage (0x01)
0x81, 0x02,        //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
0x95, 0x40,        //   Report Count (64)
0x09, 0x02,        //   Usage (0x02)
0x91, 0x02,        //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
0xC0,              // End Collection
// 28 bytes
```

But the reconstructed descriptor that HIDSharp determined from Windows parsed data is:
```
0x06, 0xAB, 0xFF,  // Usage Page (Vendor Defined 0xFFAB)
0x0A, 0x00, 0x02,  // Usage (0x0200)
0xA1, 0x01,        // Collection (Application)
0x09, 0x01,        //   Usage (0x01)
0x15, 0x00,        //   Logical Minimum (0)
0x25, 0xFF,        //   Logical Maximum (-1)
0x35, 0x00,        //   Physical Minimum (0)
0x45, 0x00,        //   Physical Maximum (0)
0x65, 0x00,        //   Unit (None)
0x55, 0x00,        //   Unit Exponent (0)
0x75, 0x08,        //   Report Size (8)
0x95, 0x40,        //   Report Count (64)
0x81, 0x02,        //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
0x09, 0x02,        //   Usage (0x02)
0x91, 0x02,        //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
0xC1, 0x00,        // End Collection
// 34 bytes
```
