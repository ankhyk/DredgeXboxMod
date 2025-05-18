using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		this.SetAbility(this.abilityData);
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool active)
	{
		if (this.abilityData.name == abilityData.name)
		{
			this.RefreshAbilityStateUI(active);
		}
	}

	private void RefreshAbilityStateUI(bool active)
	{
		this.activeIcon.sprite = (active ? this.activeSprite : this.inactiveSprite);
	}

	public void SetAbility(AbilityData abilityData)
	{
		this.abilityData = abilityData;
		this.cooldownFillIcon.fillAmount = 1f;
		this.icon.enabled = abilityData != null;
		if (abilityData.linkedAdvancedVersion && GameManager.Instance.SaveData.unlockedAbilities.Contains(abilityData.linkedAdvancedVersion.name))
		{
			this.icon.sprite = abilityData.linkedAdvancedVersion.icon;
		}
		else
		{
			this.icon.sprite = abilityData.icon;
		}
		this.RefreshAbilityStateUI(GameManager.Instance.PlayerAbilities.GetIsAbilityActive(abilityData));
	}

	public void SetDisabledEntirely(bool isDisabled)
	{
		this.disabledLayer.SetActive(isDisabled);
	}

	public void UpdateCooldownFill(float prop)
	{
		this.cooldownFillIcon.fillAmount = prop;
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image cooldownFillIcon;

	[SerializeField]
	private GameObject disabledLayer;

	[SerializeField]
	private Image activeIcon;

	[Header("Config")]
	[SerializeField]
	private Sprite activeSprite;

	[SerializeField]
	private Sprite inactiveSprite;

	private AbilityData abilityData;
}
