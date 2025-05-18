using System;
using UnityEngine;

public class TSMonsterAnimationEvents : MonoBehaviour
{
	private void OnEmergeComplete()
	{
		Action emergeCompleteAction = this.EmergeCompleteAction;
		if (emergeCompleteAction == null)
		{
			return;
		}
		emergeCompleteAction();
	}

	private void OnPeekComplete()
	{
		Action peekCompleteAction = this.PeekCompleteAction;
		if (peekCompleteAction == null)
		{
			return;
		}
		peekCompleteAction();
	}

	public void PlayEmergeSFX()
	{
		this.tsMonster.PlayEmergeSFX();
	}

	public void PlaySubmergeSFX()
	{
		this.tsMonster.PlaySubmergeSFX();
	}

	[SerializeField]
	private TSMonster tsMonster;

	public Action EmergeCompleteAction;

	public Action PeekCompleteAction;
}
