using System;

[Flags]
[Serializable]
public enum Platform
{
	NONE = 0,
	SWITCH = 1,
	PS4 = 2,
	PS5 = 4,
	XBOX_ONE = 8,
	XBOX_SERIES_X = 16,
	XBOX_SERIES_S = 32,
	PC_STEAM = 64,
	PC_GOG = 128,
	PC_GDK = 256,
	ALL = -1
}
