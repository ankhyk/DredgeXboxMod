using System;
using System.Collections;
using UnityEngine;

public class OozeTentacle : OozeEvent
{
	private void Start()
	{
		AttackingTentacle attackingTentacle = this.attackingTentacle;
		attackingTentacle.OnAttackComplete = (Action)Delegate.Combine(attackingTentacle.OnAttackComplete, new Action(this.OnAttackComplete));
	}

	private void OnAttackComplete()
	{
		Action<OozeEvent> onOozeEventComplete = this.OnOozeEventComplete;
		if (onOozeEventComplete != null)
		{
			onOozeEventComplete(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (enabled && ability.name == this.banishAbility.name && !this.finishRequested)
		{
			GameEvents.Instance.TriggerThreatBanished(true);
			this.RequestEventFinish();
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			this.finishRequested = true;
			this.attackingTentacle.RequestAttackFinish();
			base.StartCoroutine(this.DelayedEventFinish());
		}
		base.RequestEventFinish();
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitUntil(() => this.attackingTentacle.IsAttackFinished);
		Action<OozeEvent> onOozeEventComplete = this.OnOozeEventComplete;
		if (onOozeEventComplete != null)
		{
			onOozeEventComplete(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private AttackingTentacle attackingTentacle;

	[SerializeField]
	private AbilityData banishAbility;
}
