using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class AbilityRadial : MonoBehaviour
{
	private void Awake()
	{
		this.numAbilitiesEnabled = 10;
		if (GameManager.Instance.BuildInfo && GameManager.Instance.BuildInfo.photoMode)
		{
			this.numAbilitiesEnabled = 11;
		}
		this.wedgeWidthHelper = 360f / (float)this.numAbilitiesEnabled;
		this.halfWedgeWidthHelper = this.wedgeWidthHelper * 0.5f;
		this.selectionWedgeImage.fillAmount = 1f / (float)this.numAbilitiesEnabled;
		this.showRadialAction = new DredgePlayerActionPress("prompt.radial-show", GameManager.Instance.Input.Controls.RadialSelectShow);
		DredgePlayerActionPress dredgePlayerActionPress = this.showRadialAction;
		dredgePlayerActionPress.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressBegin, new Action(this.OnShowPressBegin));
		DredgePlayerActionPress dredgePlayerActionPress2 = this.showRadialAction;
		dredgePlayerActionPress2.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressEnd, new Action(this.OnShowPressEnd));
		this.confirmSelectionAction = new DredgePlayerActionPress("prompt.confirm", GameManager.Instance.Input.Controls.Confirm);
		DredgePlayerActionPress dredgePlayerActionPress3 = this.confirmSelectionAction;
		dredgePlayerActionPress3.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressBegin, new Action(this.ToggleRadial));
		for (int i = 0; i < this.abilityWedges.Count; i++)
		{
			this.abilityWedges[i].gameObject.SetActive(i < this.numAbilitiesEnabled);
			this.abilityWedges[i].LayOutButton(this.numAbilitiesEnabled);
		}
	}

	private void Start()
	{
		this.abilityBarUI.Init();
		this.radialPrompt.Init(this.showRadialAction, this.showRadialAction.GetPrimaryPlayerAction());
		string lastSavedAbilityName = GameManager.Instance.SaveData.LastSelectedAbility;
		AbilityRadialWedge abilityRadialWedge = this.abilityWedges.Find((AbilityRadialWedge w) => w.AbilityData.name == lastSavedAbilityName);
		AbilityData abilityData = this.abilityWedges[0].AbilityData;
		if (abilityRadialWedge)
		{
			abilityData = abilityRadialWedge.AbilityData;
		}
		GameEvents.Instance.SelectPlayerAbility(abilityData);
	}

	private void OnEnable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.showRadialAction };
		input.AddActionListener(array, ActionLayer.BASE);
		ApplicationEvents.Instance.OnInputActionLayerChanged += this.OnInputActionLayerChanged;
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.showRadialAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		ApplicationEvents.Instance.OnInputActionLayerChanged -= this.OnInputActionLayerChanged;
		this.HideRadial();
	}

	private void OnInputActionLayerChanged(ActionLayer newActionLayer)
	{
		if (newActionLayer != ActionLayer.BASE)
		{
			this.HideRadial();
		}
	}

	private void OnShowPressBegin()
	{
		if (GameManager.Instance.SettingsSaveData.radialTriggerMode == 0)
		{
			this.ShowRadial();
			return;
		}
		this.ToggleRadial();
	}

	private void OnShowPressEnd()
	{
		if (GameManager.Instance.SettingsSaveData.radialTriggerMode == 0)
		{
			this.HideRadial();
		}
	}

	private void ToggleRadial()
	{
		if (this.isShowing)
		{
			this.HideRadial();
			return;
		}
		this.ShowRadial();
	}

	private void ShowRadial()
	{
		GameEvents.Instance.ToggleRadialMenuShowing(true);
		this.container.SetActive(true);
		this.isShowing = true;
		Time.timeScale = 0.3f;
		if (this.scaleTween != null)
		{
			this.scaleTween.Kill(false);
			this.scaleTween = null;
		}
		Vector3 vector = new Vector3(0f, 0f, 1f);
		(this.container.transform as RectTransform).localScale = vector;
		Vector3 vector2 = new Vector3(1f, 1f, 1f);
		this.scaleTween = (this.container.transform as RectTransform).DOScale(vector2, 0.35f);
		this.scaleTween.SetUpdate(true);
		this.scaleTween.SetEase(Ease.OutExpo);
		this.scaleTween.OnComplete(delegate
		{
			this.scaleTween = null;
		});
		this.mouseDeadzoneSize = (this.cooldownRadialFill.transform as RectTransform).rect.width * 0.5f * GameManager.Instance.ScaleFactor;
		if (!GameManager.Instance.Input.IsUsingController)
		{
			Cursor.visible = true;
		}
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.confirmSelectionAction };
		input.AddActionListener(array, ActionLayer.BASE);
		GameManager.Instance.AudioPlayer.PlaySFX(this.openSFX, AudioLayer.SFX_UI, 1f, 1f);
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.ABILITY_SELECT);
		this.ChangeCurrentIndex(this.currentIndex);
	}

	private void HideRadial()
	{
		if (!GameManager.Instance.UI.IsShowingRadialMenu)
		{
			return;
		}
		GameManager.Instance.Input.RefreshCursorLockState();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.confirmSelectionAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		this.container.SetActive(false);
		this.isShowing = false;
		if (!GameManager.Instance.IsPaused)
		{
			Time.timeScale = 1f;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(this.closeSFX, AudioLayer.SFX_UI, 1f, 1f);
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
		GameEvents.Instance.ToggleRadialMenuShowing(false);
	}

	private void Update()
	{
		if (GameManager.Instance.UI.IsShowingRadialMenu)
		{
			this.angleHelper = 0f;
			this.hasChangedHelper = false;
			if (GameManager.Instance.Input.IsUsingController)
			{
				this.hasChangedHelper = false;
				this.angleHelper = 0f;
				if (GameManager.Instance.Input.Controls.RadialSelect.Vector.magnitude >= this.controllerDeadzoneMagnitude)
				{
					this.hasChangedHelper = true;
					this.angleHelper = GameManager.Instance.Input.Controls.RadialSelect.Angle;
				}
			}
			else
			{
				this.moveInput.x = Input.mousePosition.x - base.transform.position.x;
				this.moveInput.y = Input.mousePosition.y - base.transform.position.y;
				this.moveInput.Normalize();
				if (this.moveInput == Vector2.zero)
				{
					this.hasChangedHelper = false;
				}
				else if (Vector2.Distance(Input.mousePosition, base.transform.position) < this.mouseDeadzoneSize)
				{
					this.hasChangedHelper = false;
				}
				else
				{
					this.angleHelper = Mathf.Atan2(this.moveInput.y, -this.moveInput.x) / 3.1415927f;
					this.angleHelper *= 180f;
					this.angleHelper -= 90f;
					if (this.angleHelper < 0f)
					{
						this.angleHelper += 360f;
					}
					this.hasChangedHelper = true;
				}
			}
			if (this.hasChangedHelper)
			{
				int num = Mathf.RoundToInt((float)MathUtil.RoundToStep(Mathf.RoundToInt(this.angleHelper), Mathf.RoundToInt(this.wedgeWidthHelper)) / this.wedgeWidthHelper);
				num %= this.numAbilitiesEnabled;
				if (num != this.currentIndex)
				{
					GameManager.Instance.AudioPlayer.PlaySFX(this.selectSFX, AudioLayer.SFX_UI, 1f, 1f);
					this.ChangeCurrentIndex(num);
				}
			}
			if (GameManager.Instance.Input.IsUsingController)
			{
				Cursor.visible = false;
			}
		}
	}

	private void ChangeCurrentIndex(int newIndex)
	{
		this.currentIndex = newIndex % this.numAbilitiesEnabled;
		AbilityData abilityData = this.abilityWedges[this.currentIndex].AbilityData;
		if (GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked(abilityData))
		{
			if (abilityData.linkedAdvancedVersion && GameManager.Instance.SaveData.unlockedAbilities.Contains(abilityData.linkedAdvancedVersion.name))
			{
				GameManager.Instance.SaveData.SetAbilityUnseen(abilityData.linkedAdvancedVersion, false);
			}
			else
			{
				GameManager.Instance.SaveData.SetAbilityUnseen(abilityData, false);
			}
			GameEvents.Instance.SelectPlayerAbility(abilityData);
			if (abilityData.linkedAdvancedVersion && GameManager.Instance.SaveData.unlockedAbilities.Contains(abilityData.linkedAdvancedVersion.name))
			{
				this.localizedTitleField.StringReference = abilityData.linkedAdvancedVersion.nameKey;
				this.localizedDescriptionField.StringReference = abilityData.linkedAdvancedVersion.descriptionKey;
			}
			else
			{
				this.localizedTitleField.StringReference = abilityData.nameKey;
				this.localizedDescriptionField.StringReference = abilityData.descriptionKey;
			}
			bool flag;
			if (abilityData.cooldown > 0f)
			{
				float timeSinceLastCast = GameManager.Instance.PlayerAbilities.GetTimeSinceLastCast(abilityData);
				this.cooldownRadialFill.fillAmount = 1f - timeSinceLastCast / abilityData.cooldown;
				if (timeSinceLastCast > abilityData.cooldown)
				{
					flag = true;
					this.invalidAbilityTextField.gameObject.SetActive(false);
					this.cooldownRadialFill.color = Color.black;
				}
				else
				{
					flag = false;
					this.invalidAbilityLocalizedString.StringReference = this.onCooldownKey;
					this.invalidAbilityTextField.gameObject.SetActive(true);
					this.invalidAbilityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
					this.cooldownRadialFill.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
				}
			}
			else
			{
				flag = true;
				this.invalidAbilityTextField.gameObject.SetActive(false);
				this.cooldownRadialFill.color = Color.black;
			}
			if (flag)
			{
				if (GameManager.Instance.PlayerAbilities.GetHasDependantItems(abilityData))
				{
					this.invalidAbilityTextField.gameObject.SetActive(false);
					this.cooldownRadialFill.color = Color.black;
				}
				else
				{
					this.invalidAbilityLocalizedString.StringReference = this.missingEquipmentKey;
					this.invalidAbilityTextField.gameObject.SetActive(true);
					this.invalidAbilityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
					this.cooldownRadialFill.fillAmount = 1f;
					this.cooldownRadialFill.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
				}
			}
		}
		else
		{
			this.localizedTitleField.StringReference = this.unknownAbilityNameKey;
			this.localizedDescriptionField.StringReference = this.unknownAbilityDescriptionKey;
			this.invalidAbilityTextField.gameObject.SetActive(false);
			this.cooldownRadialFill.color = Color.black;
		}
		this.abilityWedges[this.prevIndex].SetHighlighted(false);
		this.prevIndex = this.currentIndex;
		this.abilityWedges[this.currentIndex].SetHighlighted(true);
		Vector3 eulerAngles = this.selectionWedge.transform.rotation.eulerAngles;
		eulerAngles.z = (float)(this.numAbilitiesEnabled - this.currentIndex + 1) * this.wedgeWidthHelper - this.halfWedgeWidthHelper;
		this.selectionWedge.transform.eulerAngles = eulerAngles;
	}

	[SerializeField]
	private AbilityBarUI abilityBarUI;

	[SerializeField]
	private GameObject selectionWedge;

	[SerializeField]
	private Image selectionWedgeImage;

	[SerializeField]
	private List<AbilityRadialWedge> abilityWedges = new List<AbilityRadialWedge>();

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private LocalizeStringEvent localizedTitleField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	[SerializeField]
	private ControlPromptIcon radialPrompt;

	[SerializeField]
	private LocalizedString unknownAbilityNameKey;

	[SerializeField]
	private LocalizedString unknownAbilityDescriptionKey;

	[SerializeField]
	private LocalizedString onCooldownKey;

	[SerializeField]
	private LocalizedString missingEquipmentKey;

	[SerializeField]
	private Image cooldownRadialFill;

	[SerializeField]
	private LocalizeStringEvent invalidAbilityLocalizedString;

	[SerializeField]
	private TextMeshProUGUI invalidAbilityTextField;

	[SerializeField]
	private AssetReference openSFX;

	[SerializeField]
	private AssetReference closeSFX;

	[SerializeField]
	private AssetReference selectSFX;

	[SerializeField]
	private float controllerDeadzoneMagnitude;

	private float mouseDeadzoneSize;

	private DredgePlayerActionPress showRadialAction;

	private DredgePlayerActionPress confirmSelectionAction;

	private int prevIndex;

	private int currentIndex;

	private Vector2 moveInput;

	private int numAbilitiesEnabled;

	private float wedgeWidthHelper;

	private float halfWedgeWidthHelper;

	private bool hasChangedHelper;

	private float angleHelper;

	private Tweener scaleTween;

	private bool isShowing;
}
