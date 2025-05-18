using System;

[Flags]
[Serializable]
public enum TutorialStepViewEnum
{
	NONE = 0,
	DOCKED = 1,
	POPUP = 2,
	INVENTORY = 4,
	DIALOGUE = 8,
	RADIAL = 16,
	ALL = -1
}
