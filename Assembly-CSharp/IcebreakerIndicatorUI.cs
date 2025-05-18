using System;
using UnityEngine;
using UnityEngine.UI;

public class IcebreakerIndicatorUI : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnIcebreakerEquipChanged += this.OnIcebreakerEquipChanged;
		this.RefreshUI();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnIcebreakerEquipChanged -= this.OnIcebreakerEquipChanged;
	}

	private void OnIcebreakerEquipChanged()
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.image.enabled = GameManager.Instance.SaveData.GetIsIcebreakerEquipped();
	}

	[SerializeField]
	private Image image;
}
