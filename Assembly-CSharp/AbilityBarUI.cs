using System;
using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{
	private bool IsCastingForbidden { get; set; }

	public AbilityData CurrentAbilityData
	{
		get
		{
			return this.currentAbilityData;
		}
	}

	private void Awake()
	{
		this.container.SetActive(false);
		this.isShowing = false;
	}

	public void Init()
	{
		this.currentAbilityActionPress = new DredgePlayerActionPress("prompt.action", GameManager.Instance.Input.Controls.DoAbility);
		DredgePlayerActionPress dredgePlayerActionPress = this.currentAbilityActionPress;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnCurrentPressComplete));
		this.currentAbilityActionHold = new DredgePlayerActionHold("prompt.action", GameManager.Instance.Input.Controls.DoAbility, 1f);
		DredgePlayerActionHold dredgePlayerActionHold = this.currentAbilityActionHold;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnCurrentPressComplete));
		this.currentAbilityActionHoldDelegate = new DredgePlayerActionHoldDelegate("prompt.action", GameManager.Instance.Input.Controls.DoAbility);
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate = this.currentAbilityActionHoldDelegate;
		dredgePlayerActionHoldDelegate.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate.OnPressBegin, new Action(this.OnCurrentPressBegin));
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate2 = this.currentAbilityActionHoldDelegate;
		dredgePlayerActionHoldDelegate2.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate2.OnPressEnd, new Action(this.OnCurrentPressEnd));
		this.attentionCallout.SetActive(GameManager.Instance.PlayerAbilities.GetHasAnyUnseenAbilities());
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilitySelected += this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
		GameEvents.Instance.OnRadialMenuShowingToggled += this.OnRadialMenuShowingToggled;
		GameEvents.Instance.OnFinaleCutsceneStarted += this.OnFinaleCutsceneStarted;
		ApplicationEvents.Instance.OnInputActionLayerChanged += this.OnInputActionLayerChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilitySelected -= this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
		GameEvents.Instance.OnRadialMenuShowingToggled -= this.OnRadialMenuShowingToggled;
		GameEvents.Instance.OnFinaleCutsceneStarted -= this.OnFinaleCutsceneStarted;
		ApplicationEvents.Instance.OnInputActionLayerChanged -= this.OnInputActionLayerChanged;
	}

	private void Update()
	{
		if (Time.time > this.timeOfLastRefresh + this.refreshDelaySec)
		{
			this.timeOfLastRefresh = Time.time;
			this.CheckAbilityCooldown(true);
		}
	}

	private void OnFinaleCutsceneStarted()
	{
		this.IsCastingForbidden = true;
	}

	private void OnRadialMenuShowingToggled(bool showing)
	{
		this.currentPrompt.gameObject.SetActive(!showing);
	}

	public void OnPlayerAbilitySelected(AbilityData abilityData)
	{
		this.RefreshAttentionCallout();
		this.currentAbilityData = abilityData;
		this.timeOfLastRefresh = float.NegativeInfinity;
		this.currentAbilityIcon.SetAbility(this.currentAbilityData);
		if (this.currentAbilityData.isContinuous)
		{
			this.currentPrompt.Init(this.currentAbilityActionHoldDelegate, this.currentAbilityActionHoldDelegate.GetPrimaryPlayerAction());
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionHold, this.currentAbilityActionPress }, ActionLayer.BASE);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionHoldDelegate }, ActionLayer.BASE);
		}
		else if (this.currentAbilityData.castTime > 0f)
		{
			this.currentAbilityActionHold.holdTimeRequiredSec = this.currentAbilityData.castTime;
			this.currentPrompt.Init(this.currentAbilityActionHold, this.currentAbilityActionHold.GetPrimaryPlayerAction());
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionHoldDelegate, this.currentAbilityActionPress }, ActionLayer.BASE);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionHold }, ActionLayer.BASE);
		}
		else
		{
			this.currentPrompt.Init(this.currentAbilityActionPress, this.currentAbilityActionPress.GetPrimaryPlayerAction());
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionHoldDelegate, this.currentAbilityActionHold }, ActionLayer.BASE);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.currentAbilityActionPress }, ActionLayer.BASE);
		}
		GameManager.Instance.SaveData.LastSelectedAbility = abilityData.name;
		this.CheckAbilityItems(true);
		this.timeOfLastRefresh = float.NegativeInfinity;
	}

	private void RefreshAttentionCallout()
	{
		this.attentionCallout.SetActive(GameManager.Instance.PlayerAbilities.GetHasAnyUnseenAbilities());
	}

	private void OnItemsRepaired()
	{
		this.CheckAbilityItems(true);
	}

	private void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		this.CheckAbilityItems(true);
	}

	private void OnInputActionLayerChanged(ActionLayer newActionLayer)
	{
		if (this.currentAbilityData == null || (GameManager.Instance.Player && GameManager.Instance.Player.IsDocked) || GameManager.Instance.Input.GetActiveActionLayer() != ActionLayer.BASE)
		{
			if (this.isShowing)
			{
				this.isShowing = false;
				this.container.SetActive(false);
				GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.GetCurrentAbilityAction() }, ActionLayer.BASE);
			}
		}
		else if (!this.isShowing)
		{
			this.isShowing = true;
			this.container.SetActive(true);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.GetCurrentAbilityAction() }, ActionLayer.BASE);
		}
		this.RefreshAttentionCallout();
	}

	private void CheckAbilityCooldown(bool refresh)
	{
		if (this.currentAbilityData != null)
		{
			float num = 1f;
			if (this.currentAbilityData.cooldown > 0f)
			{
				this.timeSinceAbilityLastCastHelper = GameManager.Instance.PlayerAbilities.GetTimeSinceLastCast(this.currentAbilityData);
				if (this.timeSinceAbilityLastCastHelper > this.currentAbilityData.cooldown)
				{
					this.isCurrentAbilityOffCooldown = true;
				}
				else
				{
					num = this.timeSinceAbilityLastCastHelper / this.currentAbilityData.cooldown;
					this.isCurrentAbilityOffCooldown = false;
				}
			}
			else
			{
				this.isCurrentAbilityOffCooldown = true;
			}
			this.currentAbilityIcon.UpdateCooldownFill(num);
			if (refresh)
			{
				this.RefreshAbilityAvailability();
			}
		}
	}

	private void CheckAbilityItems(bool refresh)
	{
		if (this.currentAbilityData != null)
		{
			this.hasRequiredItems = GameManager.Instance.PlayerAbilities.GetHasDependantItems(this.currentAbilityData);
			if (refresh)
			{
				this.RefreshAbilityAvailability();
			}
		}
	}

	private void RefreshAbilityAvailability()
	{
		if (this.currentAbilityData != null)
		{
			if (this.hasRequiredItems && this.isCurrentAbilityOffCooldown)
			{
				this.GetCurrentAbilityAction().Enable();
			}
			else
			{
				this.GetCurrentAbilityAction().Disable(true);
			}
			this.currentAbilityIcon.SetDisabledEntirely(!this.hasRequiredItems);
		}
	}

	private bool VerifyAbilityCast()
	{
		bool flag = true;
		this.timeSinceAbilityLastCastHelper = GameManager.Instance.PlayerAbilities.GetTimeSinceLastCast(this.currentAbilityData);
		if (this.IsCastingForbidden)
		{
			Debug.LogWarning(string.Format("[AbilityBarUI] OnCurrentPressComplete(): attempted to cast {0}, but casting abilities is forbidden. Ignoring.", this.currentAbilityData));
			flag = false;
		}
		if (this.timeSinceAbilityLastCastHelper < this.currentAbilityData.cooldown)
		{
			Debug.LogWarning(string.Format("[AbilityBarUI] OnCurrentPressComplete(): attempted to cast {0}, but we think it's on cooldown. Ignoring.", this.currentAbilityData));
			flag = false;
		}
		if (!this.hasRequiredItems)
		{
			Debug.LogWarning(string.Format("[AbilityBarUI] OnCurrentPressComplete(): attempted to cast {0}, but we think it doesn't have its requirements met. Ignoring.", this.currentAbilityData));
			flag = false;
		}
		if (GameManager.Instance.UI.IsShowingRadialMenu)
		{
			Debug.LogWarning(string.Format("[AbilityBarUI] OnCurrentPressComplete(): attempted to cast {0}, but radial is showing. Ignoring.", this.currentAbilityData));
			flag = false;
		}
		return flag;
	}

	private DredgePlayerActionBase GetCurrentAbilityAction()
	{
		if (this.currentAbilityData.isContinuous)
		{
			return this.currentAbilityActionHoldDelegate;
		}
		if (this.currentAbilityData.castTime > 0f)
		{
			return this.currentAbilityActionHold;
		}
		return this.currentAbilityActionPress;
	}

	private void OnCurrentPressComplete()
	{
		if (!this.VerifyAbilityCast())
		{
			return;
		}
		if (!GameManager.Instance.PlayerAbilities.ActivateAbility(this.currentAbilityData))
		{
			if (this.currentAbilityData.canFailCast)
			{
				this.GetCurrentAbilityAction().Disable(true);
				this.GetCurrentAbilityAction().Reset();
			}
			return;
		}
		if (this.currentAbilityData.cooldown > 0f)
		{
			this.GetCurrentAbilityAction().Disable(true);
		}
		string text = this.currentAbilityData.name.ToLowerInvariant();
		if (GameManager.Instance.SaveData.abilityHistory.ContainsKey(text))
		{
			GameManager.Instance.SaveData.abilityHistory[text] = GameManager.Instance.Time.TimeAndDay;
			return;
		}
		GameManager.Instance.SaveData.abilityHistory.Add(text, GameManager.Instance.Time.TimeAndDay);
	}

	private void OnCurrentPressBegin()
	{
		if (!this.VerifyAbilityCast())
		{
			return;
		}
		GameManager.Instance.PlayerAbilities.ActivateAbility(this.currentAbilityData);
	}

	private void OnCurrentPressEnd()
	{
		GameManager.Instance.PlayerAbilities.DeactivateAbility(this.currentAbilityData);
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private AbilityIcon currentAbilityIcon;

	[SerializeField]
	private ControlPromptIcon currentPrompt;

	[SerializeField]
	private float refreshDelaySec;

	[SerializeField]
	private GameObject attentionCallout;

	private DredgePlayerActionPress currentAbilityActionPress;

	private DredgePlayerActionHold currentAbilityActionHold;

	private DredgePlayerActionHoldDelegate currentAbilityActionHoldDelegate;

	private AbilityData currentAbilityData;

	private Ability currentAbility;

	private bool isShowing;

	private float timeOfLastRefresh;

	private bool hasRequiredItems;

	private bool isCurrentAbilityOffCooldown;

	private float timeSinceAbilityLastCastHelper;
}
