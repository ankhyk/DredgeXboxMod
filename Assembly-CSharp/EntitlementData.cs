using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EntitlementData", menuName = "Dredge/EntitlementData", order = 0)]
public class EntitlementData : ScriptableObject
{
	public Entitlement entitlement;

	public int bypassId;

	public int steamId;

	public int gogId;

	public int switchDLCIndex;

	public string gameCoreStoreID;

	public string PS4EntitlementLabel;

	[SerializeField]
	private string EU_PS5EntitlementLabel;

	[SerializeField]
	private string US_PS5EntitlementLabel;
}
