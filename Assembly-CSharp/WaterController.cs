using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterController : MonoBehaviour
{
	private void FindWaterPropertyModifiers()
	{
	}

	public WaterPropertyModifier UpdateStrongestModifier()
	{
		WaterPropertyModifier currentStrongestPropertyModifier = null;
		float highestStrength = 0f;
		this.allWaterPropertyModifiers.ForEach(delegate(WaterPropertyModifier wpm)
		{
			float proportionStrengthForPoint = wpm.GetProportionStrengthForPoint(GameManager.Instance.Player.transform.position);
			if (proportionStrengthForPoint > highestStrength)
			{
				highestStrength = proportionStrengthForPoint;
				currentStrongestPropertyModifier = wpm;
			}
		});
		return currentStrongestPropertyModifier;
	}

	private void Update()
	{
		WaterPropertyModifier waterPropertyModifier = null;
		if (Application.isPlaying && GameManager.Instance.Player)
		{
			waterPropertyModifier = this.UpdateStrongestModifier();
		}
		float num = this.defaultWaterProperties.waterDepth;
		Color color = this.defaultWaterProperties.foamColor;
		Color color2 = this.defaultWaterProperties.shallowColor;
		Color color3 = this.defaultWaterProperties.deepColor;
		if (waterPropertyModifier != null)
		{
			float proportionStrengthForPoint = waterPropertyModifier.GetProportionStrengthForPoint(GameManager.Instance.Player.transform.position);
			WaterProperty waterProperty = waterPropertyModifier.WaterProperty;
			if (proportionStrengthForPoint >= 1f)
			{
				num = waterProperty.waterDepth;
				color = waterProperty.foamColor;
				color2 = waterProperty.shallowColor;
				color3 = waterProperty.deepColor;
			}
			else
			{
				num = Mathf.Lerp(this.defaultWaterProperties.waterDepth, waterProperty.waterDepth, proportionStrengthForPoint);
				color = Color.Lerp(this.defaultWaterProperties.foamColor, waterProperty.foamColor, proportionStrengthForPoint);
				color2 = Color.Lerp(this.defaultWaterProperties.shallowColor, waterProperty.shallowColor, proportionStrengthForPoint);
				color3 = Color.Lerp(this.defaultWaterProperties.deepColor, waterProperty.deepColor, proportionStrengthForPoint);
			}
		}
		Shader.SetGlobalFloat("_Depth", num);
		Shader.SetGlobalColor("_FoamColor", color);
		Shader.SetGlobalColor("_ShallowColor", color2);
		Shader.SetGlobalColor("_DeepColor", color3);
	}

	private void OnDisable()
	{
		Shader.SetGlobalFloat("_Depth", this.defaultWaterProperties.waterDepth);
		Shader.SetGlobalColor("_FoamColor", this.defaultWaterProperties.foamColor);
		Shader.SetGlobalColor("_ShallowColor", this.defaultWaterProperties.shallowColor);
		Shader.SetGlobalColor("_DeepColor", this.defaultWaterProperties.deepColor);
	}

	[SerializeField]
	private WaterProperty defaultWaterProperties;

	[SerializeField]
	private List<WaterPropertyModifier> allWaterPropertyModifiers = new List<WaterPropertyModifier>();
}
