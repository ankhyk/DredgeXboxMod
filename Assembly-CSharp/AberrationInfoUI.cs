using System;
using UnityEngine;
using UnityEngine.UI;

public class AberrationInfoUI : MonoBehaviour
{
	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	public void SetData(FishItemData aberrationFishItemData)
	{
		bool flag = GameManager.Instance.SaveData.GetCaughtCountById(aberrationFishItemData.id) > 0;
		this.aberrationImage.sprite = aberrationFishItemData.sprite;
		this.aberrationImage.color = (flag ? this.itemImageColorIdentified : this.itemImageColorUnidentified);
		this.questionMarkImage.gameObject.SetActive(!flag);
		this.questionMarkImage.sprite = (aberrationFishItemData.IsAberration ? this.aberratedQuestionMarkSprite : this.regularQuestionMarkSprite);
		this.questionMarkImage.color = (aberrationFishItemData.IsAberration ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
	}

	[SerializeField]
	private Color itemImageColorIdentified;

	[SerializeField]
	private Color itemImageColorUnidentified;

	[SerializeField]
	private Image aberrationImage;

	[SerializeField]
	private Image questionMarkImage;

	[SerializeField]
	private Sprite aberratedQuestionMarkSprite;

	[SerializeField]
	private Sprite regularQuestionMarkSprite;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;
}
