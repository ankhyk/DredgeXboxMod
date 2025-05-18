using System;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerEffect : MonoBehaviour
{
	public void BeginFlicker(AnimationCurve flickerCurve, float flickerDurationSec, bool enableAfterFinish)
	{
		this.enableAfterFinish = enableAfterFinish;
		this.flickerCurve = flickerCurve;
		this.flickerDurationSec = flickerDurationSec;
		this.isFlickering = true;
		this.currentFlickerDuration = 0f;
		for (int i = 0; i < this.boatModelProxy.Lights.Length; i++)
		{
			this.boatModelProxy.Lights[i].SetActive(true);
		}
	}

	private void Update()
	{
		if (this.isFlickering)
		{
			this.currentFlickerDuration += Time.deltaTime;
			float currentStrengthProp = this.flickerCurve.Evaluate(Mathf.InverseLerp(0f, this.flickerDurationSec, this.currentFlickerDuration));
			float num = Mathf.Lerp(0f, this.maxMaterialLightStrength, currentStrengthProp);
			this.boatModelProxy.SetLightStrength(num);
			for (int i = 0; i < this.boatModelProxy.LightBeams.Length; i++)
			{
				this.boatModelProxy.LightBeams[i].SetActive(currentStrengthProp > 0f);
			}
			this.lights.ForEach(delegate(PlayerLight pl)
			{
				pl.Intensity = Mathf.Lerp(0f, pl.CalculatedIntensity, currentStrengthProp);
				pl.Range = Mathf.Lerp(0f, pl.CalculatedRange, currentStrengthProp);
			});
			if (this.currentFlickerDuration >= this.flickerDurationSec)
			{
				this.isFlickering = false;
				this.lights.ForEach(delegate(PlayerLight pl)
				{
					pl.Intensity = pl.CalculatedIntensity;
					pl.Range = pl.CalculatedRange;
				});
				if (this.enableAfterFinish)
				{
					this.boatModelProxy.SetLightStrength(this.maxMaterialLightStrength);
				}
				for (int j = 0; j < this.boatModelProxy.LightBeams.Length; j++)
				{
					this.boatModelProxy.LightBeams[j].SetActive(this.enableAfterFinish);
				}
				for (int k = 0; k < this.boatModelProxy.Lights.Length; k++)
				{
					this.boatModelProxy.Lights[k].SetActive(this.enableAfterFinish);
				}
			}
		}
	}

	[SerializeField]
	private BoatModelProxy boatModelProxy;

	[SerializeField]
	private List<PlayerLight> lights;

	[SerializeField]
	private float maxMaterialLightStrength;

	private AnimationCurve flickerCurve;

	private float flickerDurationSec;

	private bool enableAfterFinish;

	private float currentFlickerDuration;

	private bool isFlickering;
}
