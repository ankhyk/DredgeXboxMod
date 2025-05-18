using System;
using UnityEngine;

public class TimeOfDayParticles : MonoBehaviour
{
	private void Update()
	{
		float time = GameManager.Instance.Time.Time;
		if (this.particleStartTime > this.particleEndTime)
		{
			bool flag = time < this.particleEndTime || time > this.particleStartTime;
			if (flag && this.particles.isStopped)
			{
				this.particles.Play();
				return;
			}
			if (!flag && this.particles.isPlaying)
			{
				this.particles.Stop();
				return;
			}
		}
		else
		{
			bool flag2 = time < this.particleEndTime && time > this.particleStartTime;
			if (flag2 && this.particles.isStopped)
			{
				this.particles.Play();
				return;
			}
			if (!flag2 && this.particles.isPlaying)
			{
				this.particles.Stop();
			}
		}
	}

	[SerializeField]
	private float particleStartTime = 0.27f;

	[SerializeField]
	private float particleEndTime = 0.6f;

	[SerializeField]
	private ParticleSystem particles;
}
