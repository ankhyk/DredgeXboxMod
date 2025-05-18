using System;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class ParasiteWorldEvent : WorldEvent
{
	public override void Activate()
	{
		base.transform.position = GameManager.Instance.Player.transform.position;
		this.spawnTime = Time.time;
		GameManager.Instance.VibrationManager.Vibrate(this.InfectVibration, VibrationRegion.WholeBody, true).Run();
	}

	private void Update()
	{
		if (!this.hasParasiteLanded && Time.time > this.spawnTime + this.delayBeforeAddingParasite)
		{
			this.hasParasiteLanded = true;
			GameManager.Instance.GridManager.InfectRandomItemInInventory();
		}
		if (!this.finishRequested && Time.time > this.spawnTime + this.particles.main.duration)
		{
			this.RequestEventFinish();
		}
	}

	public override void RequestEventFinish()
	{
		base.RequestEventFinish();
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private float spawnRadius;

	[SerializeField]
	private float delayBeforeAddingParasite;

	[SerializeField]
	private VibrationData InfectVibration;

	private bool hasParasiteLanded;

	private float spawnTime;
}
