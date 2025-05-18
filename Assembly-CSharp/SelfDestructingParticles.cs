using System;
using UnityEngine;

public class SelfDestructingParticles : MonoBehaviour
{
	public void OnEnable()
	{
		float num = 0f;
		foreach (ParticleSystem particleSystem in this.particlesToDestroy)
		{
			num = Mathf.Max(num, particleSystem.main.startLifetime.constantMax);
		}
		global::UnityEngine.Object.Destroy(base.gameObject, num);
	}

	[SerializeField]
	private ParticleSystem[] particlesToDestroy;
}
