using System;
using UnityEngine;

public class LoreRockManager : MonoBehaviour
{
	private void Update()
	{
		if (this.playerSanity == null)
		{
			if (GameManager.Instance.Player)
			{
				this.playerSanity = GameManager.Instance.Player.Sanity;
			}
			return;
		}
		float num = Mathf.InverseLerp(0f, this.pulsePeriod, Mathf.PingPong(Time.time, this.pulsePeriod));
		float num2 = this.pulseCurve.Evaluate(num);
		this.targetStrength = ((this.playerSanity.CurrentSanity < this.sanityThreshold) ? 1f : 0f);
		this.currentStrength = Mathf.Lerp(this.currentStrength, this.targetStrength, Time.deltaTime);
		this.material.SetFloat("_GlowStrength", this.maxStrength * this.currentStrength * num2);
	}

	[SerializeField]
	private Material material;

	[SerializeField]
	private float sanityThreshold;

	[SerializeField]
	private AnimationCurve pulseCurve;

	[SerializeField]
	private float maxStrength;

	[SerializeField]
	private float pulsePeriod;

	private PlayerSanity playerSanity;

	private float currentStrength;

	private float targetStrength;
}
