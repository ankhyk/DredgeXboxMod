using System;
using UnityEngine;

public class WreckMonster : MonoBehaviour
{
	private void OnEnable()
	{
		this.lureParticles.Play();
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAttackComplete));
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAttackComplete));
	}

	private void OnAttackComplete()
	{
		this.isCurrentlyAttacking = false;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool isActive)
	{
		if (ability.name == this.banishAbility.name)
		{
			this.isBanishActive = isActive;
			this.animator.SetBool("isBanished", this.isBanishActive);
		}
	}

	private void TryAttack()
	{
		if (!this.isCurrentlyAttacking && !this.isBanishActive)
		{
			this.lureParticles.Stop();
			this.animator.SetTrigger("Attack");
			this.isCurrentlyAttacking = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		this.TryAttack();
	}

	[SerializeField]
	private ParticleSystem lureParticles;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private string id;

	private bool isBanishActive;

	private bool isCurrentlyAttacking;

	private string saveKey = "wreck-monster-last-attack";
}
