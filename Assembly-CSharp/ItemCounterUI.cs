using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCounterUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshSprites();
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.UpdateColor();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.COLOR_EMPHASIS)
		{
			this.UpdateColor();
		}
	}

	private void UpdateColor()
	{
		this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.EMPHASIS);
	}

	public void SetCount(int count, bool pulse)
	{
		this.itemCountText.text = "<mspace=10>" + count.ToString();
		if (pulse)
		{
			this.animator.SetTrigger("pulse");
		}
	}

	private void RefreshSprites()
	{
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
		if (spatialItemInstance == null)
		{
			this.animatedObjectImage.sprite = this.fishIcon;
			return;
		}
		SpatialItemData itemData = spatialItemInstance.GetItemData<SpatialItemData>();
		if (itemData.id == this.oozeNet.id)
		{
			this.animatedObjectImage.sprite = this.oozeIcon;
			return;
		}
		if (itemData.id == this.materialNet.id)
		{
			this.animatedObjectImage.sprite = this.materialIcon;
			return;
		}
		this.animatedObjectImage.sprite = this.fishIcon;
	}

	[SerializeField]
	private AbilityData trawlAbility;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI itemCountText;

	[SerializeField]
	private Image animatedObjectImage;

	[SerializeField]
	private Sprite fishIcon;

	[SerializeField]
	private Sprite materialIcon;

	[SerializeField]
	private Sprite oozeIcon;

	[SerializeField]
	private SpatialItemData oozeNet;

	[SerializeField]
	private SpatialItemData materialNet;
}
