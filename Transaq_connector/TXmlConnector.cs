using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenQuant.Finam
{
	public class TXmlConnector : ITransaqConnector
	{
		public delegate void CallBack(IntPtr pData);

		public delegate void CallBackEx(IntPtr pData, IntPtr userData);

		private const string EX_SETTING_CALLBACK = "Could not establish a callback function";

		private static volatile TXmlConnector instance;

		private static object syncRoot = new object();

		private ConcurrentQueue<string> queue;

		private bool flag;

		private Thread thread;

		public CallBack myDelegate;

		public CallBackEx myDelegateEx;

		public static TXmlConnector Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new TXmlConnector();
						}
					}
				}
				return instance;
			}
		}

		public event EventHandler<NewDataEventArgs> NewData;

		public TXmlConnector()
		{
			queue = new ConcurrentQueue<string>();
			flag = true;
			thread = new Thread(Dequeue);
			thread.IsBackground = true;
			thread.Name = "TXmlDequeue";
			thread.Start();
			myDelegate = MyCallBack;
			myDelegateEx = MyCallBackEx;
			if (Environment.Is64BitProcess)
			{
				if (!SetCallback64(myDelegate))
				{
					throw new Exception("Could not establish a callback function");
				}
				if (!SetCallbackEx64(myDelegateEx, IntPtr.Zero))
				{
					throw new Exception("Could not establish a callback function");
				}
			}
			else
			{
				if (!SetCallback(myDelegate))
				{
					throw new Exception("Could not establish a callback function");
				}
				if (!SetCallbackEx(myDelegateEx, IntPtr.Zero))
				{
					throw new Exception("Could not establish a callback function");
				}
			}
			instance = this;
		}

		public string SendCommand(string command)
		{
			IntPtr intPtr = MarshalUTF8.StringToHGlobalUTF8(command);
			IntPtr intPtr2 = default(IntPtr);
			intPtr2 = ((!Environment.Is64BitProcess) ? SendCommand(intPtr) : SendCommand64(intPtr));
			if (intPtr2 != IntPtr.Zero)
			{
				string result = MarshalUTF8.PtrToStringUTF8(intPtr2);
				Marshal.FreeHGlobal(intPtr);
				if (Environment.Is64BitProcess)
				{
					FreeMemory64(intPtr2);
				}
				else
				{
					FreeMemory(intPtr2);
				}
				return result;
			}
			return string.Empty;
		}

		public string ConnectorInitialize(string Path, short LogLevel)
		{
			IntPtr intPtr = MarshalUTF8.StringToHGlobalUTF8(Path);
			IntPtr intPtr2 = default(IntPtr);
			intPtr2 = ((!Environment.Is64BitProcess) ? Initialize(intPtr, LogLevel) : Initialize64(intPtr, LogLevel));
			if (intPtr2 != IntPtr.Zero)
			{
				string result = MarshalUTF8.PtrToStringUTF8(intPtr2);
				Marshal.FreeHGlobal(intPtr);
				if (Environment.Is64BitProcess)
				{
					FreeMemory64(intPtr2);
				}
				else
				{
					FreeMemory(intPtr2);
				}
				return result;
			}
			Marshal.FreeHGlobal(intPtr);
			return string.Empty;
		}

		public string ConnectorUnInitialize()
		{
			IntPtr intPtr = default(IntPtr);
			intPtr = ((!Environment.Is64BitProcess) ? UnInitialize() : UnInitialize64());
			if (intPtr != IntPtr.Zero)
			{
				string result = MarshalUTF8.PtrToStringUTF8(intPtr);
				if (Environment.Is64BitProcess)
				{
					FreeMemory64(intPtr);
				}
				else
				{
					FreeMemory(intPtr);
				}
				return result;
			}
			return string.Empty;
		}

		private void MyCallBack(IntPtr pData)
		{
			string item = MarshalUTF8.PtrToStringUTF8(pData);
			if (Environment.Is64BitProcess)
			{
				FreeMemory64(pData);
			}
			else
			{
				FreeMemory(pData);
			}
			queue.Enqueue(item);
		}

		private void MyCallBackEx(IntPtr pData, IntPtr userData)
		{
			string item = MarshalUTF8.PtrToStringUTF8(pData);
			if (Environment.Is64BitProcess)
			{
				FreeMemory64(pData);
			}
			else
			{
				FreeMemory(pData);
			}
			queue.Enqueue(item);
		}

		public void Stop()
		{
			flag = false;
		}

		private void Dequeue()
		{
			string result = string.Empty;
			while (flag)
			{
				while (!queue.IsEmpty)
				{
					if (queue.TryDequeue(out result))
					{
						RaiseNewDataEvent(result);
					}
					if (!flag)
					{
						break;
					}
				}
				Thread.Sleep(1);
			}
		}

		private void RaiseNewDataEvent(string data)
		{
			this.NewData?.Invoke(new object(), new NewDataEventArgs(data));
		}

		[DllImport("txmlconnector.dll")]
		private static extern bool SetCallback(CallBack pCallback);

		[DllImport("txmlconnector.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern bool SetCallbackEx(CallBackEx pCallback, IntPtr userData);

		[DllImport("txmlconnector.dll")]
		private static extern IntPtr SendCommand(IntPtr pData);

		[DllImport("txmlconnector.dll")]
		private static extern bool FreeMemory(IntPtr pData);

		[DllImport("txmlconnector.dll")]
		private static extern IntPtr Initialize(IntPtr pPath, int logLevel);

		[DllImport("txmlconnector.dll")]
		private static extern IntPtr UnInitialize();

		[DllImport("txmlconnector64.dll", EntryPoint = "SetCallback")]
		private static extern bool SetCallback64(CallBack pCallback);

		[DllImport("txmlconnector64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCallbackEx")]
		private static extern bool SetCallbackEx64(CallBackEx pCallback, IntPtr userData);

		[DllImport("txmlconnector64.dll", EntryPoint = "SendCommand")]
		private static extern IntPtr SendCommand64(IntPtr pData);

		[DllImport("txmlconnector64.dll", EntryPoint = "FreeMemory")]
		private static extern bool FreeMemory64(IntPtr pData);

		[DllImport("txmlconnector64.dll", EntryPoint = "Initialize")]
		private static extern IntPtr Initialize64(IntPtr pPath, int logLevel);

		[DllImport("txmlconnector64.dll", EntryPoint = "UnInitialize")]
		private static extern IntPtr UnInitialize64();
	}
}
