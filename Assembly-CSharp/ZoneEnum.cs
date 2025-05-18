using System;

[Flags]
[Serializable]
public enum ZoneEnum
{
	NONE = 0,
	THE_MARROWS = 1,
	GALE_CLIFFS = 2,
	STELLAR_BASIN = 4,
	TWISTED_STRAND = 8,
	DEVILS_SPINE = 16,
	OPEN_OCEAN = 32,
	PALE_REACH = 64,
	ALL = -1
}
