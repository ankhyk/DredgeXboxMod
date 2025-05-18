using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour
{
	public UpgradeData UpgradeData
	{
		get
		{
			return this.upgradeData;
		}
	}

	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	private void Awake()
	{
		this.upgradeSelectable.upgradeData = this.upgradeData;
		this.upgradeTooltipRequester.upgradeData = this.upgradeData;
	}

	private void OnEnable()
	{
		this.RefreshUI();
		GameEvents.Instance.OnUpgradesChanged += this.RefreshUI;
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
		BasicButtonWrapper basicButtonWrapper2 = this.basicButtonWrapper;
		basicButtonWrapper2.OnSelectAction = (Action)Delegate.Combine(basicButtonWrapper2.OnSelectAction, new Action(this.OnSelect));
		BasicButtonWrapper basicButtonWrapper3 = this.basicButtonWrapper;
		basicButtonWrapper3.OnDeselectAction = (Action)Delegate.Combine(basicButtonWrapper3.OnDeselectAction, new Action(this.OnDeselect));
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnUpgradesChanged -= this.RefreshUI;
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnClick));
		BasicButtonWrapper basicButtonWrapper2 = this.basicButtonWrapper;
		basicButtonWrapper2.OnSelectAction = (Action)Delegate.Remove(basicButtonWrapper2.OnSelectAction, new Action(this.OnSelect));
		BasicButtonWrapper basicButtonWrapper3 = this.basicButtonWrapper;
		basicButtonWrapper3.OnDeselectAction = (Action)Delegate.Remove(basicButtonWrapper3.OnDeselectAction, new Action(this.OnDeselect));
	}

	public void SetInteractable(bool interactable)
	{
		this.interactable = interactable;
		this.basicButtonWrapper.Interactable = interactable;
		this.upgradeTooltipRequester.enabled = interactable;
	}

	public void OnClick()
	{
		if (!this.interactable || !this.areOwningPrerequisitesMet || this.isOwned)
		{
			Debug.LogWarning("[UpgradeNodeUI] OnClick() can't process event: not interactable.");
			return;
		}
		Action<UpgradeNodeUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(this);
	}

	private void OnSelect()
	{
		if (this.canUseHoverActions)
		{
			Action<UpgradeNodeUI> onEntryHovered = this.OnEntryHovered;
			if (onEntryHovered == null)
			{
				return;
			}
			onEntryHovered(this);
		}
	}

	private void OnDeselect()
	{
		if (this.canUseHoverActions)
		{
			Action<UpgradeNodeUI> onEntryUnhovered = this.OnEntryUnhovered;
			if (onEntryUnhovered == null)
			{
				return;
			}
			onEntryUnhovered(this);
		}
	}

	private void RefreshUI(UpgradeData upgradeData)
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.iconImage.sprite = this.upgradeData.sprite;
		this.titleText.text = string.Format("+{0}", this.upgradeData.GetNewCellCount());
		this.isOwned = GameManager.Instance.SaveData.GetIsUpgradeOwned(this.upgradeData);
		this.areOwningPrerequisitesMet = this.upgradeData.prerequisiteUpgrades.TrueForAll((UpgradeData x) => GameManager.Instance.SaveData.GetIsUpgradeOwned(x));
		this.upgradeSelectable.CanBeUpgraded = !this.isOwned && this.areAllPrerequisitesMet;
		if (this.isOwned)
		{
			this.backplateImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			this.canUseHoverActions = false;
		}
		else if (this.areOwningPrerequisitesMet)
		{
			this.backplateImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			this.canUseHoverActions = true;
		}
		else
		{
			this.backplateImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED);
			this.canUseHoverActions = false;
		}
		this.linesToColorWhenPrerequisitesAreMet.ForEach(delegate(Image l)
		{
			l.color = (this.areOwningPrerequisitesMet ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
		});
		this.linesToColorWhenThisIsResearched.ForEach(delegate(Image l)
		{
			l.color = (this.isOwned ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
		});
	}

	[SerializeField]
	private UpgradeData upgradeData;

	[SerializeField]
	private Image backplateImage;

	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private UpgradeSelectable upgradeSelectable;

	[SerializeField]
	private UpgradeTooltipRequester upgradeTooltipRequester;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private List<Image> linesToColorWhenPrerequisitesAreMet;

	[SerializeField]
	private List<Image> linesToColorWhenThisIsResearched;

	private bool isOwned;

	private bool areOwningPrerequisitesMet;

	private bool areAllPrerequisitesMet;

	public Action<UpgradeNodeUI> OnEntrySelected;

	public Action<UpgradeNodeUI> OnEntryHovered;

	public Action<UpgradeNodeUI> OnEntryUnhovered;

	public bool interactable = true;

	private bool canUseHoverActions;
}
