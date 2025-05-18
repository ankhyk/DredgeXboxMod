using System;
using UnityEngine;

public class LeviathanAnimationEvents : MonoBehaviour
{
	public bool HasPlayerTeleportedAway { get; set; }

	private void DisableMovement()
	{
		if (!this.HasPlayerTeleportedAway && !GameManager.Instance.Player.IsGodModeEnabled)
		{
			GameManager.Instance.Player.Controller.IsMovementAllowed = false;
		}
	}

	private void DisableBoatModel()
	{
		if (!this.HasPlayerTeleportedAway && !GameManager.Instance.Player.IsGodModeEnabled)
		{
			GameManager.Instance.Player.ToggleBoatModel(false);
		}
	}

	private void KillPlayer()
	{
		if (!this.HasPlayerTeleportedAway && !GameManager.Instance.Player.IsGodModeEnabled && !GameManager.Instance.WorldEventManager.DoesHitSafeZone(GameManager.Instance.Player.transform.position))
		{
			GameManager.Instance.Player.Die();
		}
	}
}
