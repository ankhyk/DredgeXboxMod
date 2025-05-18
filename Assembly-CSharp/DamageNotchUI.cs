using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNotchUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshUI();
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
	}

	private void OnPlayerDamageChanged()
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		int numberOfDamagedSlots = GameManager.Instance.SaveData.GetNumberOfDamagedSlots();
		for (int i = 0; i < this.notchImages.Count; i++)
		{
			this.notchImages[i].gameObject.SetActive(i < GameManager.Instance.PlayerStats.DamageThreshold);
			this.notchImages[i].sprite = ((i < numberOfDamagedSlots) ? this.filledImage : this.emptyImage);
		}
	}

	[SerializeField]
	private List<Image> notchImages;

	[SerializeField]
	private Sprite filledImage;

	[SerializeField]
	private Sprite emptyImage;
}
