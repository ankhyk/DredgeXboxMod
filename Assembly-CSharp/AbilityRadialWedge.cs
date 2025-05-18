using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityRadialWedge : MonoBehaviour
{
	public AbilityData AbilityData
	{
		get
		{
			return this.abilityData;
		}
	}

	private void OnEnable()
	{
		this.isAbilityUnlocked = GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked(this.abilityData);
		if (this.abilityData.linkedAdvancedVersion && GameManager.Instance.SaveData.unlockedAbilities.Contains(this.abilityData.linkedAdvancedVersion.name))
		{
			this.image.sprite = this.abilityData.linkedAdvancedVersion.icon;
		}
		else
		{
			this.image.sprite = (this.isAbilityUnlocked ? this.abilityData.icon : this.lockedSprite);
		}
		this.RefreshAttentionCallout();
	}

	public void SetHighlighted(bool highlighted)
	{
		this.image.color = (highlighted ? Color.black : Color.white);
		this.RefreshAttentionCallout();
	}

	private void RefreshAttentionCallout()
	{
		if (this.abilityData.linkedAdvancedVersion && GameManager.Instance.SaveData.unlockedAbilities.Contains(this.abilityData.linkedAdvancedVersion.name))
		{
			this.attentionCallout.SetActive(this.isAbilityUnlocked && this.abilityData.linkedAdvancedVersion.showUnseenNotification && GameManager.Instance.SaveData.GetAbilityUnseen(this.abilityData.linkedAdvancedVersion));
			return;
		}
		this.attentionCallout.SetActive(this.isAbilityUnlocked && this.abilityData.showUnseenNotification && GameManager.Instance.SaveData.GetAbilityUnseen(this.abilityData));
	}

	public void LayOutButton(int numSegments)
	{
		this.image.sprite = this.abilityData.icon;
		float num = 360f / (float)numSegments * (float)this.index;
		num = 0.017453292f * num;
		Vector2 vector = this.radius * Vector2.up * Mathf.Cos(num) + this.radius * Vector2.right * Mathf.Sin(num);
		(base.transform as RectTransform).anchoredPosition = vector;
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite lockedSprite;

	[SerializeField]
	private GameObject buttonCenter;

	[SerializeField]
	private AbilityData abilityData;

	[SerializeField]
	private int index;

	[SerializeField]
	private float radius;

	[SerializeField]
	private GameObject attentionCallout;

	private bool isAbilityUnlocked;
}
