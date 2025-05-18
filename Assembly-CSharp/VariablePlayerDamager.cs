using System;
using System.Collections.Generic;
using UnityEngine;

public class VariablePlayerDamager : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.automaticallyAddListeners)
		{
			this.AddListeners();
		}
	}

	private void OnDisable()
	{
		if (this.automaticallyAddListeners)
		{
			this.RemoveListeners();
		}
	}

	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	public void AddListeners()
	{
		this.playerDetectors.ForEach(delegate(PlayerDetector p)
		{
			p.OnPlayerDetected = (Action)Delegate.Combine(p.OnPlayerDetected, new Action(this.OnPlayerHit));
		});
	}

	public void RemoveListeners()
	{
		this.playerDetectors.ForEach(delegate(PlayerDetector p)
		{
			p.OnPlayerDetected = (Action)Delegate.Remove(p.OnPlayerDetected, new Action(this.OnPlayerHit));
		});
	}

	public void OnPlayerHit()
	{
		if (this.oneHitOnly)
		{
			this.RemoveListeners();
		}
		int num = this.damagePoints;
		if (GameManager.Instance.SettingsSaveData.CurrentGameMode() == GameMode.NIGHTMARE)
		{
			num += this.extraDamageInNightmareMode;
		}
		if (this.requireOneHealthToKill)
		{
			int remainingHealth = GameManager.Instance.Player.RemainingHealth;
			if (remainingHealth != 1 && num >= remainingHealth)
			{
				num = remainingHealth - 1;
			}
		}
		GameManager.Instance.GridManager.AddDamageToInventory(num, -1, -1);
		Action playerHit = this.PlayerHit;
		if (playerHit != null)
		{
			playerHit();
		}
		if (this.hitVibration != null)
		{
			GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
		}
	}

	[SerializeField]
	private bool oneHitOnly;

	[SerializeField]
	public int damagePoints;

	[SerializeField]
	private bool requireOneHealthToKill;

	[SerializeField]
	private bool automaticallyAddListeners = true;

	[SerializeField]
	private List<PlayerDetector> playerDetectors;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private int extraDamageInNightmareMode;

	public Action PlayerHit;
}
