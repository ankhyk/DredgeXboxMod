using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataTemplate", menuName = "Dredge/SaveDataTemplate", order = 0)]
public class SaveDataTemplate : SerializedScriptableObject
{
	[SerializeField]
	public int version;

	[SerializeField]
	public string dockId = "";

	[SerializeField]
	public int dockSlotIndex;

	[SerializeField]
	public Dictionary<string, decimal> decimalVariables = new Dictionary<string, decimal>();

	[SerializeField]
	public Dictionary<string, int> intVariables = new Dictionary<string, int>();

	[SerializeField]
	public Dictionary<string, float> floatVariables = new Dictionary<string, float>();

	[SerializeField]
	public Dictionary<string, string> stringVariables = new Dictionary<string, string>();

	[SerializeField]
	public Dictionary<string, bool> boolVariables = new Dictionary<string, bool>();

	[SerializeField]
	public Dictionary<GridKey, SerializableGrid> grids = new Dictionary<GridKey, SerializableGrid>();

	[SerializeField]
	public List<NonSpatialItemInstance> ownedNonSpatialItems = new List<NonSpatialItemInstance>();

	[SerializeField]
	public HashSet<string> availableDestinations = new HashSet<string>();

	[SerializeField]
	public HashSet<string> availableSpeakers = new HashSet<string>();

	[SerializeField]
	public List<string> unlockedAbilities;

	[SerializeField]
	public Dictionary<string, SerializedQuestEntry> questEntries = new Dictionary<string, SerializedQuestEntry>();

	[SerializeField]
	public List<SerializedCrabPotPOIData> serializedCrabPotPOIs;
}
