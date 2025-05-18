using System;

[Flags]
[Serializable]
public enum ItemType
{
	NONE = 0,
	GENERAL = 1,
	EQUIPMENT = 2,
	DAMAGE = 4,
	ALL = -1
}
