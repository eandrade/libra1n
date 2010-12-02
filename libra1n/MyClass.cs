
using System;
using Alpine.Device;

namespace libra1n
{
	
	
	public class MyClass
	{
		
		static MobileDevice iDev = new MobileDevice(0x1222, MobileDevice.Verbrosity.Debug);

		
		public MyClass()
		{
			iDev.Connect();
			iDev.SendRaw(0x21, 0, 0, 0, 0, 0);
		}
	}
}
