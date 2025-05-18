using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackingTentacleWorldEvent : WorldEvent
{
	public override void Activate()
	{
		this.tentacles.ForEach(delegate(AttackingTentacle t)
		{
			t.OnAttackComplete = (Action)Delegate.Combine(t.OnAttackComplete, new Action(this.OnSingleAttackComplete));
		});
	}

	private void OnSingleAttackComplete()
	{
		if (!this.finishRequested)
		{
			if (this.tentacles.All((AttackingTentacle t) => t.IsAttackFinished))
			{
				this.RequestEventFinish();
			}
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.tentacles.ForEach(delegate(AttackingTentacle t)
			{
				t.RequestAttackFinish();
			});
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitUntil(() => this.tentacles.All((AttackingTentacle t) => t.IsAttackFinished));
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private List<AttackingTentacle> tentacles;
}
