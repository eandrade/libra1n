
using System;
using Alpine.Device;

namespace openra1n
{
		public class steaks4uce
	{
		static MobileDevice iDev = new MobileDevice(0x1222, MobileDevice.Verbrosity.Debug);

		int i, ret;
		byte data = 0x800;
		
		public steaks4uce()
		{
			iDev.Connect();
			Console.Write("Reseting usb counters.\n");
			iDev.SendRaw(0x21, 4, 0, 0, 0, 1000);
			Console.Write("Padding to 0x23800...\n");
			if (data < 0x800)
			{
				data = 0x800;
			}
			for(i = 0; i < 0x23800 ; i+=0x800)  
			{
			ret = iDev.SendRaw(0x21, 1, 0, 0, 0x800, 0x800, 1000);
				if (ret < 0) {
			error("Failed to push data to the device.\n");
			return -1;
		}
			}

		}
	}
}
