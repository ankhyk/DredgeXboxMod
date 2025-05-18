using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUpgradeCostIcon : MonoBehaviour
{
	public void Init(ItemData itemData, int addedCount, int totalCount)
	{
		this.image.sprite = itemData.itemTypeIcon;
		this.textField.text = string.Format("{0}/{1}", addedCount, totalCount);
		if (addedCount >= totalCount)
		{
			this.textField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			return;
		}
		this.textField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private TextMeshProUGUI textField;
}
