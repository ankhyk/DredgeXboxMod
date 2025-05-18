using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ResearchableGridEntryUI : NonSpatialGridEntryUI
{
	public override void Init(NonSpatialItemInstance itemInstance)
	{
		base.Init(itemInstance);
	}

	protected override void InitUI()
	{
		base.InitUI();
	}

	public override void RefreshUI()
	{
		base.RefreshUI();
		ResearchableItemInstance researchableItemInstance = this.itemInstance as ResearchableItemInstance;
		int num = Mathf.FloorToInt(researchableItemInstance.progress * 100f);
		this.achievedImage.sprite = this.unachievedSprite;
		Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		if (researchableItemInstance.IsResearchComplete)
		{
			this.achievedImage.sprite = this.achievedSprite;
			color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			this.localizedStatusTextField.StringReference = this.completeStatusStringRef;
			this.localizedDescriptionTextField.StringReference = researchableItemInstance.GetItemData<ResearchableItemData>().completedDescriptionKey;
		}
		else if (researchableItemInstance.isActive)
		{
			color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.WARNING);
			this.localizedStatusTextField.StringReference = this.activeStatusStringRef;
			this.localizedStatusTextField.StringReference.Arguments = new object[] { num };
			this.localizedDescriptionTextField.StringReference = this.activeDescriptionStringRef;
		}
		else
		{
			this.localizedStatusTextField.StringReference = this.inactiveStatusStringRef;
			this.localizedStatusTextField.StringReference.Arguments = new object[] { num };
			this.localizedDescriptionTextField.StringReference = this.inactiveDescriptionStringRef;
		}
		this.statusTextField.color = color;
		this.backgroundImage.color = color;
		this.localizedStatusTextField.RefreshString();
		this.localizedDescriptionTextField.RefreshString();
		this.localizedStatusTextField.enabled = true;
		this.localizedDescriptionTextField.enabled = true;
	}

	[SerializeField]
	protected LocalizeStringEvent localizedStatusTextField;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	protected Image achievedImage;

	[Header("Config")]
	[SerializeField]
	protected Sprite achievedSprite;

	[SerializeField]
	protected Sprite unachievedSprite;

	[SerializeField]
	protected TextMeshProUGUI statusTextField;

	[SerializeField]
	protected LocalizedString completeStatusStringRef;

	[SerializeField]
	protected LocalizedString activeStatusStringRef;

	[SerializeField]
	protected LocalizedString inactiveStatusStringRef;

	[SerializeField]
	protected LocalizedString activeDescriptionStringRef;

	[SerializeField]
	protected LocalizedString inactiveDescriptionStringRef;
}
