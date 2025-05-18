using System;

namespace ChromaSDK
{
	internal class RazerErrors
	{
		public static string GetResultString(int result)
		{
			if (result <= 1062)
			{
				if (result <= 5)
				{
					if (result <= -1)
					{
						if (result == -2147467259)
						{
							return "RZRESULT_FAILED";
						}
						if (result == -1)
						{
							return "RZRESULT_INVALID";
						}
					}
					else
					{
						if (result == 0)
						{
							return "RZRESULT_SUCCESS";
						}
						if (result == 5)
						{
							return "RZRESULT_ACCESS_DENIED";
						}
					}
				}
				else if (result <= 50)
				{
					if (result == 6)
					{
						return "RZRESULT_INVALID_HANDLE";
					}
					if (result == 50)
					{
						return "RZRESULT_NOT_SUPPORTED";
					}
				}
				else
				{
					if (result == 87)
					{
						return "RZRESULT_INVALID_PARAMETER";
					}
					if (result == 259)
					{
						return "RZRESULT_NO_MORE_ITEMS";
					}
					if (result == 1062)
					{
						return "RZRESULT_SERVICE_NOT_ACTIVE";
					}
				}
			}
			else if (result <= 1247)
			{
				if (result <= 1167)
				{
					if (result == 1152)
					{
						return "RZRESULT_SINGLE_INSTANCE_APP";
					}
					if (result == 1167)
					{
						return "RZRESULT_DEVICE_NOT_CONNECTED";
					}
				}
				else
				{
					if (result == 1168)
					{
						return "RZRESULT_NOT_FOUND";
					}
					if (result == 1235)
					{
						return "RZRESULT_REQUEST_ABORTED";
					}
					if (result == 1247)
					{
						return "RZRESULT_ALREADY_INITIALIZED";
					}
				}
			}
			else if (result <= 4319)
			{
				if (result == 4309)
				{
					return "RZRESULT_RESOURCE_DISABLED";
				}
				if (result == 4319)
				{
					return "RZRESULT_DEVICE_NOT_AVAILABLE";
				}
			}
			else
			{
				if (result == 5023)
				{
					return "RZRESULT_NOT_VALID_STATE";
				}
				if (result == 6023)
				{
					return "RZRESULT_DLL_NOT_FOUND";
				}
				if (result == 6033)
				{
					return "RZRESULT_DLL_INVALID_SIGNATURE";
				}
			}
			return result.ToString();
		}

		public const int RZRESULT_INVALID = -1;

		public const int RZRESULT_SUCCESS = 0;

		public const int RZRESULT_ACCESS_DENIED = 5;

		public const int RZRESULT_INVALID_HANDLE = 6;

		public const int RZRESULT_NOT_SUPPORTED = 50;

		public const int RZRESULT_INVALID_PARAMETER = 87;

		public const int RZRESULT_SERVICE_NOT_ACTIVE = 1062;

		public const int RZRESULT_SINGLE_INSTANCE_APP = 1152;

		public const int RZRESULT_DEVICE_NOT_CONNECTED = 1167;

		public const int RZRESULT_NOT_FOUND = 1168;

		public const int RZRESULT_REQUEST_ABORTED = 1235;

		public const int RZRESULT_ALREADY_INITIALIZED = 1247;

		public const int RZRESULT_RESOURCE_DISABLED = 4309;

		public const int RZRESULT_DEVICE_NOT_AVAILABLE = 4319;

		public const int RZRESULT_NOT_VALID_STATE = 5023;

		public const int RZRESULT_NO_MORE_ITEMS = 259;

		public const int RZRESULT_DLL_NOT_FOUND = 6023;

		public const int RZRESULT_DLL_INVALID_SIGNATURE = 6033;

		public const int RZRESULT_FAILED = -2147467259;
	}
}
