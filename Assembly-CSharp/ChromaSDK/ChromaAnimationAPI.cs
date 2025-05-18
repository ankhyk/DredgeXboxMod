using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ChromaSDK.Stream;
using UnityEngine;

namespace ChromaSDK
{
	public class ChromaAnimationAPI
	{
		public static bool IsChromaSDKAvailable()
		{
			try
			{
				string text = "C:\\Program Files\\Razer Chroma SDK\\bin\\RzChromaSDK64.dll";
				if (!new FileInfo(text).Exists)
				{
					return false;
				}
				string[] array = FileVersionInfo.GetVersionInfo(text).ProductVersion.Split(".".ToCharArray());
				if (array.Length < 3)
				{
					return false;
				}
				if (array.Length < 3)
				{
					return false;
				}
				int num;
				if (!int.TryParse(array[0], out num))
				{
					return false;
				}
				int num2;
				if (!int.TryParse(array[1], out num2))
				{
					return false;
				}
				int num3;
				if (!int.TryParse(array[2], out num3))
				{
					return false;
				}
				if (num < 3)
				{
					return false;
				}
				if (num == 3 && num2 < 20)
				{
					return false;
				}
				if (num == 3 && num2 == 20 && num3 < 2)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				global::UnityEngine.Debug.LogError(string.Format("The ChromaSDK is not available! Exception={0}", ex));
			}
			return false;
		}

		public static int GetKeyboardRazerKey(KeyCode keyCode)
		{
			if (ChromaAnimationAPI._sKeyMapping == null)
			{
				ChromaAnimationAPI._sKeyMapping = new Dictionary<KeyCode, int>();
				ChromaAnimationAPI._sKeyMapping[KeyCode.Backspace] = 270;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Tab] = 513;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Return] = 782;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Pause] = 17;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Escape] = 1;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Space] = 1287;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Exclaim] = 258;
				ChromaAnimationAPI._sKeyMapping[KeyCode.DoubleQuote] = 780;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Hash] = 260;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Dollar] = 261;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Ampersand] = 264;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Quote] = 780;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftParen] = 266;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightParen] = 267;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Asterisk] = 265;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Plus] = 269;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Comma] = 1034;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Minus] = 268;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Period] = 1035;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Slash] = 1036;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha0] = 267;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha1] = 258;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha2] = 259;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha3] = 260;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha4] = 261;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha5] = 262;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha6] = 263;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha7] = 264;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha8] = 265;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Alpha9] = 266;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Colon] = 779;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Semicolon] = 779;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Less] = 1034;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Equals] = 269;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Greater] = 1035;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Question] = 1036;
				ChromaAnimationAPI._sKeyMapping[KeyCode.At] = 259;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftBracket] = 524;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Backslash] = 526;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightBracket] = 525;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Caret] = 263;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Underscore] = 268;
				ChromaAnimationAPI._sKeyMapping[KeyCode.BackQuote] = 257;
				ChromaAnimationAPI._sKeyMapping[KeyCode.A] = 770;
				ChromaAnimationAPI._sKeyMapping[KeyCode.B] = 1031;
				ChromaAnimationAPI._sKeyMapping[KeyCode.C] = 1029;
				ChromaAnimationAPI._sKeyMapping[KeyCode.D] = 772;
				ChromaAnimationAPI._sKeyMapping[KeyCode.E] = 516;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F] = 773;
				ChromaAnimationAPI._sKeyMapping[KeyCode.G] = 774;
				ChromaAnimationAPI._sKeyMapping[KeyCode.H] = 775;
				ChromaAnimationAPI._sKeyMapping[KeyCode.I] = 521;
				ChromaAnimationAPI._sKeyMapping[KeyCode.J] = 776;
				ChromaAnimationAPI._sKeyMapping[KeyCode.K] = 777;
				ChromaAnimationAPI._sKeyMapping[KeyCode.L] = 778;
				ChromaAnimationAPI._sKeyMapping[KeyCode.M] = 1033;
				ChromaAnimationAPI._sKeyMapping[KeyCode.N] = 1032;
				ChromaAnimationAPI._sKeyMapping[KeyCode.O] = 522;
				ChromaAnimationAPI._sKeyMapping[KeyCode.P] = 523;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Q] = 514;
				ChromaAnimationAPI._sKeyMapping[KeyCode.R] = 517;
				ChromaAnimationAPI._sKeyMapping[KeyCode.S] = 771;
				ChromaAnimationAPI._sKeyMapping[KeyCode.T] = 518;
				ChromaAnimationAPI._sKeyMapping[KeyCode.U] = 520;
				ChromaAnimationAPI._sKeyMapping[KeyCode.V] = 1030;
				ChromaAnimationAPI._sKeyMapping[KeyCode.W] = 515;
				ChromaAnimationAPI._sKeyMapping[KeyCode.X] = 1028;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Y] = 519;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Z] = 1027;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Delete] = 527;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad0] = 1299;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad1] = 1042;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad2] = 1043;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad3] = 1044;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad4] = 786;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad5] = 787;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad6] = 788;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad7] = 530;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad8] = 531;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Keypad9] = 532;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadPeriod] = 1300;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadDivide] = 275;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadMultiply] = 276;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadMinus] = 277;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadPlus] = 533;
				ChromaAnimationAPI._sKeyMapping[KeyCode.KeypadEnter] = 1045;
				ChromaAnimationAPI._sKeyMapping[KeyCode.UpArrow] = 1040;
				ChromaAnimationAPI._sKeyMapping[KeyCode.DownArrow] = 1296;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightArrow] = 1297;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftArrow] = 1295;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Insert] = 271;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Home] = 272;
				ChromaAnimationAPI._sKeyMapping[KeyCode.End] = 528;
				ChromaAnimationAPI._sKeyMapping[KeyCode.PageUp] = 273;
				ChromaAnimationAPI._sKeyMapping[KeyCode.PageDown] = 529;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F1] = 3;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F2] = 4;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F3] = 5;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F4] = 6;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F5] = 7;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F6] = 8;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F7] = 9;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F8] = 10;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F9] = 11;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F10] = 12;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F11] = 13;
				ChromaAnimationAPI._sKeyMapping[KeyCode.F12] = 14;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Numlock] = 274;
				ChromaAnimationAPI._sKeyMapping[KeyCode.CapsLock] = 769;
				ChromaAnimationAPI._sKeyMapping[KeyCode.ScrollLock] = 16;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightShift] = 1038;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftShift] = 1025;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightControl] = 1294;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftControl] = 1281;
				ChromaAnimationAPI._sKeyMapping[KeyCode.RightAlt] = 1291;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftAlt] = 1283;
				ChromaAnimationAPI._sKeyMapping[KeyCode.LeftWindows] = 1282;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Print] = 15;
				ChromaAnimationAPI._sKeyMapping[KeyCode.SysReq] = 15;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Break] = 17;
				ChromaAnimationAPI._sKeyMapping[KeyCode.Menu] = 1293;
			}
			if (ChromaAnimationAPI._sKeyMapping.ContainsKey(keyCode))
			{
				return ChromaAnimationAPI._sKeyMapping[keyCode];
			}
			return 65535;
		}

		private static IntPtr GetPathIntPtr(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return IntPtr.Zero;
			}
			FileInfo fileInfo = new FileInfo(path);
			byte[] bytes = Encoding.ASCII.GetBytes(fileInfo.FullName + "\0");
			IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			return intPtr;
		}

		private static IntPtr GetAsciiIntPtr(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return IntPtr.Zero;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(str + "\0");
			IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			return intPtr;
		}

		private static IntPtr GetUnicodeIntPtr(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return IntPtr.Zero;
			}
			byte[] bytes = Encoding.Unicode.GetBytes(str + "\0");
			IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			return intPtr;
		}

		private static void FreeIntPtr(IntPtr lpData)
		{
			if (lpData != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(lpData);
			}
		}

		public static int UninitAPI()
		{
			ChromaAnimationAPI.UnloadLibrarySDK();
			ChromaAnimationAPI.UnloadLibraryStreamingPlugin();
			return 0;
		}

		public static string GetStreamingPath(string animation)
		{
			return string.Format("{0}/{1}", ChromaAnimationAPI._sStreamingAssetPath, animation);
		}

		public static int AddColor(int color1, int color2)
		{
			return ChromaAnimationAPI.PluginAddColor(color1, color2);
		}

		public static int AddFrame(int animationId, float duration, int[] colors, int length)
		{
			return ChromaAnimationAPI.PluginAddFrame(animationId, duration, colors, length);
		}

		public static void AddNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginAddNonZeroAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void AddNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void AddNonZeroAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AddNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void AddNonZeroAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AddNonZeroAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void AddNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginAddNonZeroAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void AddNonZeroAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AddNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void AddNonZeroTargetAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroTargetAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AddNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void AddNonZeroTargetAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroTargetAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AddNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void AddNonZeroTargetAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AddNonZeroTargetAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAddNonZeroTargetAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void AppendAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginAppendAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void AppendAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginAppendAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double AppendAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginAppendAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void ClearAll()
		{
			ChromaAnimationAPI.PluginClearAll();
		}

		public static void ClearAnimationType(int deviceType, int device)
		{
			ChromaAnimationAPI.PluginClearAnimationType(deviceType, device);
		}

		public static void CloseAll()
		{
			ChromaAnimationAPI.PluginCloseAll();
		}

		public static int CloseAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginCloseAnimation(animationId);
		}

		public static double CloseAnimationD(double animationId)
		{
			return ChromaAnimationAPI.PluginCloseAnimationD(animationId);
		}

		public static void CloseAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginCloseAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double CloseAnimationNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginCloseAnimationNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CloseComposite(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			ChromaAnimationAPI.PluginCloseComposite(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double CloseCompositeD(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			double num = ChromaAnimationAPI.PluginCloseCompositeD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CopyAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginCopyAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void CopyAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static int CopyAnimation(int sourceAnimationId, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			int num = ChromaAnimationAPI.PluginCopyAnimation(sourceAnimationId, pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CopyAnimationName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyAnimationName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyAnimationNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyAnimationNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyBlueChannelAllFrames(int animationId, float redIntensity, float greenIntensity)
		{
			ChromaAnimationAPI.PluginCopyBlueChannelAllFrames(animationId, redIntensity, greenIntensity);
		}

		public static void CopyBlueChannelAllFramesName(string path, float redIntensity, float greenIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginCopyBlueChannelAllFramesName(pathIntPtr, redIntensity, greenIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double CopyBlueChannelAllFramesNameD(string path, double redIntensity, double greenIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginCopyBlueChannelAllFramesNameD(pathIntPtr, redIntensity, greenIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CopyGreenChannelAllFrames(int animationId, float redIntensity, float blueIntensity)
		{
			ChromaAnimationAPI.PluginCopyGreenChannelAllFrames(animationId, redIntensity, blueIntensity);
		}

		public static void CopyGreenChannelAllFramesName(string path, float redIntensity, float blueIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginCopyGreenChannelAllFramesName(pathIntPtr, redIntensity, blueIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double CopyGreenChannelAllFramesNameD(string path, double redIntensity, double blueIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginCopyGreenChannelAllFramesNameD(pathIntPtr, redIntensity, blueIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CopyKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey)
		{
			ChromaAnimationAPI.PluginCopyKeyColor(sourceAnimationId, targetAnimationId, frameId, rzkey);
		}

		public static void CopyKeyColorAllFrames(int sourceAnimationId, int targetAnimationId, int rzkey)
		{
			ChromaAnimationAPI.PluginCopyKeyColorAllFrames(sourceAnimationId, targetAnimationId, rzkey);
		}

		public static void CopyKeyColorAllFramesName(string sourceAnimation, string targetAnimation, int rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeyColorAllFramesName(pathIntPtr, pathIntPtr2, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyKeyColorAllFramesNameD(string sourceAnimation, string targetAnimation, double rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyKeyColorAllFramesNameD(pathIntPtr, pathIntPtr2, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyKeyColorAllFramesOffset(int sourceAnimationId, int targetAnimationId, int rzkey, int offset)
		{
			ChromaAnimationAPI.PluginCopyKeyColorAllFramesOffset(sourceAnimationId, targetAnimationId, rzkey, offset);
		}

		public static void CopyKeyColorAllFramesOffsetName(string sourceAnimation, string targetAnimation, int rzkey, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeyColorAllFramesOffsetName(pathIntPtr, pathIntPtr2, rzkey, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyKeyColorAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double rzkey, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyKeyColorAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, rzkey, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyKeyColorName(string sourceAnimation, string targetAnimation, int frameId, int rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeyColorName(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyKeyColorNameD(string sourceAnimation, string targetAnimation, double frameId, double rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyKeyColorNameD(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyKeysColor(int sourceAnimationId, int targetAnimationId, int frameId, int[] keys, int size)
		{
			ChromaAnimationAPI.PluginCopyKeysColor(sourceAnimationId, targetAnimationId, frameId, keys, size);
		}

		public static void CopyKeysColorAllFrames(int sourceAnimationId, int targetAnimationId, int[] keys, int size)
		{
			ChromaAnimationAPI.PluginCopyKeysColorAllFrames(sourceAnimationId, targetAnimationId, keys, size);
		}

		public static void CopyKeysColorAllFramesName(string sourceAnimation, string targetAnimation, int[] keys, int size)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeysColorAllFramesName(pathIntPtr, pathIntPtr2, keys, size);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void CopyKeysColorName(string sourceAnimation, string targetAnimation, int frameId, int[] keys, int size)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeysColorName(pathIntPtr, pathIntPtr2, frameId, keys, size);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void CopyKeysColorOffset(int sourceAnimationId, int targetAnimationId, int sourceFrameId, int targetFrameId, int[] keys, int size)
		{
			ChromaAnimationAPI.PluginCopyKeysColorOffset(sourceAnimationId, targetAnimationId, sourceFrameId, targetFrameId, keys, size);
		}

		public static void CopyKeysColorOffsetName(string sourceAnimation, string targetAnimation, int sourceFrameId, int targetFrameId, int[] keys, int size)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyKeysColorOffsetName(pathIntPtr, pathIntPtr2, sourceFrameId, targetFrameId, keys, size);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void CopyNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginCopyNonZeroAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void CopyNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void CopyNonZeroAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void CopyNonZeroAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroAllKeysNameD(string sourceAnimation, string targetAnimation, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroAllKeysNameD(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void CopyNonZeroAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey)
		{
			ChromaAnimationAPI.PluginCopyNonZeroKeyColor(sourceAnimationId, targetAnimationId, frameId, rzkey);
		}

		public static void CopyNonZeroKeyColorName(string sourceAnimation, string targetAnimation, int frameId, int rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroKeyColorName(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroKeyColorNameD(string sourceAnimation, string targetAnimation, double frameId, double rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroKeyColorNameD(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroTargetAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void CopyNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void CopyNonZeroTargetAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroTargetAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void CopyNonZeroTargetAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroTargetAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroTargetAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroTargetAllKeysNameD(string sourceAnimation, string targetAnimation, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysNameD(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void CopyNonZeroTargetAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroTargetAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroTargetAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyNonZeroTargetZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginCopyNonZeroTargetZeroAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void CopyNonZeroTargetZeroAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyNonZeroTargetZeroAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyNonZeroTargetZeroAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyNonZeroTargetZeroAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyRedChannelAllFrames(int animationId, float greenIntensity, float blueIntensity)
		{
			ChromaAnimationAPI.PluginCopyRedChannelAllFrames(animationId, greenIntensity, blueIntensity);
		}

		public static void CopyRedChannelAllFramesName(string path, float greenIntensity, float blueIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginCopyRedChannelAllFramesName(pathIntPtr, greenIntensity, blueIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double CopyRedChannelAllFramesNameD(string path, double greenIntensity, double blueIntensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginCopyRedChannelAllFramesNameD(pathIntPtr, greenIntensity, blueIntensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void CopyZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginCopyZeroAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void CopyZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginCopyZeroAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void CopyZeroAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyZeroAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyZeroAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginCopyZeroAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void CopyZeroAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyZeroAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyZeroAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyZeroAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void CopyZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginCopyZeroAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void CopyZeroAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void CopyZeroKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey)
		{
			ChromaAnimationAPI.PluginCopyZeroKeyColor(sourceAnimationId, targetAnimationId, frameId, rzkey);
		}

		public static void CopyZeroKeyColorName(string sourceAnimation, string targetAnimation, int frameId, int rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroKeyColorName(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyZeroKeyColorNameD(string sourceAnimation, string targetAnimation, double frameId, double rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyZeroKeyColorNameD(pathIntPtr, pathIntPtr2, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyZeroTargetAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginCopyZeroTargetAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void CopyZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginCopyZeroTargetAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void CopyZeroTargetAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroTargetAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double CopyZeroTargetAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginCopyZeroTargetAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void CopyZeroTargetAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginCopyZeroTargetAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static int CoreCreateChromaLinkEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateChromaLinkEffect(effect, pParam, out pEffectId);
		}

		public static int CoreCreateEffect(Guid deviceId, EFFECT_TYPE effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateEffect(deviceId, (int)effect, pParam, out pEffectId);
		}

		public static int CoreCreateHeadsetEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateHeadsetEffect(effect, pParam, out pEffectId);
		}

		public static int CoreCreateKeyboardEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateKeyboardEffect(effect, pParam, out pEffectId);
		}

		public static int CoreCreateKeypadEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateKeypadEffect(effect, pParam, out pEffectId);
		}

		public static int CoreCreateMouseEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateMouseEffect(effect, pParam, out pEffectId);
		}

		public static int CoreCreateMousepadEffect(int effect, IntPtr pParam, out Guid pEffectId)
		{
			return ChromaAnimationAPI.PluginCoreCreateMousepadEffect(effect, pParam, out pEffectId);
		}

		public static int CoreDeleteEffect(Guid effectId)
		{
			return ChromaAnimationAPI.PluginCoreDeleteEffect(effectId);
		}

		public static int CoreInit()
		{
			return ChromaAnimationAPI.PluginCoreInit();
		}

		public static int CoreInitSDK(ref APPINFOTYPE appInfo)
		{
			return ChromaAnimationAPI.PluginCoreInitSDK(ref appInfo);
		}

		public static int CoreQueryDevice(Guid deviceId, out DEVICE_INFO_TYPE deviceInfo)
		{
			return ChromaAnimationAPI.PluginCoreQueryDevice(deviceId, out deviceInfo);
		}

		public static int CoreSetEffect(Guid effectId)
		{
			return ChromaAnimationAPI.PluginCoreSetEffect(effectId);
		}

		public static bool CoreStreamBroadcast(string streamId, string streamKey)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(streamId);
			IntPtr asciiIntPtr2 = ChromaAnimationAPI.GetAsciiIntPtr(streamKey);
			bool flag = ChromaAnimationAPI.PluginCoreStreamBroadcast(asciiIntPtr, asciiIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr2);
			return flag;
		}

		public static bool CoreStreamBroadcastEnd()
		{
			return ChromaAnimationAPI.PluginCoreStreamBroadcastEnd();
		}

		public static void CoreStreamGetAuthShortcode(ref string shortcode, out byte length, string platform, string title)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(shortcode);
			IntPtr unicodeIntPtr = ChromaAnimationAPI.GetUnicodeIntPtr(platform);
			IntPtr unicodeIntPtr2 = ChromaAnimationAPI.GetUnicodeIntPtr(title);
			ChromaAnimationAPI.PluginCoreStreamGetAuthShortcode(asciiIntPtr, out length, unicodeIntPtr, unicodeIntPtr2);
			if (asciiIntPtr != IntPtr.Zero)
			{
				shortcode = Marshal.PtrToStringAnsi(asciiIntPtr);
			}
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			ChromaAnimationAPI.FreeIntPtr(unicodeIntPtr);
			ChromaAnimationAPI.FreeIntPtr(unicodeIntPtr2);
		}

		public static bool CoreStreamGetFocus(ref string focus, out byte length)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(focus);
			bool flag = ChromaAnimationAPI.PluginCoreStreamGetFocus(asciiIntPtr, out length);
			if (asciiIntPtr != IntPtr.Zero)
			{
				focus = Marshal.PtrToStringAnsi(asciiIntPtr);
			}
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			return flag;
		}

		public static void CoreStreamGetId(string shortcode, ref string streamId, out byte length)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(shortcode);
			IntPtr asciiIntPtr2 = ChromaAnimationAPI.GetAsciiIntPtr(streamId);
			ChromaAnimationAPI.PluginCoreStreamGetId(asciiIntPtr, asciiIntPtr2, out length);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			if (asciiIntPtr2 != IntPtr.Zero)
			{
				streamId = Marshal.PtrToStringAnsi(asciiIntPtr2);
			}
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr2);
		}

		public static void CoreStreamGetKey(string shortcode, ref string streamKey, out byte length)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(shortcode);
			IntPtr asciiIntPtr2 = ChromaAnimationAPI.GetAsciiIntPtr(streamKey);
			ChromaAnimationAPI.PluginCoreStreamGetKey(asciiIntPtr, asciiIntPtr2, out length);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			if (asciiIntPtr2 != IntPtr.Zero)
			{
				streamKey = Marshal.PtrToStringAnsi(asciiIntPtr2);
			}
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr2);
		}

		public static StreamStatusType CoreStreamGetStatus()
		{
			return ChromaAnimationAPI.PluginCoreStreamGetStatus();
		}

		public static string CoreStreamGetStatusString(StreamStatusType status)
		{
			return Marshal.PtrToStringAnsi(ChromaAnimationAPI.PluginCoreStreamGetStatusString(status));
		}

		public static bool CoreStreamReleaseShortcode(string shortcode)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(shortcode);
			bool flag = ChromaAnimationAPI.PluginCoreStreamReleaseShortcode(asciiIntPtr);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			return flag;
		}

		public static bool CoreStreamSetFocus(string focus)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(focus);
			bool flag = ChromaAnimationAPI.PluginCoreStreamSetFocus(asciiIntPtr);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			return flag;
		}

		public static bool CoreStreamSupportsStreaming()
		{
			return ChromaAnimationAPI.PluginCoreStreamSupportsStreaming();
		}

		public static bool CoreStreamWatch(string streamId, ulong timestamp)
		{
			IntPtr asciiIntPtr = ChromaAnimationAPI.GetAsciiIntPtr(streamId);
			bool flag = ChromaAnimationAPI.PluginCoreStreamWatch(asciiIntPtr, timestamp);
			ChromaAnimationAPI.FreeIntPtr(asciiIntPtr);
			return flag;
		}

		public static bool CoreStreamWatchEnd()
		{
			return ChromaAnimationAPI.PluginCoreStreamWatchEnd();
		}

		public static int CoreUnInit()
		{
			return ChromaAnimationAPI.PluginCoreUnInit();
		}

		public static int CreateAnimation(string path, int deviceType, int device)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginCreateAnimation(pathIntPtr, deviceType, device);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int CreateAnimationInMemory(int deviceType, int device)
		{
			return ChromaAnimationAPI.PluginCreateAnimationInMemory(deviceType, device);
		}

		public static int CreateEffect(Guid deviceId, EFFECT_TYPE effect, int[] colors, int size, out FChromaSDKGuid effectId)
		{
			return ChromaAnimationAPI.PluginCreateEffect(deviceId, (int)effect, colors, size, out effectId);
		}

		public static int DeleteEffect(Guid effectId)
		{
			return ChromaAnimationAPI.PluginDeleteEffect(effectId);
		}

		public static void DuplicateFirstFrame(int animationId, int frameCount)
		{
			ChromaAnimationAPI.PluginDuplicateFirstFrame(animationId, frameCount);
		}

		public static void DuplicateFirstFrameName(string path, int frameCount)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginDuplicateFirstFrameName(pathIntPtr, frameCount);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double DuplicateFirstFrameNameD(string path, double frameCount)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginDuplicateFirstFrameNameD(pathIntPtr, frameCount);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void DuplicateFrames(int animationId)
		{
			ChromaAnimationAPI.PluginDuplicateFrames(animationId);
		}

		public static void DuplicateFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginDuplicateFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double DuplicateFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginDuplicateFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void DuplicateMirrorFrames(int animationId)
		{
			ChromaAnimationAPI.PluginDuplicateMirrorFrames(animationId);
		}

		public static void DuplicateMirrorFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginDuplicateMirrorFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double DuplicateMirrorFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginDuplicateMirrorFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FadeEndFrames(int animationId, int fade)
		{
			ChromaAnimationAPI.PluginFadeEndFrames(animationId, fade);
		}

		public static void FadeEndFramesName(string path, int fade)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFadeEndFramesName(pathIntPtr, fade);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FadeEndFramesNameD(string path, double fade)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFadeEndFramesNameD(pathIntPtr, fade);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FadeStartFrames(int animationId, int fade)
		{
			ChromaAnimationAPI.PluginFadeStartFrames(animationId, fade);
		}

		public static void FadeStartFramesName(string path, int fade)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFadeStartFramesName(pathIntPtr, fade);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FadeStartFramesNameD(string path, double fade)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFadeStartFramesNameD(pathIntPtr, fade);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillColor(int animationId, int frameId, int color)
		{
			ChromaAnimationAPI.PluginFillColor(animationId, frameId, color);
		}

		public static void FillColorAllFrames(int animationId, int color)
		{
			ChromaAnimationAPI.PluginFillColorAllFrames(animationId, color);
		}

		public static void FillColorAllFramesName(string path, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillColorAllFramesName(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillColorAllFramesNameD(string path, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillColorAllFramesNameD(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillColorAllFramesRGB(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillColorAllFramesRGB(animationId, red, green, blue);
		}

		public static void FillColorAllFramesRGBName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillColorAllFramesRGBName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillColorAllFramesRGBNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillColorAllFramesRGBNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillColorName(string path, int frameId, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillColorName(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillColorNameD(string path, double frameId, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillColorNameD(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillColorRGB(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillColorRGB(animationId, frameId, red, green, blue);
		}

		public static void FillColorRGBName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillColorRGBName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillColorRGBNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillColorRGBNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillNonZeroColor(int animationId, int frameId, int color)
		{
			ChromaAnimationAPI.PluginFillNonZeroColor(animationId, frameId, color);
		}

		public static void FillNonZeroColorAllFrames(int animationId, int color)
		{
			ChromaAnimationAPI.PluginFillNonZeroColorAllFrames(animationId, color);
		}

		public static void FillNonZeroColorAllFramesName(string path, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillNonZeroColorAllFramesName(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillNonZeroColorAllFramesNameD(string path, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillNonZeroColorAllFramesNameD(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillNonZeroColorAllFramesRGB(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillNonZeroColorAllFramesRGB(animationId, red, green, blue);
		}

		public static void FillNonZeroColorAllFramesRGBName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillNonZeroColorAllFramesRGBName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillNonZeroColorAllFramesRGBNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillNonZeroColorAllFramesRGBNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillNonZeroColorName(string path, int frameId, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillNonZeroColorName(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillNonZeroColorNameD(string path, double frameId, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillNonZeroColorNameD(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillNonZeroColorRGB(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillNonZeroColorRGB(animationId, frameId, red, green, blue);
		}

		public static void FillNonZeroColorRGBName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillNonZeroColorRGBName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillNonZeroColorRGBNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillNonZeroColorRGBNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillRandomColors(int animationId, int frameId)
		{
			ChromaAnimationAPI.PluginFillRandomColors(animationId, frameId);
		}

		public static void FillRandomColorsAllFrames(int animationId)
		{
			ChromaAnimationAPI.PluginFillRandomColorsAllFrames(animationId);
		}

		public static void FillRandomColorsAllFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillRandomColorsAllFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillRandomColorsAllFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillRandomColorsAllFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillRandomColorsBlackAndWhite(int animationId, int frameId)
		{
			ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhite(animationId, frameId);
		}

		public static void FillRandomColorsBlackAndWhiteAllFrames(int animationId)
		{
			ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhiteAllFrames(animationId);
		}

		public static void FillRandomColorsBlackAndWhiteAllFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhiteAllFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillRandomColorsBlackAndWhiteAllFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhiteAllFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillRandomColorsBlackAndWhiteName(string path, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhiteName(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillRandomColorsBlackAndWhiteNameD(string path, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillRandomColorsBlackAndWhiteNameD(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillRandomColorsName(string path, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillRandomColorsName(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillRandomColorsNameD(string path, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillRandomColorsNameD(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColors(int animationId, int frameId, int threshold, int color)
		{
			ChromaAnimationAPI.PluginFillThresholdColors(animationId, frameId, threshold, color);
		}

		public static void FillThresholdColorsAllFrames(int animationId, int threshold, int color)
		{
			ChromaAnimationAPI.PluginFillThresholdColorsAllFrames(animationId, threshold, color);
		}

		public static void FillThresholdColorsAllFramesName(string path, int threshold, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsAllFramesName(pathIntPtr, threshold, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsAllFramesNameD(string path, double threshold, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsAllFramesNameD(pathIntPtr, threshold, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColorsAllFramesRGB(int animationId, int threshold, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillThresholdColorsAllFramesRGB(animationId, threshold, red, green, blue);
		}

		public static void FillThresholdColorsAllFramesRGBName(string path, int threshold, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsAllFramesRGBName(pathIntPtr, threshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsAllFramesRGBNameD(string path, double threshold, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsAllFramesRGBNameD(pathIntPtr, threshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColorsMinMaxAllFramesRGB(int animationId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			ChromaAnimationAPI.PluginFillThresholdColorsMinMaxAllFramesRGB(animationId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
		}

		public static void FillThresholdColorsMinMaxAllFramesRGBName(string path, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsMinMaxAllFramesRGBName(pathIntPtr, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsMinMaxAllFramesRGBNameD(string path, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsMinMaxAllFramesRGBNameD(pathIntPtr, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColorsMinMaxRGB(int animationId, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			ChromaAnimationAPI.PluginFillThresholdColorsMinMaxRGB(animationId, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
		}

		public static void FillThresholdColorsMinMaxRGBName(string path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsMinMaxRGBName(pathIntPtr, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsMinMaxRGBNameD(string path, double frameId, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsMinMaxRGBNameD(pathIntPtr, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColorsName(string path, int frameId, int threshold, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsName(pathIntPtr, frameId, threshold, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsNameD(string path, double frameId, double threshold, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsNameD(pathIntPtr, frameId, threshold, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdColorsRGB(int animationId, int frameId, int threshold, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillThresholdColorsRGB(animationId, frameId, threshold, red, green, blue);
		}

		public static void FillThresholdColorsRGBName(string path, int frameId, int threshold, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdColorsRGBName(pathIntPtr, frameId, threshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdColorsRGBNameD(string path, double frameId, double threshold, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdColorsRGBNameD(pathIntPtr, frameId, threshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdRGBColorsAllFramesRGB(int animationId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillThresholdRGBColorsAllFramesRGB(animationId, redThreshold, greenThreshold, blueThreshold, red, green, blue);
		}

		public static void FillThresholdRGBColorsAllFramesRGBName(string path, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdRGBColorsAllFramesRGBName(pathIntPtr, redThreshold, greenThreshold, blueThreshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdRGBColorsAllFramesRGBNameD(string path, double redThreshold, double greenThreshold, double blueThreshold, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdRGBColorsAllFramesRGBNameD(pathIntPtr, redThreshold, greenThreshold, blueThreshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillThresholdRGBColorsRGB(int animationId, int frameId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillThresholdRGBColorsRGB(animationId, frameId, redThreshold, greenThreshold, blueThreshold, red, green, blue);
		}

		public static void FillThresholdRGBColorsRGBName(string path, int frameId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillThresholdRGBColorsRGBName(pathIntPtr, frameId, redThreshold, greenThreshold, blueThreshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillThresholdRGBColorsRGBNameD(string path, double frameId, double redThreshold, double greenThreshold, double blueThreshold, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillThresholdRGBColorsRGBNameD(pathIntPtr, frameId, redThreshold, greenThreshold, blueThreshold, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillZeroColor(int animationId, int frameId, int color)
		{
			ChromaAnimationAPI.PluginFillZeroColor(animationId, frameId, color);
		}

		public static void FillZeroColorAllFrames(int animationId, int color)
		{
			ChromaAnimationAPI.PluginFillZeroColorAllFrames(animationId, color);
		}

		public static void FillZeroColorAllFramesName(string path, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillZeroColorAllFramesName(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillZeroColorAllFramesNameD(string path, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillZeroColorAllFramesNameD(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillZeroColorAllFramesRGB(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillZeroColorAllFramesRGB(animationId, red, green, blue);
		}

		public static void FillZeroColorAllFramesRGBName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillZeroColorAllFramesRGBName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillZeroColorAllFramesRGBNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillZeroColorAllFramesRGBNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillZeroColorName(string path, int frameId, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillZeroColorName(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillZeroColorNameD(string path, double frameId, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillZeroColorNameD(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void FillZeroColorRGB(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginFillZeroColorRGB(animationId, frameId, red, green, blue);
		}

		public static void FillZeroColorRGBName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginFillZeroColorRGBName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double FillZeroColorRGBNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginFillZeroColorRGBNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int Get1DColor(int animationId, int frameId, int led)
		{
			return ChromaAnimationAPI.PluginGet1DColor(animationId, frameId, led);
		}

		public static int Get1DColorName(string path, int frameId, int led)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGet1DColorName(pathIntPtr, frameId, led);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double Get1DColorNameD(string path, double frameId, double led)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGet1DColorNameD(pathIntPtr, frameId, led);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int Get2DColor(int animationId, int frameId, int row, int column)
		{
			return ChromaAnimationAPI.PluginGet2DColor(animationId, frameId, row, column);
		}

		public static int Get2DColorName(string path, int frameId, int row, int column)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGet2DColorName(pathIntPtr, frameId, row, column);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double Get2DColorNameD(string path, double frameId, double row, double column)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGet2DColorNameD(pathIntPtr, frameId, row, column);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetAnimation(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			int num = ChromaAnimationAPI.PluginGetAnimation(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetAnimationCount()
		{
			return ChromaAnimationAPI.PluginGetAnimationCount();
		}

		public static double GetAnimationD(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			double num = ChromaAnimationAPI.PluginGetAnimationD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetAnimationId(int index)
		{
			return ChromaAnimationAPI.PluginGetAnimationId(index);
		}

		public static string GetAnimationName(int animationId)
		{
			return Marshal.PtrToStringAnsi(ChromaAnimationAPI.PluginGetAnimationName(animationId));
		}

		public static int GetCurrentFrame(int animationId)
		{
			return ChromaAnimationAPI.PluginGetCurrentFrame(animationId);
		}

		public static int GetCurrentFrameName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetCurrentFrameName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double GetCurrentFrameNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGetCurrentFrameNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetDevice(int animationId)
		{
			return ChromaAnimationAPI.PluginGetDevice(animationId);
		}

		public static int GetDeviceName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetDeviceName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double GetDeviceNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGetDeviceNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetDeviceType(int animationId)
		{
			return ChromaAnimationAPI.PluginGetDeviceType(animationId);
		}

		public static int GetDeviceTypeName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetDeviceTypeName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double GetDeviceTypeNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGetDeviceTypeNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetFrame(int animationId, int frameIndex, out float duration, int[] colors, int length, int[] keys, int keysLength)
		{
			return ChromaAnimationAPI.PluginGetFrame(animationId, frameIndex, out duration, colors, length, keys, keysLength);
		}

		public static int GetFrameCount(int animationId)
		{
			return ChromaAnimationAPI.PluginGetFrameCount(animationId);
		}

		public static int GetFrameCountName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetFrameCountName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double GetFrameCountNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGetFrameCountNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetFrameName(string path, int frameIndex, out float duration, int[] colors, int length, int[] keys, int keysLength)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetFrameName(pathIntPtr, frameIndex, out duration, colors, length, keys, keysLength);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetKeyColor(int animationId, int frameId, int rzkey)
		{
			return ChromaAnimationAPI.PluginGetKeyColor(animationId, frameId, rzkey);
		}

		public static double GetKeyColorD(string path, double frameId, double rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginGetKeyColorD(pathIntPtr, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetKeyColorName(string path, int frameId, int rzkey)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginGetKeyColorName(pathIntPtr, frameId, rzkey);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int GetLibraryLoadedState()
		{
			return ChromaAnimationAPI.PluginGetLibraryLoadedState();
		}

		public static double GetLibraryLoadedStateD()
		{
			return ChromaAnimationAPI.PluginGetLibraryLoadedStateD();
		}

		public static int GetMaxColumn(ChromaAnimationAPI.Device2D device)
		{
			return ChromaAnimationAPI.PluginGetMaxColumn((int)device);
		}

		public static double GetMaxColumnD(double device)
		{
			return ChromaAnimationAPI.PluginGetMaxColumnD(device);
		}

		public static int GetMaxLeds(ChromaAnimationAPI.Device1D device)
		{
			return ChromaAnimationAPI.PluginGetMaxLeds((int)device);
		}

		public static double GetMaxLedsD(double device)
		{
			return ChromaAnimationAPI.PluginGetMaxLedsD(device);
		}

		public static int GetMaxRow(ChromaAnimationAPI.Device2D device)
		{
			return ChromaAnimationAPI.PluginGetMaxRow((int)device);
		}

		public static double GetMaxRowD(double device)
		{
			return ChromaAnimationAPI.PluginGetMaxRowD(device);
		}

		public static int GetPlayingAnimationCount()
		{
			return ChromaAnimationAPI.PluginGetPlayingAnimationCount();
		}

		public static int GetPlayingAnimationId(int index)
		{
			return ChromaAnimationAPI.PluginGetPlayingAnimationId(index);
		}

		public static int GetRGB(int red, int green, int blue)
		{
			return ChromaAnimationAPI.PluginGetRGB(red, green, blue);
		}

		public static double GetRGBD(double red, double green, double blue)
		{
			return ChromaAnimationAPI.PluginGetRGBD(red, green, blue);
		}

		public static bool HasAnimationLoop(int animationId)
		{
			return ChromaAnimationAPI.PluginHasAnimationLoop(animationId);
		}

		public static bool HasAnimationLoopName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			bool flag = ChromaAnimationAPI.PluginHasAnimationLoopName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return flag;
		}

		public static double HasAnimationLoopNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginHasAnimationLoopNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int Init()
		{
			return ChromaAnimationAPI.PluginInit();
		}

		public static double InitD()
		{
			return ChromaAnimationAPI.PluginInitD();
		}

		public static int InitSDK(ref APPINFOTYPE appInfo)
		{
			return ChromaAnimationAPI.PluginInitSDK(ref appInfo);
		}

		public static void InsertDelay(int animationId, int frameId, int delay)
		{
			ChromaAnimationAPI.PluginInsertDelay(animationId, frameId, delay);
		}

		public static void InsertDelayName(string path, int frameId, int delay)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginInsertDelayName(pathIntPtr, frameId, delay);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double InsertDelayNameD(string path, double frameId, double delay)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginInsertDelayNameD(pathIntPtr, frameId, delay);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void InsertFrame(int animationId, int sourceFrame, int targetFrame)
		{
			ChromaAnimationAPI.PluginInsertFrame(animationId, sourceFrame, targetFrame);
		}

		public static void InsertFrameName(string path, int sourceFrame, int targetFrame)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginInsertFrameName(pathIntPtr, sourceFrame, targetFrame);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double InsertFrameNameD(string path, double sourceFrame, double targetFrame)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginInsertFrameNameD(pathIntPtr, sourceFrame, targetFrame);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void InvertColors(int animationId, int frameId)
		{
			ChromaAnimationAPI.PluginInvertColors(animationId, frameId);
		}

		public static void InvertColorsAllFrames(int animationId)
		{
			ChromaAnimationAPI.PluginInvertColorsAllFrames(animationId);
		}

		public static void InvertColorsAllFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginInvertColorsAllFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double InvertColorsAllFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginInvertColorsAllFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void InvertColorsName(string path, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginInvertColorsName(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double InvertColorsNameD(string path, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginInvertColorsNameD(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static bool IsAnimationPaused(int animationId)
		{
			return ChromaAnimationAPI.PluginIsAnimationPaused(animationId);
		}

		public static bool IsAnimationPausedName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			bool flag = ChromaAnimationAPI.PluginIsAnimationPausedName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return flag;
		}

		public static double IsAnimationPausedNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginIsAnimationPausedNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static bool IsDialogOpen()
		{
			return ChromaAnimationAPI.PluginIsDialogOpen();
		}

		public static double IsDialogOpenD()
		{
			return ChromaAnimationAPI.PluginIsDialogOpenD();
		}

		public static bool IsInitialized()
		{
			return ChromaAnimationAPI.PluginIsInitialized();
		}

		public static double IsInitializedD()
		{
			return ChromaAnimationAPI.PluginIsInitializedD();
		}

		public static bool IsPlatformSupported()
		{
			return ChromaAnimationAPI.PluginIsPlatformSupported();
		}

		public static double IsPlatformSupportedD()
		{
			return ChromaAnimationAPI.PluginIsPlatformSupportedD();
		}

		public static bool IsPlaying(int animationId)
		{
			return ChromaAnimationAPI.PluginIsPlaying(animationId);
		}

		public static double IsPlayingD(double animationId)
		{
			return ChromaAnimationAPI.PluginIsPlayingD(animationId);
		}

		public static bool IsPlayingName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			bool flag = ChromaAnimationAPI.PluginIsPlayingName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return flag;
		}

		public static double IsPlayingNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginIsPlayingNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static bool IsPlayingType(int deviceType, int device)
		{
			return ChromaAnimationAPI.PluginIsPlayingType(deviceType, device);
		}

		public static double IsPlayingTypeD(double deviceType, double device)
		{
			return ChromaAnimationAPI.PluginIsPlayingTypeD(deviceType, device);
		}

		public static float Lerp(float start, float end, float amt)
		{
			return ChromaAnimationAPI.PluginLerp(start, end, amt);
		}

		public static int LerpColor(int from, int to, float t)
		{
			return ChromaAnimationAPI.PluginLerpColor(from, to, t);
		}

		public static int LoadAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginLoadAnimation(animationId);
		}

		public static double LoadAnimationD(double animationId)
		{
			return ChromaAnimationAPI.PluginLoadAnimationD(animationId);
		}

		public static void LoadAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginLoadAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void LoadComposite(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			ChromaAnimationAPI.PluginLoadComposite(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void MakeBlankFrames(int animationId, int frameCount, float duration, int color)
		{
			ChromaAnimationAPI.PluginMakeBlankFrames(animationId, frameCount, duration, color);
		}

		public static void MakeBlankFramesName(string path, int frameCount, float duration, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMakeBlankFramesName(pathIntPtr, frameCount, duration, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MakeBlankFramesNameD(string path, double frameCount, double duration, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMakeBlankFramesNameD(pathIntPtr, frameCount, duration, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MakeBlankFramesRandom(int animationId, int frameCount, float duration)
		{
			ChromaAnimationAPI.PluginMakeBlankFramesRandom(animationId, frameCount, duration);
		}

		public static void MakeBlankFramesRandomBlackAndWhite(int animationId, int frameCount, float duration)
		{
			ChromaAnimationAPI.PluginMakeBlankFramesRandomBlackAndWhite(animationId, frameCount, duration);
		}

		public static void MakeBlankFramesRandomBlackAndWhiteName(string path, int frameCount, float duration)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMakeBlankFramesRandomBlackAndWhiteName(pathIntPtr, frameCount, duration);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MakeBlankFramesRandomBlackAndWhiteNameD(string path, double frameCount, double duration)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMakeBlankFramesRandomBlackAndWhiteNameD(pathIntPtr, frameCount, duration);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MakeBlankFramesRandomName(string path, int frameCount, float duration)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMakeBlankFramesRandomName(pathIntPtr, frameCount, duration);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MakeBlankFramesRandomNameD(string path, double frameCount, double duration)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMakeBlankFramesRandomNameD(pathIntPtr, frameCount, duration);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MakeBlankFramesRGB(int animationId, int frameCount, float duration, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginMakeBlankFramesRGB(animationId, frameCount, duration, red, green, blue);
		}

		public static void MakeBlankFramesRGBName(string path, int frameCount, float duration, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMakeBlankFramesRGBName(pathIntPtr, frameCount, duration, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MakeBlankFramesRGBNameD(string path, double frameCount, double duration, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMakeBlankFramesRGBNameD(pathIntPtr, frameCount, duration, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int MirrorHorizontally(int animationId)
		{
			return ChromaAnimationAPI.PluginMirrorHorizontally(animationId);
		}

		public static int MirrorVertically(int animationId)
		{
			return ChromaAnimationAPI.PluginMirrorVertically(animationId);
		}

		public static void MultiplyColorLerpAllFrames(int animationId, int color1, int color2)
		{
			ChromaAnimationAPI.PluginMultiplyColorLerpAllFrames(animationId, color1, color2);
		}

		public static void MultiplyColorLerpAllFramesName(string path, int color1, int color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyColorLerpAllFramesName(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyColorLerpAllFramesNameD(string path, double color1, double color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyColorLerpAllFramesNameD(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensity(int animationId, int frameId, float intensity)
		{
			ChromaAnimationAPI.PluginMultiplyIntensity(animationId, frameId, intensity);
		}

		public static void MultiplyIntensityAllFrames(int animationId, float intensity)
		{
			ChromaAnimationAPI.PluginMultiplyIntensityAllFrames(animationId, intensity);
		}

		public static void MultiplyIntensityAllFramesName(string path, float intensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityAllFramesName(pathIntPtr, intensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityAllFramesNameD(string path, double intensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityAllFramesNameD(pathIntPtr, intensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensityAllFramesRGB(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginMultiplyIntensityAllFramesRGB(animationId, red, green, blue);
		}

		public static void MultiplyIntensityAllFramesRGBName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityAllFramesRGBName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityAllFramesRGBNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityAllFramesRGBNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensityColor(int animationId, int frameId, int color)
		{
			ChromaAnimationAPI.PluginMultiplyIntensityColor(animationId, frameId, color);
		}

		public static void MultiplyIntensityColorAllFrames(int animationId, int color)
		{
			ChromaAnimationAPI.PluginMultiplyIntensityColorAllFrames(animationId, color);
		}

		public static void MultiplyIntensityColorAllFramesName(string path, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityColorAllFramesName(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityColorAllFramesNameD(string path, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityColorAllFramesNameD(pathIntPtr, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensityColorName(string path, int frameId, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityColorName(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityColorNameD(string path, double frameId, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityColorNameD(pathIntPtr, frameId, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensityName(string path, int frameId, float intensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityName(pathIntPtr, frameId, intensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityNameD(string path, double frameId, double intensity)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityNameD(pathIntPtr, frameId, intensity);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyIntensityRGB(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginMultiplyIntensityRGB(animationId, frameId, red, green, blue);
		}

		public static void MultiplyIntensityRGBName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyIntensityRGBName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyIntensityRGBNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyIntensityRGBNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyNonZeroTargetColorLerp(int animationId, int frameId, int color1, int color2)
		{
			ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerp(animationId, frameId, color1, color2);
		}

		public static void MultiplyNonZeroTargetColorLerpAllFrames(int animationId, int color1, int color2)
		{
			ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFrames(animationId, color1, color2);
		}

		public static void MultiplyNonZeroTargetColorLerpAllFramesName(string path, int color1, int color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFramesName(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyNonZeroTargetColorLerpAllFramesNameD(string path, double color1, double color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFramesNameD(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyNonZeroTargetColorLerpAllFramesRGB(int animationId, int red1, int green1, int blue1, int red2, int green2, int blue2)
		{
			ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFramesRGB(animationId, red1, green1, blue1, red2, green2, blue2);
		}

		public static void MultiplyNonZeroTargetColorLerpAllFramesRGBName(string path, int red1, int green1, int blue1, int red2, int green2, int blue2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFramesRGBName(pathIntPtr, red1, green1, blue1, red2, green2, blue2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyNonZeroTargetColorLerpAllFramesRGBNameD(string path, double red1, double green1, double blue1, double red2, double green2, double blue2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyNonZeroTargetColorLerpAllFramesRGBNameD(pathIntPtr, red1, green1, blue1, red2, green2, blue2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyTargetColorLerp(int animationId, int frameId, int color1, int color2)
		{
			ChromaAnimationAPI.PluginMultiplyTargetColorLerp(animationId, frameId, color1, color2);
		}

		public static void MultiplyTargetColorLerpAllFrames(int animationId, int color1, int color2)
		{
			ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFrames(animationId, color1, color2);
		}

		public static void MultiplyTargetColorLerpAllFramesName(string path, int color1, int color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFramesName(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyTargetColorLerpAllFramesNameD(string path, double color1, double color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFramesNameD(pathIntPtr, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyTargetColorLerpAllFramesRGB(int animationId, int red1, int green1, int blue1, int red2, int green2, int blue2)
		{
			ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFramesRGB(animationId, red1, green1, blue1, red2, green2, blue2);
		}

		public static void MultiplyTargetColorLerpAllFramesRGBName(string path, int red1, int green1, int blue1, int red2, int green2, int blue2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFramesRGBName(pathIntPtr, red1, green1, blue1, red2, green2, blue2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double MultiplyTargetColorLerpAllFramesRGBNameD(string path, double red1, double green1, double blue1, double red2, double green2, double blue2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginMultiplyTargetColorLerpAllFramesRGBNameD(pathIntPtr, red1, green1, blue1, red2, green2, blue2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void MultiplyTargetColorLerpName(string path, int frameId, int color1, int color2)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginMultiplyTargetColorLerpName(pathIntPtr, frameId, color1, color2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void OffsetColors(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginOffsetColors(animationId, frameId, red, green, blue);
		}

		public static void OffsetColorsAllFrames(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginOffsetColorsAllFrames(animationId, red, green, blue);
		}

		public static void OffsetColorsAllFramesName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginOffsetColorsAllFramesName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double OffsetColorsAllFramesNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOffsetColorsAllFramesNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void OffsetColorsName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginOffsetColorsName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double OffsetColorsNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOffsetColorsNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void OffsetNonZeroColors(int animationId, int frameId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginOffsetNonZeroColors(animationId, frameId, red, green, blue);
		}

		public static void OffsetNonZeroColorsAllFrames(int animationId, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginOffsetNonZeroColorsAllFrames(animationId, red, green, blue);
		}

		public static void OffsetNonZeroColorsAllFramesName(string path, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginOffsetNonZeroColorsAllFramesName(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double OffsetNonZeroColorsAllFramesNameD(string path, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOffsetNonZeroColorsAllFramesNameD(pathIntPtr, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void OffsetNonZeroColorsName(string path, int frameId, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginOffsetNonZeroColorsName(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double OffsetNonZeroColorsNameD(string path, double frameId, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOffsetNonZeroColorsNameD(pathIntPtr, frameId, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int OpenAnimation(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginOpenAnimation(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double OpenAnimationD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOpenAnimationD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int OpenAnimationFromMemory(byte[] data, string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			int num = ChromaAnimationAPI.PluginOpenAnimationFromMemory(data, pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int OpenEditorDialog(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginOpenEditorDialog(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int OpenEditorDialogAndPlay(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginOpenEditorDialogAndPlay(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double OpenEditorDialogAndPlayD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOpenEditorDialogAndPlayD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static double OpenEditorDialogD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginOpenEditorDialogD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int OverrideFrameDuration(int animationId, float duration)
		{
			return ChromaAnimationAPI.PluginOverrideFrameDuration(animationId, duration);
		}

		public static double OverrideFrameDurationD(double animationId, double duration)
		{
			return ChromaAnimationAPI.PluginOverrideFrameDurationD(animationId, duration);
		}

		public static void OverrideFrameDurationName(string path, float duration)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginOverrideFrameDurationName(pathIntPtr, duration);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void PauseAnimation(int animationId)
		{
			ChromaAnimationAPI.PluginPauseAnimation(animationId);
		}

		public static void PauseAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginPauseAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double PauseAnimationNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginPauseAnimationNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int PlayAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginPlayAnimation(animationId);
		}

		public static double PlayAnimationD(double animationId)
		{
			return ChromaAnimationAPI.PluginPlayAnimationD(animationId);
		}

		public static void PlayAnimationFrame(int animationId, int frameId, bool loop)
		{
			ChromaAnimationAPI.PluginPlayAnimationFrame(animationId, frameId, loop);
		}

		public static void PlayAnimationFrameName(string path, int frameId, bool loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginPlayAnimationFrameName(pathIntPtr, frameId, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double PlayAnimationFrameNameD(string path, double frameId, double loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginPlayAnimationFrameNameD(pathIntPtr, frameId, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void PlayAnimationLoop(int animationId, bool loop)
		{
			ChromaAnimationAPI.PluginPlayAnimationLoop(animationId, loop);
		}

		public static void PlayAnimationName(string path, bool loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginPlayAnimationName(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double PlayAnimationNameD(string path, double loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginPlayAnimationNameD(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void PlayComposite(string name, bool loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			ChromaAnimationAPI.PluginPlayComposite(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double PlayCompositeD(string name, double loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			double num = ChromaAnimationAPI.PluginPlayCompositeD(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int PreviewFrame(int animationId, int frameIndex)
		{
			return ChromaAnimationAPI.PluginPreviewFrame(animationId, frameIndex);
		}

		public static double PreviewFrameD(double animationId, double frameIndex)
		{
			return ChromaAnimationAPI.PluginPreviewFrameD(animationId, frameIndex);
		}

		public static void PreviewFrameName(string path, int frameIndex)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginPreviewFrameName(pathIntPtr, frameIndex);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void ReduceFrames(int animationId, int n)
		{
			ChromaAnimationAPI.PluginReduceFrames(animationId, n);
		}

		public static void ReduceFramesName(string path, int n)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginReduceFramesName(pathIntPtr, n);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double ReduceFramesNameD(string path, double n)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginReduceFramesNameD(pathIntPtr, n);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int ResetAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginResetAnimation(animationId);
		}

		public static void ResumeAnimation(int animationId, bool loop)
		{
			ChromaAnimationAPI.PluginResumeAnimation(animationId, loop);
		}

		public static void ResumeAnimationName(string path, bool loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginResumeAnimationName(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double ResumeAnimationNameD(string path, double loop)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginResumeAnimationNameD(pathIntPtr, loop);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int Reverse(int animationId)
		{
			return ChromaAnimationAPI.PluginReverse(animationId);
		}

		public static void ReverseAllFrames(int animationId)
		{
			ChromaAnimationAPI.PluginReverseAllFrames(animationId);
		}

		public static void ReverseAllFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginReverseAllFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double ReverseAllFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginReverseAllFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int SaveAnimation(int animationId, string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginSaveAnimation(animationId, pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int SaveAnimationName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			int num = ChromaAnimationAPI.PluginSaveAnimationName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void Set1DColor(int animationId, int frameId, int led, int color)
		{
			ChromaAnimationAPI.PluginSet1DColor(animationId, frameId, led, color);
		}

		public static void Set1DColorName(string path, int frameId, int led, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSet1DColorName(pathIntPtr, frameId, led, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double Set1DColorNameD(string path, double frameId, double led, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSet1DColorNameD(pathIntPtr, frameId, led, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void Set2DColor(int animationId, int frameId, int row, int column, int color)
		{
			ChromaAnimationAPI.PluginSet2DColor(animationId, frameId, row, column, color);
		}

		public static void Set2DColorName(string path, int frameId, int row, int column, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSet2DColorName(pathIntPtr, frameId, row, column, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double Set2DColorNameD(string path, double frameId, double rowColumnIndex, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSet2DColorNameD(pathIntPtr, frameId, rowColumnIndex, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetChromaCustomColorAllFrames(int animationId)
		{
			ChromaAnimationAPI.PluginSetChromaCustomColorAllFrames(animationId);
		}

		public static void SetChromaCustomColorAllFramesName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetChromaCustomColorAllFramesName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetChromaCustomColorAllFramesNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetChromaCustomColorAllFramesNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetChromaCustomFlag(int animationId, bool flag)
		{
			ChromaAnimationAPI.PluginSetChromaCustomFlag(animationId, flag);
		}

		public static void SetChromaCustomFlagName(string path, bool flag)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetChromaCustomFlagName(pathIntPtr, flag);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetChromaCustomFlagNameD(string path, double flag)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetChromaCustomFlagNameD(pathIntPtr, flag);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetCurrentFrame(int animationId, int frameId)
		{
			ChromaAnimationAPI.PluginSetCurrentFrame(animationId, frameId);
		}

		public static void SetCurrentFrameName(string path, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetCurrentFrameName(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetCurrentFrameNameD(string path, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetCurrentFrameNameD(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int SetCustomColorFlag2D(int device, int[] colors)
		{
			return ChromaAnimationAPI.PluginSetCustomColorFlag2D(device, colors);
		}

		public static int SetDevice(int animationId, int deviceType, int device)
		{
			return ChromaAnimationAPI.PluginSetDevice(animationId, deviceType, device);
		}

		public static int SetEffect(Guid effectId)
		{
			return ChromaAnimationAPI.PluginSetEffect(effectId);
		}

		public static int SetEffectCustom1D(int device, int[] colors)
		{
			return ChromaAnimationAPI.PluginSetEffectCustom1D(device, colors);
		}

		public static int SetEffectCustom2D(int device, int[] colors)
		{
			return ChromaAnimationAPI.PluginSetEffectCustom2D(device, colors);
		}

		public static int SetEffectKeyboardCustom2D(int device, int[] colors, int[] keys)
		{
			return ChromaAnimationAPI.PluginSetEffectKeyboardCustom2D(device, colors, keys);
		}

		public static void SetIdleAnimation(int animationId)
		{
			ChromaAnimationAPI.PluginSetIdleAnimation(animationId);
		}

		public static void SetIdleAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetIdleAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeyColor(int animationId, int frameId, int rzkey, int color)
		{
			ChromaAnimationAPI.PluginSetKeyColor(animationId, frameId, rzkey, color);
		}

		public static void SetKeyColorAllFrames(int animationId, int rzkey, int color)
		{
			ChromaAnimationAPI.PluginSetKeyColorAllFrames(animationId, rzkey, color);
		}

		public static void SetKeyColorAllFramesName(string path, int rzkey, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyColorAllFramesName(pathIntPtr, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyColorAllFramesNameD(string path, double rzkey, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyColorAllFramesNameD(pathIntPtr, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyColorAllFramesRGB(int animationId, int rzkey, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeyColorAllFramesRGB(animationId, rzkey, red, green, blue);
		}

		public static void SetKeyColorAllFramesRGBName(string path, int rzkey, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyColorAllFramesRGBName(pathIntPtr, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyColorAllFramesRGBNameD(string path, double rzkey, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyColorAllFramesRGBNameD(pathIntPtr, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyColorName(string path, int frameId, int rzkey, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyColorName(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyColorNameD(string path, double frameId, double rzkey, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyColorNameD(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeyColorRGB(animationId, frameId, rzkey, red, green, blue);
		}

		public static void SetKeyColorRGBName(string path, int frameId, int rzkey, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyColorRGBName(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyColorRGBNameD(string path, double frameId, double rzkey, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyColorRGBNameD(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyNonZeroColor(int animationId, int frameId, int rzkey, int color)
		{
			ChromaAnimationAPI.PluginSetKeyNonZeroColor(animationId, frameId, rzkey, color);
		}

		public static void SetKeyNonZeroColorName(string path, int frameId, int rzkey, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyNonZeroColorName(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyNonZeroColorNameD(string path, double frameId, double rzkey, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyNonZeroColorNameD(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyNonZeroColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeyNonZeroColorRGB(animationId, frameId, rzkey, red, green, blue);
		}

		public static void SetKeyNonZeroColorRGBName(string path, int frameId, int rzkey, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyNonZeroColorRGBName(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyNonZeroColorRGBNameD(string path, double frameId, double rzkey, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyNonZeroColorRGBNameD(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyRowColumnColorName(string path, int frameId, int row, int column, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyRowColumnColorName(pathIntPtr, frameId, row, column, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysColor(animationId, frameId, rzkeys, keyCount, color);
		}

		public static void SetKeysColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysColorAllFrames(animationId, rzkeys, keyCount, color);
		}

		public static void SetKeysColorAllFramesName(string path, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysColorAllFramesName(pathIntPtr, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysColorAllFramesRGB(int animationId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeysColorAllFramesRGB(animationId, rzkeys, keyCount, red, green, blue);
		}

		public static void SetKeysColorAllFramesRGBName(string path, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysColorAllFramesRGBName(pathIntPtr, rzkeys, keyCount, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysColorName(string path, int frameId, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysColorName(pathIntPtr, frameId, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeysColorRGB(animationId, frameId, rzkeys, keyCount, red, green, blue);
		}

		public static void SetKeysColorRGBName(string path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysColorRGBName(pathIntPtr, frameId, rzkeys, keyCount, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysNonZeroColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysNonZeroColor(animationId, frameId, rzkeys, keyCount, color);
		}

		public static void SetKeysNonZeroColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysNonZeroColorAllFrames(animationId, rzkeys, keyCount, color);
		}

		public static void SetKeysNonZeroColorAllFramesName(string path, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysNonZeroColorAllFramesName(pathIntPtr, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysNonZeroColorName(string path, int frameId, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysNonZeroColorName(pathIntPtr, frameId, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysNonZeroColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeysNonZeroColorRGB(animationId, frameId, rzkeys, keyCount, red, green, blue);
		}

		public static void SetKeysNonZeroColorRGBName(string path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysNonZeroColorRGBName(pathIntPtr, frameId, rzkeys, keyCount, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysZeroColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysZeroColor(animationId, frameId, rzkeys, keyCount, color);
		}

		public static void SetKeysZeroColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color)
		{
			ChromaAnimationAPI.PluginSetKeysZeroColorAllFrames(animationId, rzkeys, keyCount, color);
		}

		public static void SetKeysZeroColorAllFramesName(string path, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysZeroColorAllFramesName(pathIntPtr, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysZeroColorAllFramesRGB(int animationId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeysZeroColorAllFramesRGB(animationId, rzkeys, keyCount, red, green, blue);
		}

		public static void SetKeysZeroColorAllFramesRGBName(string path, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysZeroColorAllFramesRGBName(pathIntPtr, rzkeys, keyCount, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysZeroColorName(string path, int frameId, int[] rzkeys, int keyCount, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysZeroColorName(pathIntPtr, frameId, rzkeys, keyCount, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeysZeroColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeysZeroColorRGB(animationId, frameId, rzkeys, keyCount, red, green, blue);
		}

		public static void SetKeysZeroColorRGBName(string path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeysZeroColorRGBName(pathIntPtr, frameId, rzkeys, keyCount, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void SetKeyZeroColor(int animationId, int frameId, int rzkey, int color)
		{
			ChromaAnimationAPI.PluginSetKeyZeroColor(animationId, frameId, rzkey, color);
		}

		public static void SetKeyZeroColorName(string path, int frameId, int rzkey, int color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyZeroColorName(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyZeroColorNameD(string path, double frameId, double rzkey, double color)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyZeroColorNameD(pathIntPtr, frameId, rzkey, color);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetKeyZeroColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue)
		{
			ChromaAnimationAPI.PluginSetKeyZeroColorRGB(animationId, frameId, rzkey, red, green, blue);
		}

		public static void SetKeyZeroColorRGBName(string path, int frameId, int rzkey, int red, int green, int blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSetKeyZeroColorRGBName(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SetKeyZeroColorRGBNameD(string path, double frameId, double rzkey, double red, double green, double blue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSetKeyZeroColorRGBNameD(pathIntPtr, frameId, rzkey, red, green, blue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SetLogDelegate(IntPtr fp)
		{
			ChromaAnimationAPI.PluginSetLogDelegate(fp);
		}

		public static void SetStaticColor(int deviceType, int device, int color)
		{
			ChromaAnimationAPI.PluginSetStaticColor(deviceType, device, color);
		}

		public static void SetStaticColorAll(int color)
		{
			ChromaAnimationAPI.PluginSetStaticColorAll(color);
		}

		public static void StaticColor(int deviceType, int device, int color)
		{
			ChromaAnimationAPI.PluginStaticColor(deviceType, device, color);
		}

		public static void StaticColorAll(int color)
		{
			ChromaAnimationAPI.PluginStaticColorAll(color);
		}

		public static double StaticColorD(double deviceType, double device, double color)
		{
			return ChromaAnimationAPI.PluginStaticColorD(deviceType, device, color);
		}

		public static void StopAll()
		{
			ChromaAnimationAPI.PluginStopAll();
		}

		public static int StopAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginStopAnimation(animationId);
		}

		public static double StopAnimationD(double animationId)
		{
			return ChromaAnimationAPI.PluginStopAnimationD(animationId);
		}

		public static void StopAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginStopAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double StopAnimationNameD(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginStopAnimationNameD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void StopAnimationType(int deviceType, int device)
		{
			ChromaAnimationAPI.PluginStopAnimationType(deviceType, device);
		}

		public static double StopAnimationTypeD(double deviceType, double device)
		{
			return ChromaAnimationAPI.PluginStopAnimationTypeD(deviceType, device);
		}

		public static void StopComposite(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			ChromaAnimationAPI.PluginStopComposite(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double StopCompositeD(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			double num = ChromaAnimationAPI.PluginStopCompositeD(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int SubtractColor(int color1, int color2)
		{
			return ChromaAnimationAPI.PluginSubtractColor(color1, color2);
		}

		public static void SubtractNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeys(sourceAnimationId, targetAnimationId, frameId);
		}

		public static void SubtractNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void SubtractNonZeroAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void SubtractNonZeroAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractNonZeroAllKeysName(string sourceAnimation, string targetAnimation, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysName(pathIntPtr, pathIntPtr2, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static void SubtractNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void SubtractNonZeroAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFrames(sourceAnimationId, targetAnimationId);
		}

		public static void SubtractNonZeroTargetAllKeysAllFramesName(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFramesName(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroTargetAllKeysAllFramesNameD(string sourceAnimation, string targetAnimation)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFramesNameD(pathIntPtr, pathIntPtr2);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFramesOffset(sourceAnimationId, targetAnimationId, offset);
		}

		public static void SubtractNonZeroTargetAllKeysAllFramesOffsetName(string sourceAnimation, string targetAnimation, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFramesOffsetName(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroTargetAllKeysAllFramesOffsetNameD(string sourceAnimation, string targetAnimation, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysAllFramesOffsetNameD(pathIntPtr, pathIntPtr2, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset)
		{
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysOffset(sourceAnimationId, targetAnimationId, frameId, offset);
		}

		public static void SubtractNonZeroTargetAllKeysOffsetName(string sourceAnimation, string targetAnimation, int frameId, int offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysOffsetName(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
		}

		public static double SubtractNonZeroTargetAllKeysOffsetNameD(string sourceAnimation, string targetAnimation, double frameId, double offset)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(sourceAnimation));
			IntPtr pathIntPtr2 = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(targetAnimation));
			double num = ChromaAnimationAPI.PluginSubtractNonZeroTargetAllKeysOffsetNameD(pathIntPtr, pathIntPtr2, frameId, offset);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr2);
			return num;
		}

		public static void SubtractThresholdColorsMinMaxAllFramesRGB(int animationId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxAllFramesRGB(animationId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
		}

		public static void SubtractThresholdColorsMinMaxAllFramesRGBName(string path, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxAllFramesRGBName(pathIntPtr, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SubtractThresholdColorsMinMaxAllFramesRGBNameD(string path, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxAllFramesRGBNameD(pathIntPtr, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void SubtractThresholdColorsMinMaxRGB(int animationId, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxRGB(animationId, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
		}

		public static void SubtractThresholdColorsMinMaxRGBName(string path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxRGBName(pathIntPtr, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double SubtractThresholdColorsMinMaxRGBNameD(string path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginSubtractThresholdColorsMinMaxRGBNameD(pathIntPtr, frameId, minThreshold, minRed, minGreen, minBlue, maxThreshold, maxRed, maxGreen, maxBlue);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void TrimEndFrames(int animationId, int lastFrameId)
		{
			ChromaAnimationAPI.PluginTrimEndFrames(animationId, lastFrameId);
		}

		public static void TrimEndFramesName(string path, int lastFrameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginTrimEndFramesName(pathIntPtr, lastFrameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double TrimEndFramesNameD(string path, double lastFrameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginTrimEndFramesNameD(pathIntPtr, lastFrameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void TrimFrame(int animationId, int frameId)
		{
			ChromaAnimationAPI.PluginTrimFrame(animationId, frameId);
		}

		public static void TrimFrameName(string path, int frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginTrimFrameName(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double TrimFrameNameD(string path, double frameId)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginTrimFrameNameD(pathIntPtr, frameId);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void TrimStartFrames(int animationId, int numberOfFrames)
		{
			ChromaAnimationAPI.PluginTrimStartFrames(animationId, numberOfFrames);
		}

		public static void TrimStartFramesName(string path, int numberOfFrames)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginTrimStartFramesName(pathIntPtr, numberOfFrames);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static double TrimStartFramesNameD(string path, double numberOfFrames)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			double num = ChromaAnimationAPI.PluginTrimStartFramesNameD(pathIntPtr, numberOfFrames);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static int Uninit()
		{
			return ChromaAnimationAPI.PluginUninit();
		}

		public static double UninitD()
		{
			return ChromaAnimationAPI.PluginUninitD();
		}

		public static int UnloadAnimation(int animationId)
		{
			return ChromaAnimationAPI.PluginUnloadAnimation(animationId);
		}

		public static double UnloadAnimationD(double animationId)
		{
			return ChromaAnimationAPI.PluginUnloadAnimationD(animationId);
		}

		public static void UnloadAnimationName(string path)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginUnloadAnimationName(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void UnloadComposite(string name)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(name));
			ChromaAnimationAPI.PluginUnloadComposite(pathIntPtr);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		public static void UnloadLibrarySDK()
		{
			ChromaAnimationAPI.PluginUnloadLibrarySDK();
		}

		public static void UnloadLibraryStreamingPlugin()
		{
			ChromaAnimationAPI.PluginUnloadLibraryStreamingPlugin();
		}

		public static int UpdateFrame(int animationId, int frameIndex, float duration, int[] colors, int length, int[] keys, int keysLength)
		{
			return ChromaAnimationAPI.PluginUpdateFrame(animationId, frameIndex, duration, colors, length, keys, keysLength);
		}

		public static int UpdateFrameName(string path, int frameIndex, float duration, int[] colors, int length, int[] keys, int keysLength)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			int num = ChromaAnimationAPI.PluginUpdateFrameName(pathIntPtr, frameIndex, duration, colors, length, keys, keysLength);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
			return num;
		}

		public static void UseIdleAnimation(int device, bool flag)
		{
			ChromaAnimationAPI.PluginUseIdleAnimation(device, flag);
		}

		public static void UseIdleAnimations(bool flag)
		{
			ChromaAnimationAPI.PluginUseIdleAnimations(flag);
		}

		public static void UsePreloading(int animationId, bool flag)
		{
			ChromaAnimationAPI.PluginUsePreloading(animationId, flag);
		}

		public static void UsePreloadingName(string path, bool flag)
		{
			IntPtr pathIntPtr = ChromaAnimationAPI.GetPathIntPtr(ChromaAnimationAPI.GetStreamingPath(path));
			ChromaAnimationAPI.PluginUsePreloadingName(pathIntPtr, flag);
			ChromaAnimationAPI.FreeIntPtr(pathIntPtr);
		}

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginAddColor(int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginAddFrame(int animationId, float duration, int[] colors, int length);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroTargetAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroTargetAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAddNonZeroTargetAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAddNonZeroTargetAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAppendAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginAppendAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginAppendAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginClearAll();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginClearAnimationType(int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCloseAll();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCloseAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCloseAnimationD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCloseAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCloseAnimationNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCloseComposite(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCloseCompositeD(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCopyAnimation(int sourceAnimationId, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyAnimationName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyAnimationNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyBlueChannelAllFrames(int animationId, float redIntensity, float greenIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyBlueChannelAllFramesName(IntPtr path, float redIntensity, float greenIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyBlueChannelAllFramesNameD(IntPtr path, double redIntensity, double greenIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyGreenChannelAllFrames(int animationId, float redIntensity, float blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyGreenChannelAllFramesName(IntPtr path, float redIntensity, float blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyGreenChannelAllFramesNameD(IntPtr path, double redIntensity, double blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColorAllFrames(int sourceAnimationId, int targetAnimationId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColorAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyKeyColorAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColorAllFramesOffset(int sourceAnimationId, int targetAnimationId, int rzkey, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColorAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int rzkey, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyKeyColorAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double rzkey, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeyColorName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyKeyColorNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColor(int sourceAnimationId, int targetAnimationId, int frameId, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColorAllFrames(int sourceAnimationId, int targetAnimationId, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColorAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColorName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColorOffset(int sourceAnimationId, int targetAnimationId, int sourceFrameId, int targetFrameId, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyKeysColorOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int sourceFrameId, int targetFrameId, int[] keys, int size);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroAllKeysNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroKeyColorName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroKeyColorNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroTargetAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroTargetAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroTargetAllKeysNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroTargetAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyNonZeroTargetZeroAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyNonZeroTargetZeroAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyRedChannelAllFrames(int animationId, float greenIntensity, float blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyRedChannelAllFramesName(IntPtr path, float greenIntensity, float blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyRedChannelAllFramesNameD(IntPtr path, double greenIntensity, double blueIntensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyZeroAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyZeroAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroKeyColor(int sourceAnimationId, int targetAnimationId, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroKeyColorName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyZeroKeyColorNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroTargetAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroTargetAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginCopyZeroTargetAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCopyZeroTargetAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateChromaLinkEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateEffect(Guid deviceId, int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateHeadsetEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateKeyboardEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateKeypadEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateMouseEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreCreateMousepadEffect(int effect, IntPtr pParam, out Guid pEffectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreDeleteEffect(Guid effectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreInit();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreInitSDK(ref APPINFOTYPE appInfo);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreQueryDevice(Guid deviceId, out DEVICE_INFO_TYPE deviceInfo);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreSetEffect(Guid effectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamBroadcast(IntPtr streamId, IntPtr streamKey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamBroadcastEnd();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCoreStreamGetAuthShortcode(IntPtr shortcode, out byte length, IntPtr platform, IntPtr title);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamGetFocus(IntPtr focus, out byte length);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCoreStreamGetId(IntPtr shortcode, IntPtr streamId, out byte length);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginCoreStreamGetKey(IntPtr shortcode, IntPtr streamKey, out byte length);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern StreamStatusType PluginCoreStreamGetStatus();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr PluginCoreStreamGetStatusString(StreamStatusType status);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamReleaseShortcode(IntPtr shortcode);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamSetFocus(IntPtr focus);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamSupportsStreaming();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamWatch(IntPtr streamId, ulong timestamp);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginCoreStreamWatchEnd();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCoreUnInit();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCreateAnimation(IntPtr path, int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCreateAnimationInMemory(int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginCreateEffect(Guid deviceId, int effect, int[] colors, int size, out FChromaSDKGuid effectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginDeleteEffect(Guid effectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateFirstFrame(int animationId, int frameCount);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateFirstFrameName(IntPtr path, int frameCount);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginDuplicateFirstFrameNameD(IntPtr path, double frameCount);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginDuplicateFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateMirrorFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginDuplicateMirrorFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginDuplicateMirrorFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFadeEndFrames(int animationId, int fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFadeEndFramesName(IntPtr path, int fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFadeEndFramesNameD(IntPtr path, double fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFadeStartFrames(int animationId, int fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFadeStartFramesName(IntPtr path, int fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFadeStartFramesNameD(IntPtr path, double fade);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColor(int animationId, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorAllFrames(int animationId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorAllFramesName(IntPtr path, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillColorAllFramesNameD(IntPtr path, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorAllFramesRGB(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorAllFramesRGBName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillColorAllFramesRGBNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorName(IntPtr path, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillColorNameD(IntPtr path, double frameId, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorRGB(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillColorRGBName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillColorRGBNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColor(int animationId, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorAllFrames(int animationId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorAllFramesName(IntPtr path, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillNonZeroColorAllFramesNameD(IntPtr path, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorAllFramesRGB(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorAllFramesRGBName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillNonZeroColorAllFramesRGBNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorName(IntPtr path, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillNonZeroColorNameD(IntPtr path, double frameId, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorRGB(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillNonZeroColorRGBName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillNonZeroColorRGBNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColors(int animationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsAllFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsAllFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillRandomColorsAllFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsBlackAndWhite(int animationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsBlackAndWhiteAllFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsBlackAndWhiteAllFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillRandomColorsBlackAndWhiteAllFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsBlackAndWhiteName(IntPtr path, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillRandomColorsBlackAndWhiteNameD(IntPtr path, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillRandomColorsName(IntPtr path, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillRandomColorsNameD(IntPtr path, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColors(int animationId, int frameId, int threshold, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsAllFrames(int animationId, int threshold, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsAllFramesName(IntPtr path, int threshold, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsAllFramesNameD(IntPtr path, double threshold, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsAllFramesRGB(int animationId, int threshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsAllFramesRGBName(IntPtr path, int threshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsAllFramesRGBNameD(IntPtr path, double threshold, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsMinMaxAllFramesRGB(int animationId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsMinMaxAllFramesRGBName(IntPtr path, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsMinMaxAllFramesRGBNameD(IntPtr path, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsMinMaxRGB(int animationId, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsMinMaxRGBName(IntPtr path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsMinMaxRGBNameD(IntPtr path, double frameId, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsName(IntPtr path, int frameId, int threshold, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsNameD(IntPtr path, double frameId, double threshold, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsRGB(int animationId, int frameId, int threshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdColorsRGBName(IntPtr path, int frameId, int threshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdColorsRGBNameD(IntPtr path, double frameId, double threshold, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdRGBColorsAllFramesRGB(int animationId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdRGBColorsAllFramesRGBName(IntPtr path, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdRGBColorsAllFramesRGBNameD(IntPtr path, double redThreshold, double greenThreshold, double blueThreshold, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdRGBColorsRGB(int animationId, int frameId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillThresholdRGBColorsRGBName(IntPtr path, int frameId, int redThreshold, int greenThreshold, int blueThreshold, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillThresholdRGBColorsRGBNameD(IntPtr path, double frameId, double redThreshold, double greenThreshold, double blueThreshold, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColor(int animationId, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorAllFrames(int animationId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorAllFramesName(IntPtr path, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillZeroColorAllFramesNameD(IntPtr path, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorAllFramesRGB(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorAllFramesRGBName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillZeroColorAllFramesRGBNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorName(IntPtr path, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillZeroColorNameD(IntPtr path, double frameId, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorRGB(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginFillZeroColorRGBName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginFillZeroColorRGBNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGet1DColor(int animationId, int frameId, int led);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGet1DColorName(IntPtr path, int frameId, int led);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGet1DColorNameD(IntPtr path, double frameId, double led);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGet2DColor(int animationId, int frameId, int row, int column);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGet2DColorName(IntPtr path, int frameId, int row, int column);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGet2DColorNameD(IntPtr path, double frameId, double row, double column);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetAnimation(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetAnimationCount();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetAnimationD(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetAnimationId(int index);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr PluginGetAnimationName(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetCurrentFrame(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetCurrentFrameName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetCurrentFrameNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetDevice(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetDeviceName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetDeviceNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetDeviceType(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetDeviceTypeName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetDeviceTypeNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetFrame(int animationId, int frameIndex, out float duration, int[] colors, int length, int[] keys, int keysLength);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetFrameCount(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetFrameCountName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetFrameCountNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetFrameName(IntPtr path, int frameIndex, out float duration, int[] colors, int length, int[] keys, int keysLength);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetKeyColor(int animationId, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetKeyColorD(IntPtr path, double frameId, double rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetKeyColorName(IntPtr path, int frameId, int rzkey);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetLibraryLoadedState();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetLibraryLoadedStateD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetMaxColumn(int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetMaxColumnD(double device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetMaxLeds(int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetMaxLedsD(double device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetMaxRow(int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetMaxRowD(double device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetPlayingAnimationCount();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetPlayingAnimationId(int index);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginGetRGB(int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginGetRGBD(double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginHasAnimationLoop(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginHasAnimationLoopName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginHasAnimationLoopNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginInit();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginInitD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginInitSDK(ref APPINFOTYPE appInfo);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInsertDelay(int animationId, int frameId, int delay);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInsertDelayName(IntPtr path, int frameId, int delay);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginInsertDelayNameD(IntPtr path, double frameId, double delay);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInsertFrame(int animationId, int sourceFrame, int targetFrame);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInsertFrameName(IntPtr path, int sourceFrame, int targetFrame);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginInsertFrameNameD(IntPtr path, double sourceFrame, double targetFrame);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInvertColors(int animationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInvertColorsAllFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInvertColorsAllFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginInvertColorsAllFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginInvertColorsName(IntPtr path, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginInvertColorsNameD(IntPtr path, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsAnimationPaused(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsAnimationPausedName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsAnimationPausedNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsDialogOpen();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsDialogOpenD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsInitialized();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsInitializedD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsPlatformSupported();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsPlatformSupportedD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsPlaying(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsPlayingD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsPlayingName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsPlayingNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool PluginIsPlayingType(int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginIsPlayingTypeD(double deviceType, double device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern float PluginLerp(float start, float end, float amt);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginLerpColor(int from, int to, float t);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginLoadAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginLoadAnimationD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginLoadAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginLoadComposite(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFrames(int animationId, int frameCount, float duration, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesName(IntPtr path, int frameCount, float duration, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMakeBlankFramesNameD(IntPtr path, double frameCount, double duration, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRandom(int animationId, int frameCount, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRandomBlackAndWhite(int animationId, int frameCount, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRandomBlackAndWhiteName(IntPtr path, int frameCount, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMakeBlankFramesRandomBlackAndWhiteNameD(IntPtr path, double frameCount, double duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRandomName(IntPtr path, int frameCount, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMakeBlankFramesRandomNameD(IntPtr path, double frameCount, double duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRGB(int animationId, int frameCount, float duration, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMakeBlankFramesRGBName(IntPtr path, int frameCount, float duration, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMakeBlankFramesRGBNameD(IntPtr path, double frameCount, double duration, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginMirrorHorizontally(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginMirrorVertically(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyColorLerpAllFrames(int animationId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyColorLerpAllFramesName(IntPtr path, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyColorLerpAllFramesNameD(IntPtr path, double color1, double color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensity(int animationId, int frameId, float intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityAllFrames(int animationId, float intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityAllFramesName(IntPtr path, float intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityAllFramesNameD(IntPtr path, double intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityAllFramesRGB(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityAllFramesRGBName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityAllFramesRGBNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityColor(int animationId, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityColorAllFrames(int animationId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityColorAllFramesName(IntPtr path, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityColorAllFramesNameD(IntPtr path, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityColorName(IntPtr path, int frameId, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityColorNameD(IntPtr path, double frameId, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityName(IntPtr path, int frameId, float intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityNameD(IntPtr path, double frameId, double intensity);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityRGB(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyIntensityRGBName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyIntensityRGBNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyNonZeroTargetColorLerp(int animationId, int frameId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyNonZeroTargetColorLerpAllFrames(int animationId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyNonZeroTargetColorLerpAllFramesName(IntPtr path, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyNonZeroTargetColorLerpAllFramesNameD(IntPtr path, double color1, double color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyNonZeroTargetColorLerpAllFramesRGB(int animationId, int red1, int green1, int blue1, int red2, int green2, int blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyNonZeroTargetColorLerpAllFramesRGBName(IntPtr path, int red1, int green1, int blue1, int red2, int green2, int blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyNonZeroTargetColorLerpAllFramesRGBNameD(IntPtr path, double red1, double green1, double blue1, double red2, double green2, double blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerp(int animationId, int frameId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerpAllFrames(int animationId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerpAllFramesName(IntPtr path, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyTargetColorLerpAllFramesNameD(IntPtr path, double color1, double color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerpAllFramesRGB(int animationId, int red1, int green1, int blue1, int red2, int green2, int blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerpAllFramesRGBName(IntPtr path, int red1, int green1, int blue1, int red2, int green2, int blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginMultiplyTargetColorLerpAllFramesRGBNameD(IntPtr path, double red1, double green1, double blue1, double red2, double green2, double blue2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginMultiplyTargetColorLerpName(IntPtr path, int frameId, int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetColors(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetColorsAllFrames(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetColorsAllFramesName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOffsetColorsAllFramesNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetColorsName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOffsetColorsNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetNonZeroColors(int animationId, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetNonZeroColorsAllFrames(int animationId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetNonZeroColorsAllFramesName(IntPtr path, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOffsetNonZeroColorsAllFramesNameD(IntPtr path, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOffsetNonZeroColorsName(IntPtr path, int frameId, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOffsetNonZeroColorsNameD(IntPtr path, double frameId, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginOpenAnimation(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOpenAnimationD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginOpenAnimationFromMemory(byte[] data, IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginOpenEditorDialog(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginOpenEditorDialogAndPlay(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOpenEditorDialogAndPlayD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOpenEditorDialogD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginOverrideFrameDuration(int animationId, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginOverrideFrameDurationD(double animationId, double duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginOverrideFrameDurationName(IntPtr path, float duration);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPauseAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPauseAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPauseAnimationNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginPlayAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPlayAnimationD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPlayAnimationFrame(int animationId, int frameId, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPlayAnimationFrameName(IntPtr path, int frameId, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPlayAnimationFrameNameD(IntPtr path, double frameId, double loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPlayAnimationLoop(int animationId, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPlayAnimationName(IntPtr path, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPlayAnimationNameD(IntPtr path, double loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPlayComposite(IntPtr name, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPlayCompositeD(IntPtr name, double loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginPreviewFrame(int animationId, int frameIndex);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginPreviewFrameD(double animationId, double frameIndex);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginPreviewFrameName(IntPtr path, int frameIndex);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginReduceFrames(int animationId, int n);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginReduceFramesName(IntPtr path, int n);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginReduceFramesNameD(IntPtr path, double n);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginResetAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginResumeAnimation(int animationId, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginResumeAnimationName(IntPtr path, bool loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginResumeAnimationNameD(IntPtr path, double loop);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginReverse(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginReverseAllFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginReverseAllFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginReverseAllFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSaveAnimation(int animationId, IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSaveAnimationName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSet1DColor(int animationId, int frameId, int led, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSet1DColorName(IntPtr path, int frameId, int led, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSet1DColorNameD(IntPtr path, double frameId, double led, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSet2DColor(int animationId, int frameId, int row, int column, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSet2DColorName(IntPtr path, int frameId, int row, int column, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSet2DColorNameD(IntPtr path, double frameId, double rowColumnIndex, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetChromaCustomColorAllFrames(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetChromaCustomColorAllFramesName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetChromaCustomColorAllFramesNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetChromaCustomFlag(int animationId, bool flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetChromaCustomFlagName(IntPtr path, bool flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetChromaCustomFlagNameD(IntPtr path, double flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetCurrentFrame(int animationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetCurrentFrameName(IntPtr path, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetCurrentFrameNameD(IntPtr path, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetCustomColorFlag2D(int device, int[] colors);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetDevice(int animationId, int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetEffect(Guid effectId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetEffectCustom1D(int device, int[] colors);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetEffectCustom2D(int device, int[] colors);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSetEffectKeyboardCustom2D(int device, int[] colors, int[] keys);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetIdleAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetIdleAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColor(int animationId, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorAllFrames(int animationId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorAllFramesName(IntPtr path, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyColorAllFramesNameD(IntPtr path, double rzkey, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorAllFramesRGB(int animationId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorAllFramesRGBName(IntPtr path, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyColorAllFramesRGBNameD(IntPtr path, double rzkey, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorName(IntPtr path, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyColorNameD(IntPtr path, double frameId, double rzkey, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyColorRGBName(IntPtr path, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyColorRGBNameD(IntPtr path, double frameId, double rzkey, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyNonZeroColor(int animationId, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyNonZeroColorName(IntPtr path, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyNonZeroColorNameD(IntPtr path, double frameId, double rzkey, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyNonZeroColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyNonZeroColorRGBName(IntPtr path, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyNonZeroColorRGBNameD(IntPtr path, double frameId, double rzkey, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyRowColumnColorName(IntPtr path, int frameId, int row, int column, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorAllFramesName(IntPtr path, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorAllFramesRGB(int animationId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorAllFramesRGBName(IntPtr path, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysColorRGBName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColorAllFramesName(IntPtr path, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColorName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysNonZeroColorRGBName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColor(int animationId, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorAllFrames(int animationId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorAllFramesName(IntPtr path, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorAllFramesRGB(int animationId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorAllFramesRGBName(IntPtr path, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorRGB(int animationId, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeysZeroColorRGBName(IntPtr path, int frameId, int[] rzkeys, int keyCount, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyZeroColor(int animationId, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyZeroColorName(IntPtr path, int frameId, int rzkey, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyZeroColorNameD(IntPtr path, double frameId, double rzkey, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyZeroColorRGB(int animationId, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetKeyZeroColorRGBName(IntPtr path, int frameId, int rzkey, int red, int green, int blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSetKeyZeroColorRGBNameD(IntPtr path, double frameId, double rzkey, double red, double green, double blue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetLogDelegate(IntPtr fp);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetStaticColor(int deviceType, int device, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSetStaticColorAll(int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStaticColor(int deviceType, int device, int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStaticColorAll(int color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginStaticColorD(double deviceType, double device, double color);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStopAll();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginStopAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginStopAnimationD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStopAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginStopAnimationNameD(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStopAnimationType(int deviceType, int device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginStopAnimationTypeD(double deviceType, double device);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginStopComposite(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginStopCompositeD(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginSubtractColor(int color1, int color2);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeys(int sourceAnimationId, int targetAnimationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysAllFrames(int sourceAnimationId, int targetAnimationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysAllFramesName(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroTargetAllKeysAllFramesNameD(IntPtr sourceAnimation, IntPtr targetAnimation);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysAllFramesOffset(int sourceAnimationId, int targetAnimationId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysAllFramesOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroTargetAllKeysAllFramesOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysOffset(int sourceAnimationId, int targetAnimationId, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractNonZeroTargetAllKeysOffsetName(IntPtr sourceAnimation, IntPtr targetAnimation, int frameId, int offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractNonZeroTargetAllKeysOffsetNameD(IntPtr sourceAnimation, IntPtr targetAnimation, double frameId, double offset);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractThresholdColorsMinMaxAllFramesRGB(int animationId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractThresholdColorsMinMaxAllFramesRGBName(IntPtr path, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractThresholdColorsMinMaxAllFramesRGBNameD(IntPtr path, double minThreshold, double minRed, double minGreen, double minBlue, double maxThreshold, double maxRed, double maxGreen, double maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractThresholdColorsMinMaxRGB(int animationId, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginSubtractThresholdColorsMinMaxRGBName(IntPtr path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginSubtractThresholdColorsMinMaxRGBNameD(IntPtr path, int frameId, int minThreshold, int minRed, int minGreen, int minBlue, int maxThreshold, int maxRed, int maxGreen, int maxBlue);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimEndFrames(int animationId, int lastFrameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimEndFramesName(IntPtr path, int lastFrameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginTrimEndFramesNameD(IntPtr path, double lastFrameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimFrame(int animationId, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimFrameName(IntPtr path, int frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginTrimFrameNameD(IntPtr path, double frameId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimStartFrames(int animationId, int numberOfFrames);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginTrimStartFramesName(IntPtr path, int numberOfFrames);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginTrimStartFramesNameD(IntPtr path, double numberOfFrames);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginUninit();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginUninitD();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginUnloadAnimation(int animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern double PluginUnloadAnimationD(double animationId);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUnloadAnimationName(IntPtr path);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUnloadComposite(IntPtr name);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUnloadLibrarySDK();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUnloadLibraryStreamingPlugin();

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginUpdateFrame(int animationId, int frameIndex, float duration, int[] colors, int length, int[] keys, int keysLength);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PluginUpdateFrameName(IntPtr path, int frameIndex, float duration, int[] colors, int length, int[] keys, int keysLength);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUseIdleAnimation(int device, bool flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUseIdleAnimations(bool flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUsePreloading(int animationId, bool flag);

		[DllImport("CChromaEditorLibrary64", CallingConvention = CallingConvention.Cdecl)]
		private static extern void PluginUsePreloadingName(IntPtr path, bool flag);

		private const string DLL_NAME = "CChromaEditorLibrary64";

		private static Dictionary<KeyCode, int> _sKeyMapping = null;

		public static string _sStreamingAssetPath = string.Empty;

		public enum DeviceType
		{
			Invalid = -1,
			DE_1D,
			DE_2D,
			MAX
		}

		public enum Device
		{
			Invalid = -1,
			ChromaLink,
			Headset,
			Keyboard,
			Keypad,
			Mouse,
			Mousepad,
			KeyboardExtended,
			MAX
		}

		public enum Device1D
		{
			Invalid = -1,
			ChromaLink,
			Headset,
			Mousepad,
			MAX
		}

		public enum Device2D
		{
			Invalid = -1,
			Keyboard,
			Keypad,
			Mouse,
			KeyboardExtended,
			MAX
		}

		public class FChromaSDKDeviceFrameIndex
		{
			public FChromaSDKDeviceFrameIndex()
			{
				this._mFrameIndex[0] = 0;
				this._mFrameIndex[1] = 0;
				this._mFrameIndex[2] = 0;
				this._mFrameIndex[3] = 0;
				this._mFrameIndex[4] = 0;
				this._mFrameIndex[5] = 0;
				this._mFrameIndex[6] = 0;
			}

			public int[] _mFrameIndex = new int[7];
		}

		public enum EChromaSDKSceneBlend
		{
			SB_None,
			SB_Invert,
			SB_Threshold,
			SB_Lerp
		}

		public enum EChromaSDKSceneMode
		{
			SM_Replace,
			SM_Max,
			SM_Min,
			SM_Average,
			SM_Multiply,
			SM_Add,
			SM_Subtract
		}

		public class FChromaSDKSceneEffect
		{
			public string _mAnimation = "";

			public bool _mState;

			public int _mPrimaryColor;

			public int _mSecondaryColor;

			public int _mSpeed = 1;

			public ChromaAnimationAPI.EChromaSDKSceneBlend _mBlend;

			public ChromaAnimationAPI.EChromaSDKSceneMode _mMode;

			public ChromaAnimationAPI.FChromaSDKDeviceFrameIndex _mFrameIndex = new ChromaAnimationAPI.FChromaSDKDeviceFrameIndex();
		}

		public class FChromaSDKScene
		{
			public bool GetState(int effect)
			{
				return effect >= 0 && effect < this._mEffects.Count && this._mEffects[effect]._mState;
			}

			public void ToggleState(int effect)
			{
				if (effect >= 0 && effect < this._mEffects.Count)
				{
					this._mEffects[effect]._mState = !this._mEffects[effect]._mState;
				}
			}

			public List<ChromaAnimationAPI.FChromaSDKSceneEffect> _mEffects = new List<ChromaAnimationAPI.FChromaSDKSceneEffect>();
		}
	}
}
