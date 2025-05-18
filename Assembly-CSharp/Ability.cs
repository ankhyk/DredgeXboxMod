using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class Ability : MonoBehaviour
{
	public AbilityData AbilityData
	{
		get
		{
			return this.abilityData;
		}
	}

	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
	}

	public DredgePlayerActionPress CycleItemPrevAction
	{
		get
		{
			return this.cycleItemPrevAction;
		}
	}

	public DredgePlayerActionPress CycleItemNextAction
	{
		get
		{
			return this.cycleItemNextAction;
		}
	}

	public SpatialItemData CurrentlySelectedItem
	{
		get
		{
			return this.currentlySelectedItem;
		}
	}

	public List<SpatialItemData> UniqueItemDatasUsedByAbility
	{
		get
		{
			return this.uniqueItemDatasUsedByAbility;
		}
	}

	protected void Awake()
	{
		GameManager.Instance.PlayerAbilities.RegisterAbility(this.abilityData, this);
		if (this.abilityData.linkedAdvancedVersion)
		{
			GameManager.Instance.PlayerAbilities.RegisterAbility(this.abilityData.linkedAdvancedVersion, this);
		}
		GameEvents.Instance.OnPlayerAbilitySelected += this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
	}

	public bool GetOwnsAdvancedVersion()
	{
		return this.abilityData != null && this.abilityData.linkedAdvancedVersion != null && GameManager.Instance.SaveData.unlockedAbilities.Contains(this.abilityData.linkedAdvancedVersion.name);
	}

	public AssetReference GetActivateSFX()
	{
		if (!this.GetOwnsAdvancedVersion())
		{
			return this.abilityData.castSFX;
		}
		return this.abilityData.linkedAdvancedVersion.castSFX;
	}

	public AssetReference GetDeactivateSFX()
	{
		if (!this.GetOwnsAdvancedVersion())
		{
			return this.abilityData.deactivateSFX;
		}
		return this.abilityData.linkedAdvancedVersion.deactivateSFX;
	}

	private void Start()
	{
		this.Init();
	}

	private void OnDestroy()
	{
		this.isActive = false;
		ApplicationEvents.Instance.OnInputActionLayerChanged -= this.OnInputActionLayerChanged;
		GameEvents.Instance.OnRadialMenuShowingToggled -= this.OnRadialMenuShowingToggled;
		GameEvents.Instance.OnPlayerAbilitySelected -= this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
	}

	public virtual void Init()
	{
		if (this.abilityData.deactivateOnInputLayerChanged)
		{
			ApplicationEvents.Instance.OnInputActionLayerChanged += this.OnInputActionLayerChanged;
			GameEvents.Instance.OnRadialMenuShowingToggled += this.OnRadialMenuShowingToggled;
		}
		if (this.abilityData.persistAbilityToggle)
		{
			bool flag = false;
			GameManager.Instance.SaveData.abilityToggleStates.TryGetValue(this.abilityData.name, out flag);
			this.isActive = !flag;
			if (flag)
			{
				this.Activate();
			}
			else
			{
				this.isActive = false;
			}
		}
		if (this.abilityData.allowExitAction)
		{
			this.exitAbilityAction = new DredgePlayerActionPress("prompt.back", GameManager.Instance.Input.Controls.GetPlayerAction(DredgeControlEnum.BACK));
			DredgePlayerActionPress dredgePlayerActionPress = this.exitAbilityAction;
			dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.Deactivate));
			this.exitAbilityAction.showInControlArea = true;
		}
		if (this.abilityData.allowItemCycling)
		{
			this.cycleItemPrevAction = new DredgePlayerActionPress("prompt.prev", GameManager.Instance.Input.Controls.GetPlayerAction(DredgeControlEnum.CYCLE_ABILITY_PREV));
			DredgePlayerActionPress dredgePlayerActionPress2 = this.cycleItemPrevAction;
			dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.CycleItemPrev));
			this.cycleItemPrevAction.showInControlArea = false;
			this.cycleItemNextAction = new DredgePlayerActionPress("prompt.next", GameManager.Instance.Input.Controls.GetPlayerAction(DredgeControlEnum.CYCLE_ABILITY_NEXT));
			DredgePlayerActionPress dredgePlayerActionPress3 = this.cycleItemNextAction;
			dredgePlayerActionPress3.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressComplete, new Action(this.CycleItemNext));
			this.cycleItemNextAction.showInControlArea = false;
		}
		if (this.abilityData.name == GameManager.Instance.PlayerAbilities.CurrentlySelectedAbilityData.name)
		{
			this.OnPlayerAbilitySelected(GameManager.Instance.PlayerAbilities.CurrentlySelectedAbilityData);
		}
		this.OnItemInventoryChanged(null);
		this.isInit = true;
	}

	private void OnPlayerAbilitySelected(AbilityData selectedAbilityData)
	{
		if (selectedAbilityData != null && selectedAbilityData.name == this.abilityData.name)
		{
			if (!this.isSelected)
			{
				this.isSelected = true;
				if (this.abilityData.allowItemCycling)
				{
					DredgeInputManager input = GameManager.Instance.Input;
					DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.cycleItemPrevAction, this.cycleItemNextAction };
					input.AddActionListener(array, ActionLayer.BASE);
					return;
				}
			}
		}
		else if (this.isSelected)
		{
			this.isSelected = false;
			if (this.abilityData.allowItemCycling)
			{
				DredgeInputManager input2 = GameManager.Instance.Input;
				DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.cycleItemPrevAction, this.cycleItemNextAction };
				input2.RemoveActionListener(array, ActionLayer.BASE);
			}
		}
	}

	protected virtual void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
	}

	protected void CycleItemPrev()
	{
		this.CycleItem(-1);
	}

	protected void CycleItemNext()
	{
		this.CycleItem(1);
	}

	protected void CycleItem(int direction)
	{
		if (direction != 0)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.swapItemSFXAssetReference, AudioLayer.SFX_UI, 1f, 1f);
		}
		if (this.uniqueItemDatasUsedByAbility.Count == 0)
		{
			this.currentlySelectedItem = null;
		}
		else
		{
			this.currentlySelectedItemIndex += direction;
			this.currentlySelectedItemIndex = MathUtil.NegativeMod(this.currentlySelectedItemIndex, this.uniqueItemDatasUsedByAbility.Count);
			SpatialItemData spatialItemData = this.uniqueItemDatasUsedByAbility[this.currentlySelectedItemIndex];
			this.currentlySelectedItem = spatialItemData;
		}
		if (this.abilityData.name == GameManager.Instance.PlayerAbilities.CurrentlySelectedAbilityData.name)
		{
			GameEvents.Instance.TriggerAbilityItemCycled(this.currentlySelectedItem, this.uniqueItemDatasUsedByAbility.Count);
		}
	}

	protected virtual void RefreshItemCyclingCollection()
	{
	}

	public virtual int GetItemCount()
	{
		return 0;
	}

	public virtual bool Activate()
	{
		if (this.isActive)
		{
			GameEvents.Instance.TogglePlayerAbility(this.abilityData, true);
			if (this.abilityData.persistAbilityToggle)
			{
				this.SaveAbilityState(this.isActive);
			}
			if (Time.time > this.timeOfLastRelease + this.abilityData.sfxRepeatThreshold)
			{
				GameManager.Instance.AudioPlayer.PlaySFX(this.GetActivateSFX(), AudioLayer.SFX_PLAYER, 1f, 1f);
			}
			if (this.abilityData.allowExitAction)
			{
				DredgeInputManager input = GameManager.Instance.Input;
				DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.exitAbilityAction };
				input.AddActionListener(array, this.abilityData.exitActionLayer);
			}
		}
		return this.isActive;
	}

	public virtual void Deactivate()
	{
		this.isActive = false;
		this.timeOfLastRelease = Time.time;
		GameEvents.Instance.TogglePlayerAbility(this.abilityData, false);
		if (this.isInit)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.GetDeactivateSFX(), AudioLayer.SFX_PLAYER, 1f, 1f);
		}
		if (this.abilityData.persistAbilityToggle)
		{
			this.SaveAbilityState(this.isActive);
		}
		if (this.abilityData.allowExitAction)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.exitAbilityAction };
			input.RemoveActionListener(array, this.abilityData.exitActionLayer);
		}
		this.continueVibrating = false;
	}

	private void SaveAbilityState(bool state)
	{
		if (GameManager.Instance.SaveData.abilityToggleStates.ContainsKey(this.abilityData.name))
		{
			GameManager.Instance.SaveData.abilityToggleStates[this.abilityData.name] = state;
			return;
		}
		GameManager.Instance.SaveData.abilityToggleStates.Add(this.abilityData.name, state);
	}

	private void OnInputActionLayerChanged(ActionLayer newActionLayer)
	{
		if (this.isActive && newActionLayer != ActionLayer.BASE)
		{
			this.Deactivate();
		}
	}

	private void OnRadialMenuShowingToggled(bool showing)
	{
		if (this.isActive && showing)
		{
			this.Deactivate();
		}
	}

	[SerializeField]
	protected AbilityData abilityData;

	[SerializeField]
	private AssetReference swapItemSFXAssetReference;

	public Action<int> ItemCountChanged;

	protected bool isActive;

	protected bool isInit;

	public bool Locked;

	private DredgePlayerActionPress exitAbilityAction;

	private DredgePlayerActionPress cycleItemPrevAction;

	private DredgePlayerActionPress cycleItemNextAction;

	private bool continueVibrating;

	private float timeOfLastRelease;

	private bool isSelected;

	protected List<SpatialItemData> uniqueItemDatasUsedByAbility = new List<SpatialItemData>();

	protected SpatialItemData currentlySelectedItem;

	private int currentlySelectedItemIndex;
}
