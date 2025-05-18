using System;
using UnityEngine;

public class IceMonsterAnimationHelper : MonoBehaviour
{
	private void OnHuntComplete()
	{
		Action onHuntCompleteAction = this.OnHuntCompleteAction;
		if (onHuntCompleteAction == null)
		{
			return;
		}
		onHuntCompleteAction();
	}

	private void OnJumpComplete()
	{
		Action onJumpCompleteAction = this.OnJumpCompleteAction;
		if (onJumpCompleteAction == null)
		{
			return;
		}
		onJumpCompleteAction();
	}

	private void OnFeedShouldRemoveFish()
	{
		Action onFeedShouldRemoveFishAction = this.OnFeedShouldRemoveFishAction;
		if (onFeedShouldRemoveFishAction == null)
		{
			return;
		}
		onFeedShouldRemoveFishAction();
	}

	private void OnFeedingComplete()
	{
		Action onFeedingCompleteAction = this.OnFeedingCompleteAction;
		if (onFeedingCompleteAction == null)
		{
			return;
		}
		onFeedingCompleteAction();
	}

	private void OnDespawnComplete()
	{
		Action onDespawnCompleteAction = this.OnDespawnCompleteAction;
		if (onDespawnCompleteAction == null)
		{
			return;
		}
		onDespawnCompleteAction();
	}

	private void OnSpawnCall()
	{
		Action onSpawnCallAction = this.OnSpawnCallAction;
		if (onSpawnCallAction == null)
		{
			return;
		}
		onSpawnCallAction();
	}

	private void OnSniff()
	{
		Action onSniffAction = this.OnSniffAction;
		if (onSniffAction == null)
		{
			return;
		}
		onSniffAction();
	}

	public Action OnHuntCompleteAction;

	public Action OnJumpCompleteAction;

	public Action OnFeedShouldRemoveFishAction;

	public Action OnFeedingCompleteAction;

	public Action OnDespawnCompleteAction;

	public Action OnSpawnCallAction;

	public Action OnSniffAction;
}
