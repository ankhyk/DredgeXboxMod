using System;
using UnityEngine;

public class SafeParticleDestroyer : MonoBehaviour
{
	public void Destroy()
	{
		float num = 0f;
		foreach (ParticleSystem particleSystem in this.particlesToDestroy)
		{
			num = Mathf.Max(num, particleSystem.main.startLifetime.constantMax);
			particleSystem.Stop();
		}
		global::UnityEngine.Object.Destroy(base.gameObject, num);
	}

	[SerializeField]
	private ParticleSystem[] particlesToDestroy;
}
