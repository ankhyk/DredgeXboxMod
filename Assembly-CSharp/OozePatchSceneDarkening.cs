using System;
using UnityEngine;

public class OozePatchSceneDarkening : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
		this.OnTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
	}

	private void OnTIRWorldPhaseChanged(int phase)
	{
		this.maximumDarknessForTIRPhase = (this.maximumDarkness - this.minDarkness) * Mathf.Min(1f, (float)phase / 5f) + this.minDarkness;
	}

	private void Start()
	{
		Shader.SetGlobalFloat("_LightingTintStrength", 0f);
		Shader.SetGlobalFloat("_LightingTintStrength", this.targetStrength);
	}

	private void OnDestroy()
	{
		Shader.SetGlobalFloat("_LightingTintStrength", 0f);
	}

	private void Update()
	{
		Shader.SetGlobalColor("_LightingTint", this.color);
		if (GameManager.Instance.OozePatchManager.isOozeFarToPlayer || GameManager.Instance.OozePatchManager.isOozeNearToPlayer)
		{
			this.changeVelocity += Time.deltaTime;
		}
		else
		{
			this.changeVelocity -= Time.deltaTime;
		}
		this.changeVelocity = Mathf.Clamp(this.changeVelocity, -0.5f, 0.5f);
		this.targetStrength += this.changeVelocity * Time.deltaTime * this.transitionSpeed;
		this.targetStrength = Mathf.Clamp(this.targetStrength, 0f, this.maximumDarknessForTIRPhase);
		this.darkeningStrength = Mathf.Lerp(this.darkeningStrength, this.targetStrength, Time.deltaTime * this.transitionSpeed);
		Shader.SetGlobalFloat("_LightingTintStrength", this.darkeningStrength);
	}

	[SerializeField]
	private float transitionSpeed = 2f;

	[SerializeField]
	private float maximumDarkness = 1f;

	public Color color = new Color(0.2f, 0f, 0f, 1f);

	private float targetStrength;

	private float darkeningStrength;

	private float changeVelocity;

	private float maximumDarknessForTIRPhase;

	private float minDarkness;
}
