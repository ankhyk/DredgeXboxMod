using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildInfo", menuName = "Dredge/BuildInfo", order = 0)]
public class BuildInfo : ScriptableObject
{
	public int VersionMajor;

	public int VersionMinor;

	public int VersionRevision;

	public string BuildNumber;

	[Header("Feature Toggles")]
	public bool advancedMap;

	public bool photoMode;
}
