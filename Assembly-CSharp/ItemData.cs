using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

public class ItemData : SerializedScriptableObject
{
	[SerializeField]
	public string id;

	[SerializeField]
	public LocalizedString itemNameKey;

	[SerializeField]
	public LocalizedString itemDescriptionKey;

	[SerializeField]
	public bool hasAdditionalNote;

	[SerializeField]
	public LocalizedString additionalNoteKey;

	[SerializeField]
	public LocalizedString itemInsaneTitleKey;

	[SerializeField]
	public LocalizedString itemInsaneDescriptionKey;

	[SerializeField]
	public string linkedDialogueNode;

	[SerializeField]
	public LocalizedString dialogueNodeSpecificDescription;

	[SerializeField]
	public ItemType itemType;

	[SerializeField]
	public ItemSubtype itemSubtype;

	[SerializeField]
	public Color tooltipTextColor;

	[SerializeField]
	public Color tooltipNotesColor = Color.white;

	[SerializeField]
	public Sprite itemTypeIcon;

	[SerializeField]
	public GameObject harvestParticlePrefab;

	[SerializeField]
	public bool overrideHarvestParticleDepth;

	[SerializeField]
	public float harvestParticleDepthOffset = -3f;

	[SerializeField]
	public bool flattenParticleShape;

	[SerializeField]
	public bool availableInDemo;

	[SerializeField]
	public List<Entitlement> entitlementsRequired = new List<Entitlement>();
}
