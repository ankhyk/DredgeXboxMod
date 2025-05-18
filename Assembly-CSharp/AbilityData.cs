using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Dredge/AbilityData", order = 0)]
public class AbilityData : ScriptableObject
{
	[SerializeField]
	public LocalizedString nameKey;

	[SerializeField]
	public LocalizedString descriptionKey;

	[SerializeField]
	public LocalizedString shortDescriptionKey;

	[SerializeField]
	public AbilityData linkedAdvancedVersion;

	[SerializeField]
	public VibrationData primaryVibration;

	[SerializeField]
	public VibrationData secondaryVibration;

	[SerializeField]
	public AssetReference castSFX;

	[SerializeField]
	public AssetReference deactivateSFX;

	[SerializeField]
	public bool showUnseenNotification;

	[SerializeField]
	public Sprite icon;

	[Header("Dependent Item Configuration")]
	[SerializeField]
	public ItemData[] linkedItems;

	[SerializeField]
	public ItemSubtype linkedItemSubtype;

	[SerializeField]
	public bool allowDamagedItems;

	[SerializeField]
	public bool allowExhaustedItems;

	[Header("Timings")]
	[SerializeField]
	public float duration;

	[SerializeField]
	public float cooldown;

	[SerializeField]
	public float castTime;

	[Header("Behaviour")]
	[SerializeField]
	public bool isContinuous;

	[SerializeField]
	public bool deactivateOnInputLayerChanged;

	[SerializeField]
	public bool requiresAbilityFocus;

	[SerializeField]
	public bool persistAbilityToggle;

	[SerializeField]
	public bool canFailCast;

	[SerializeField]
	public bool showsCounter;

	[SerializeField]
	public bool allowExitAction;

	[SerializeField]
	public bool allowItemCycling;

	[SerializeField]
	public ActionLayer exitActionLayer;

	[SerializeField]
	public float sfxRepeatThreshold;
}
