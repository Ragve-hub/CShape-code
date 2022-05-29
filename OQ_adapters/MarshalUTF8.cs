using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenQuant.Finam
{
	public static class MarshalUTF8
	{
		private static UTF8Encoding utf8;

		static MarshalUTF8()
		{
			utf8 = new UTF8Encoding();
		}

		public static IntPtr StringToHGlobalUTF8(string data)
		{
			byte[] bytes = utf8.GetBytes(data);
			int cb = Marshal.SizeOf((object)bytes[0]) * bytes.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(cb);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			return intPtr;
		}

		public static string PtrToStringUTF8(IntPtr pData)
		{
			string text = Marshal.PtrToStringAnsi(pData);
			int length = text.Length;
			byte[] array = new byte[length];
			Marshal.Copy(pData, array, 0, length);
			return utf8.GetString(array);
		}
	}
}
