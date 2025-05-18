using System;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShadowQualitySettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.SHADOW_QUALITY)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null)
		{
			switch (GameManager.Instance.SettingsSaveData.shadowQuality)
			{
			case 1:
				this.MainLightShadowResolution = ShadowResolution._2048;
				return;
			case 2:
				this.MainLightShadowResolution = ShadowResolution._4096;
				return;
			}
			this.MainLightShadowResolution = ShadowResolution._1024;
		}
	}

	private void InitializeShadowMapFieldInfo()
	{
		this.universalRenderPipelineAssetType = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetType();
		this.mainLightShadowmapResolutionFieldInfo = this.universalRenderPipelineAssetType.GetField("m_MainLightShadowmapResolution", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	public ShadowResolution MainLightShadowResolution
	{
		get
		{
			if (this.mainLightShadowmapResolutionFieldInfo == null)
			{
				this.InitializeShadowMapFieldInfo();
			}
			return (ShadowResolution)this.mainLightShadowmapResolutionFieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
		}
		set
		{
			if (this.mainLightShadowmapResolutionFieldInfo == null)
			{
				this.InitializeShadowMapFieldInfo();
			}
			this.mainLightShadowmapResolutionFieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
		}
	}

	private Type universalRenderPipelineAssetType;

	private FieldInfo mainLightShadowmapResolutionFieldInfo;
}
