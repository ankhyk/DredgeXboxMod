using System;
using UnityEngine;

public class TIRTentacles : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnShowRigTentacles += this.Show;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnShowRigTentacles -= this.Show;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (enabled && abilityData.name == this.banishAbilityData.name)
		{
			this.animator.SetTrigger("banish");
		}
	}

	public void Show()
	{
		this.animator.runtimeAnimatorController = this.animationController;
		this.tentacleObject.SetActive(true);
	}

	[SerializeField]
	private GameObject tentacleObject;

	[SerializeField]
	private RuntimeAnimatorController animationController;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AbilityData banishAbilityData;
}
