using System;
using System.Runtime.InteropServices;

namespace ChromaSDK
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct APPINFOTYPE
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Title;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string Description;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Author_Name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Author_Contact;

		public uint SupportedDevice;

		public uint Category;
	}
}
