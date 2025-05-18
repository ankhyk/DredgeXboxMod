using System;

[Flags]
[Serializable]
public enum ItemSubtype
{
	NONE = 0,
	FISH = 1,
	ENGINE = 2,
	ROD = 4,
	GENERAL = 8,
	RELIC = 16,
	TRINKET = 32,
	MATERIAL = 64,
	LIGHT = 128,
	POT = 256,
	NET = 512,
	DREDGE = 1024,
	GADGET = 2048,
	ALL = -1
}
