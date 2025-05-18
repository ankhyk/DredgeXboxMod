using System;
using UnityEngine;

public class DSBigMonsterAnimationEvents : MonoBehaviour
{
	private void AttackComplete()
	{
		Action onAttackComplete = this.OnAttackComplete;
		if (onAttackComplete == null)
		{
			return;
		}
		onAttackComplete();
	}

	private void LungeStart()
	{
		Action onLungeStart = this.OnLungeStart;
		if (onLungeStart == null)
		{
			return;
		}
		onLungeStart();
	}

	public Action OnAttackComplete;

	public Action OnLungeStart;
}
