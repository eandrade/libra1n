//using Microsoft.VisualBasic;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
////============================================================================
//// Name        : MobileDevice.vb
//// Author       : fallensn0w (Originally based on code developed by: geohot, ixtli, nightwatch, warren)
//// Copyright   : http://www.fallensn0w-devkit.blogspot.com
//// Description : A VB.NET implementation of MobileDevice.h
////============================================================================

using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.IO;
using LibUsbDotNet.Info;

class MobileDevice
{

	public enum DeviceMode : int
	{
		Test = 1,
		// also known as 0x1 in C.
		Normal = 0x1293,
		// also known as 0x1293 in C.
		Recovery = 0x1281,
		// also known as 0x1281 in C.
		DFU = 0x1222,
		// also known as 0x1222 in C.
		WTF = 0x1227
		// also known as 0x1227 in C.
	}

		// Vendor ID, aka 0x5AC in C.
	private int mVendor = 0x5ac;
	private DeviceMode mMode = DeviceMode.Recovery;

	public UsbDevice mDevice;
	private string mdPositive = "SUCCESS";

	private string mdNegative = "FAILURE";
	public MobileDevice(int mode)
	{
		mMode = (DeviceMode)mode;
	}


	public string getDeviceInfo()
	{
		if (Connect() == true) {
			UsbDeviceInfo mDeviceInfo = mDevice.Info;
			return mDeviceInfo.ToString;
			WriteLog("GetDeviceInfo - " + mdPositive);
		} else {
			WriteLog("GetDeviceInfo - FAIL");
			return false;
		}
	}
	public bool AutoBoot(bool mode)
	{
		if (SendCommand("setenv auto-boot " + mode.ToString())) {
			if (SendCommand("saveenv")) {
				if (SendCommand("reboot")) {
					WriteLog("AutoBoot - " + mdPositive);
					return true;
				}
			}
		} else {
			WriteLog("AutoBoot - FAIL");
			return false;
		}
	}
	public bool Connect()
	{
		foreach (int mode in Enum.GetValues(typeof(DeviceMode))) {
			if (!ReferenceEquals((InlineAssignHelper(ref mDevice, UsbDevice.OpenUsbDevice(new UsbDeviceFinder(mVendor, mode)))), null)) {
				WriteLog("Connect - " + mdPositive);
				return true;
			}
		}
		WriteLog("Connect - FAIL");
		return false;
	}
	public bool IsRunningMode(int currMode)
	{
		foreach (int mode in Enum.GetValues(typeof(DeviceMode))) {
			if (!ReferenceEquals((InlineAssignHelper(ref mDevice, UsbDevice.OpenUsbDevice(new UsbDeviceFinder(mVendor, currMode)))), null)) {
				WriteLog("IsRunningMode - " + mdPositive);
				return true;
			}
		}
		WriteLog("IsRunningMode - " + mdNegative);
		return false;
	}
	public void Dispose()
	{
		OuputNeeded = true;
		if (mDevice.IsOpen) {
			//        WriteLog("Dispose - CLOSING")
			if (mDevice.Close()) {
				WriteLog("Dispose - " + mdPositive);
			}
		}
	}


	public bool SendBuffer(byte[] dataBytes, short index, short length)
	{
		int size = 0;
		int packets = 0;
		int last = 0;
		int i = 0;
		size = dataBytes.Length - index;

		if (length > size) {
			WriteLog("SendBuffer - INVALID DATA");
			return false;
		}

		packets = length / 0x800;

		if ((length % 0x800) == 0) {
			packets += 1;
		}

		last = length % 0x800;

		if (last == 0) {
			last = 0x800;
		}

		int sent = 0;
		char[] response = new char[6];

		for (i = 0; i <= packets - 1; i++) {
			int tosend = (i + 1) > packets ? 0x800 : last;
			sent += sent;

			if (SendRaw(0x21, 1, 0, Convert.ToInt16(i), new byte[dataBytes[index + (i * 0x800)]], Convert.ToInt16(tosend))) {
				//wont work SendRaw return's true / false need to find a work around
				if (!SendRaw(0xa1, 3, 0, 0, Encoding.Default.GetBytes(response.ToString()), 6)) {
					// != 6 
					//cant check if its 6 so fuck knows :(
					if (response[4] == "5") {
						WriteLog("SendBuffer - Sent Chunk");
						continue;
					}
					WriteLog("SendBuffer - Invalid Status");
					return false;
				}
				WriteLog("SendBuffer - Failed To Retreive Status");
				return false;
			}
			WriteLog("SendBuffer - Fail To Send");
			return false;
		}

		WriteLog("SendBuffer - Executing Buffer");
		SendRaw(0x21, 1, Convert.ToInt16(i), 0, dataBytes, 0);
		//might work probably wont
		for (i = 6; i <= 7; i++) {
			// != 6 
			if (!(SendRaw(0x21, 1, Convert.ToInt16(i), 0, Encoding.Default.GetBytes(response), 6))) {
				// need to find a work around
				if (!string.IsNullOrEmpty(response[4])) {
					WriteLog("SendBuffer - Failed To Execute");
					return false;
				}
			}
		}

		WriteLog("SendBuffer - Transfered Buffer");
		return true;
	}
	public bool SendCommand(string command)
	{
		if (command.Length > 0x200) {
			WriteLog("SendCommand - Command Is Too Long");
			return false;
		}

		// ummm.. how come this if-statement is inverted?
		if (iSendRaw(0x40, 0, 0, 0, command, Convert.ToInt16(command.Length))) {
			WriteLog("SendCommand - " + mdNegative);
			return false;
		} else {
			WriteLog("SendCommand - " + mdPositive);
			return true;
		}

	}
	public bool SendPayload(string PayloadName)
	{
		bytes = openBinary(PayloadName);
		return SendExploit(bytes);
	}
	public bool SendExploit(byte[] dataBytes)
	{
		if (!SendBuffer(dataBytes, 0, Convert.ToInt16(dataBytes.Length))) {
			if (SendRaw(0x21, 2, 0, 0, new byte[-1 + 1], dataBytes.Length)) {
				WriteLog("SendExploit - Executed Exploit At 0x21");
				return true;
			}
		}

		WriteLog("SendExploit - Failed To Exploit At 0x21");
		return false;
	}
	public bool SendFile(string FileLocation)
	{

	}

	public bool iSendRaw(byte requestType, byte request, short value, short index, string data, short length)
	{
		return SendRaw(requestType, request, value, index, Encoding.Default.GetBytes(data), length);
	}
	public bool SendRaw(byte requestType, byte request, short value, short index, byte[] data, short length)
	{
		if (!mDevice.IsOpen) {
			WriteLog("SendRaw - Opening Connection");
			if (!Connect()) {
				WriteLog("SendRaw - Failed To Connect");
				return false;
			}
		}

		length += 1;
		// allocate null byte
		UsbSetupPacket setupPacket = new UsbSetupPacket(requestType, request, value, index, length);

		int transfered = 0;

		// the SendRAW status is fucked up, i dont want to fix it atm.
		if (mDevice.ControlTransfer(setupPacket, data, length, transfered)) {
			WriteLog("SendRaw - " + mdNegative);
			return true;
		} else {
			WriteLog("SendRaw - " + mdPositive);
			return false;
		}

	}


	private static T InlineAssignHelper<T>(ref T target, T value)
	{
		target = value;
		return value;
	}


	public bool SendRawUsb_0xA1(string command)
	{
		//  WriteLog("SendRawUsb_0xA1 - SENDNING")
		if (iSendRaw(0xa1, Strings.Len(command), 0, 0, 0, 0)) {
			WriteLog("SendRawUsb_0xA1 - " + mdPositive);
			return true;
		} else {
			WriteLog("SendRawUsb_0xA1 - " + mdNegative);
			return false;
		}
	}
	public bool SendRawUsb_0x40(string command)
	{
		//    WriteLog("SendRawUsb_0x40 - SENDNING")
		if (iSendRaw(0x40, Strings.Len(command), 0, 0, 0, 0)) {
			WriteLog("SendRawUsb_0x40 - " + mdPositive);
			return true;
		} else {
			WriteLog("SendRawUsb_0x40 - " + mdNegative);
			return false;
		}
	}
	public bool SendRawUsb_0x21(string command)
	{
		//    WriteLog("SendRawUsb_0x21 - SENDNING")
		if (iSendRaw(0x21, Strings.Len(command), 0, 0, 0, 0)) {
			WriteLog("SendRawUsb_0x21 - " + mdPositive);
			return true;
		} else {
			WriteLog("SendRawUsb_0x21 - " + mdNegative);
			return false;
		}
	}


	public bool Send_Arm7_Go()
	{
		string[] command = new string[12];
		WriteLog("Sending Arm7_Go (ipt2-2.1.1 iBSS Exploit).");

		Interaction.Command(0) = "arm7_stop";
		Interaction.Command(1) = "mw 0x9000000 0xe59f3014";
		Interaction.Command(2) = "mw 0x9000004 0xe3a02a02";
		Interaction.Command(3) = "mw 0x9000008 0xe1c320b0";
		Interaction.Command(4) = "mw 0x900000c 0xe3e02000";
		Interaction.Command(5) = "mw 0x9000010 0xe2833c9d";
		Interaction.Command(6) = "mw 0x9000014 0xe58326c0";
		Interaction.Command(7) = "mw 0x9000018 0xeafffffe";
		Interaction.Command(8) = "mw 0x900001c 0x2200f300";
		Interaction.Command(9) = "arm7_go";
		Interaction.Command(10) = "#";
		Interaction.Command(11) = "arm7_stop";


		if (SendCommand(Interaction.Command(0))) {
			if (SendCommand(Interaction.Command(1))) {
				if (SendCommand(Interaction.Command(2))) {
					if (SendCommand(Interaction.Command(3))) {
						if (SendCommand(Interaction.Command(4))) {
							if (SendCommand(Interaction.Command(5))) {
								if (SendCommand(Interaction.Command(6))) {
									if (SendCommand(Interaction.Command(7))) {
										if (SendCommand(Interaction.Command(8))) {
											if (SendCommand(Interaction.Command(9))) {
												if (SendCommand(Interaction.Command(10))) {
													if (SendCommand(Interaction.Command(11))) {
														WriteLog("Send_Arm7_Go - " + mdPositive);
														return true;
														//               <- LOL AT THAT STATEMENT
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		} else {
			WriteLog("Send_Arm7_Go - " + mdNegative);
			return false;
		}


		Console.WriteLine("Done sending Arm7_Go Exploit.");
	}


	#region " I/O Functions "


	//   Public Function SaveTextToFile(ByVal strData As String, ByVal FullPath As String, Optional ByVal ErrInfo As String = "") As Boolean
	//       Dim bAns As Boolean = False, objReader As StreamWriter : Try : objReader = New StreamWriter(FullPath) : objReader.Write(strData) : objReader.Close() : bAns = True : Catch Ex As Exception : ErrInfo = Ex.Message : End Try : Return bAns
	//   End Function
	//   Public Function GetFileContents(ByVal FullPath As String, Optional ByRef ErrInfo As String = "") As String
	//        Dim strContents As String = "" : Dim objReader As StreamReader : Try : objReader = New StreamReader(FullPath) : strContents = objReader.ReadToEnd() : objReader.Close() : Catch Ex As Exception : ErrInfo = Ex.Message : End Try : Return strContents
	//    End Function

	public void WriteLog(string Text)
	{
		// old = GetFileContents("MobileDevice.txt")
		// SaveTextToFile((old & "MobileDevice" & " @ " & TimeOfDay & " -> " & LCase(Text)) & vbCrLf, "MobileDevice.txt")

		Console.WriteLine(" MobileDevice" + " @ " + DateAndTime.TimeOfDay + " -> " + Strings.LCase(Text));

	}

	public byte[] openBinary(string BinaryFile)
	{
		FileStream input = new FileStream(BinaryFile, FileMode.Open);
		byte[] bytes = new byte[Convert.ToInt32(input.Length - 1) + 1];
		input.Read(bytes, 0, Convert.ToInt32(input.Length));
		return bytes;
	}

	#endregion



}

