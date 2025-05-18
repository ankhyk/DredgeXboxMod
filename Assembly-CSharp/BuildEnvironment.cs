using System;

[Flags]
[Serializable]
public enum BuildEnvironment
{
	NONE = 0,
	PROD = 1,
	STAGE = 2,
	DEMO = 4,
	ALL = -1
}
