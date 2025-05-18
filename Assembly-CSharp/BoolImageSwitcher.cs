using System;
using UnityEngine;
using UnityEngine.UI;

public class BoolImageSwitcher : MonoBehaviour
{
	private void OnEnable()
	{
		this.image.sprite = (GameManager.Instance.SaveData.GetBoolVariable(this.boolKey, false) ? this.trueSprite : this.falseSprite);
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private string boolKey;

	[SerializeField]
	private Sprite trueSprite;

	[SerializeField]
	private Sprite falseSprite;
}
