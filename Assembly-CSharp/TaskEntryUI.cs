using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TaskEntryUI : MonoBehaviour
{
	public void Init(LocalizedString stringKey, bool isComplete)
	{
		this.localizedStringField.StringReference = stringKey;
		this.localizedStringField.RefreshString();
		this.bulletImage.sprite = (isComplete ? this.filledBullet : this.emptyBullet);
		this.activeBackplate.gameObject.SetActive(!isComplete);
	}

	[SerializeField]
	private LocalizeStringEvent localizedStringField;

	[SerializeField]
	private Image bulletImage;

	[SerializeField]
	private Image activeBackplate;

	[Header("Config")]
	[SerializeField]
	private Sprite filledBullet;

	[SerializeField]
	private Sprite emptyBullet;
}
