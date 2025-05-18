using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FogController : MonoBehaviour
{
	private void Start()
	{
		Shader.SetGlobalFloat("_FogRemove", 0f);
		this.allFogPropertyModifiers = global::UnityEngine.Object.FindObjectsOfType<FogPropertyModifier>().ToList<FogPropertyModifier>();
	}

	public FogPropertyModifier UpdateStrongestModifier()
	{
		FogPropertyModifier currentStrongestPropertyModifier = null;
		float highestStrength = 0f;
		this.allFogPropertyModifiers.ForEach(delegate(FogPropertyModifier fpm)
		{
			if (fpm)
			{
				float proportionStrengthForPoint = fpm.GetProportionStrengthForPoint(GameManager.Instance.Player.transform.position);
				if (proportionStrengthForPoint > highestStrength)
				{
					highestStrength = proportionStrengthForPoint;
					currentStrongestPropertyModifier = fpm;
				}
			}
		});
		return currentStrongestPropertyModifier;
	}

	private void Update()
	{
		FogPropertyModifier fogPropertyModifier = null;
		if (Application.isPlaying && GameManager.Instance.Player)
		{
			fogPropertyModifier = this.UpdateStrongestModifier();
		}
		float time = this.timeController.Time;
		float num = this.defaultFogDensityOverDay.Evaluate(time);
		float num2 = this.defaultFogHeight;
		Color color = this.defaultFogColorOverDay.Evaluate(time);
		if (fogPropertyModifier != null)
		{
			float proportionStrengthForPoint = fogPropertyModifier.GetProportionStrengthForPoint(GameManager.Instance.Player.transform.position);
			FogProperty fogProperty = fogPropertyModifier.FogProperty;
			if (proportionStrengthForPoint >= 1f)
			{
				num = fogProperty.fogDensityOverDay.Evaluate(time);
				num2 = fogProperty.fogHeight;
				color = fogProperty.fogColorOverDay.Evaluate(time);
			}
			else
			{
				float num3 = fogProperty.fogDensityOverDay.Evaluate(time);
				float fogHeight = fogProperty.fogHeight;
				Color color2 = fogProperty.fogColorOverDay.Evaluate(time);
				num = Mathf.Lerp(num, num3, proportionStrengthForPoint);
				num2 = Mathf.Lerp(num2, fogHeight, proportionStrengthForPoint);
				color = Color.Lerp(color, color2, proportionStrengthForPoint);
			}
		}
		Shader.SetGlobalFloat("_FogDensity", num);
		Shader.SetGlobalFloat("_FogHeight", num2);
		RenderSettings.fogColor = color;
	}

	private void OnDisable()
	{
		Shader.SetGlobalFloat("_FogDensity", this.defaultFogDensityOverDay.Evaluate(this.timeController.Time));
		Shader.SetGlobalFloat("_FogHeight", this.defaultFogHeight);
		RenderSettings.fogColor = this.defaultFogColorOverDay.Evaluate(this.timeController.Time);
	}

	private void ToggleFog()
	{
		if (Shader.IsKeywordEnabled("_FOGOFF"))
		{
			Shader.DisableKeyword("_FOGOFF");
			return;
		}
		Shader.EnableKeyword("_FOGOFF");
	}

	[SerializeField]
	public AnimationCurve defaultFogDensityOverDay;

	[SerializeField]
	public Gradient defaultFogColorOverDay;

	[SerializeField]
	public float defaultFogHeight;

	[SerializeField]
	private TimeController timeController;

	private List<FogPropertyModifier> allFogPropertyModifiers;
}
