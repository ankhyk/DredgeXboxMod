using System;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class SplashWorldEvent : WorldEvent
{
	public override void Activate()
	{
		base.transform.position = GameManager.Instance.Player.transform.position;
		this.spawnTime = Time.time;
		GameManager.Instance.VibrationManager.Vibrate(this.splashStartVibration, VibrationRegion.WholeBody, true).Run();
	}

	private void Update()
	{
		if (!this.finishRequested && Time.time > this.spawnTime + this.particles.main.duration)
		{
			this.RequestEventFinish();
		}
	}

	public override void RequestEventFinish()
	{
		base.RequestEventFinish();
		global::UnityEngine.Object.Destroy(base.gameObject);
		this.EventFinished();
	}

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private float spawnRadius;

	[SerializeField]
	private VibrationData splashStartVibration;

	private float spawnTime;
}
