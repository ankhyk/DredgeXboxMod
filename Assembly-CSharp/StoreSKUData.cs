using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreSKUData", menuName = "Dredge/StoreSKUData", order = 0)]
public class StoreSKUData : ScriptableObject
{
	public int steamId;

	public int gogId;

	public string switchId;

	public bool switchShowDLC;

	public string xboxStoreID;

	[SerializeField]
	private string EU_PS5StoreId;

	[SerializeField]
	private string US_PS5StoreId;

	[SerializeField]
	private string EU_PS4StoreId;

	[SerializeField]
	private string US_PS4StoreId;
}
