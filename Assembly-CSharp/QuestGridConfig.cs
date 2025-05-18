using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "QuestGridConfig", menuName = "Dredge/QuestGridConfig", order = 0)]
public class QuestGridConfig : SerializedScriptableObject
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(CompletedGridCondition));
	}

	[SerializeField]
	public QuestGridExitMode questGridExitMode;

	[SerializeField]
	public LocalizedString titleString;

	[SerializeField]
	public LocalizedString helpStringOverride;

	[SerializeField]
	public LocalizedString exitPromptOverride;

	[SerializeField]
	public Sprite backgroundImage;

	[SerializeField]
	public float gridHeightOverride;

	[SerializeField]
	public bool overrideGridCellColor;

	[SerializeField]
	public DredgeColorTypeEnum gridCellColor;

	[SerializeField]
	public bool allowStorageAccess;

	[SerializeField]
	public bool isSaved;

	[SerializeField]
	public bool createItemsIfEmpty;

	[SerializeField]
	public GridKey gridKey;

	[SerializeField]
	public bool allowManualExit;

	[SerializeField]
	public bool allowEquipmentInstallation;

	[SerializeField]
	public bool createWithDurabilityValue;

	[SerializeField]
	public float startingDurabilityProportion = 1f;

	[SerializeField]
	public GridConfiguration gridConfiguration;

	[SerializeField]
	public SerializableGrid presetGrid;

	[SerializeField]
	public PresetGridMode presetGridMode;

	[SerializeField]
	public List<CompletedGridCondition> completeConditions = new List<CompletedGridCondition>();
}
