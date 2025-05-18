using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DestinationButton : MonoBehaviour
{
	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	private void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
	}

	private void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
	}

	public virtual void Init(BaseDestination destination)
	{
		this.destination = destination;
		this.destinationLocalizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, destination.Id);
		this.destinationLocalizedString.StringReference.RefreshString();
		if (destination.Icon != null)
		{
			this.icon.sprite = destination.Icon;
		}
		else
		{
			this.icon.enabled = false;
			RectTransform rectTransform = this.destinationLocalizedString.gameObject.transform as RectTransform;
			rectTransform.SetLeft(-rectTransform.offsetMax.x);
			this.textField.alignment = TextAlignmentOptions.Center;
		}
		this.questAttentionCallout.SetActive(destination.HighlightConditions.Any((HighlightCondition h) => h.ShouldHighlight()));
	}

	private void OnButtonClicked()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
		BaseDestination useThisDestinationInsteadIfConstructed = this.destination;
		if (this.destination.useThisDestinationInsteadIfConstructed != null && this.destination is ConstructableDestination && GameManager.Instance.ConstructableBuildingManager && GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed((this.destination as ConstructableDestination).constructableDestinationData.tiers[0].tierId))
		{
			useThisDestinationInsteadIfConstructed = this.destination.useThisDestinationInsteadIfConstructed;
		}
		GameManager.Instance.UI.ShowDestination(useThisDestinationInsteadIfConstructed);
	}

	private void LateUpdate()
	{
		base.transform.position = Camera.main.WorldToScreenPoint(this.destination.transform.position);
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private LocalizeStringEvent destinationLocalizedString;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private GameObject questAttentionCallout;

	[SerializeField]
	private Image icon;

	public BaseDestination destination;
}
