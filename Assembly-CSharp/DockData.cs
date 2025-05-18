using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "DockData", menuName = "Dredge/DockData", order = 0)]
public class DockData : ScriptableObject
{
	public LocalizedString DockNameKey
	{
		get
		{
			return this.dockNameKey;
		}
	}

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public AssetReference MusicAssetReference
	{
		get
		{
			return this.musicAssetReference;
		}
	}

	public List<AssetReferenceOverride> MusicAssetOverrides
	{
		get
		{
			return this.musicAssetOverrides;
		}
	}

	public AssetReference AmbienceDayAssetReference
	{
		get
		{
			return this.ambienceDayAssetReference;
		}
	}

	public AssetReference AmbienceNightAssetReference
	{
		get
		{
			return this.ambienceNightAssetReference;
		}
	}

	public List<AssetReferenceOverride> AmbienceDayAssetOverrides
	{
		get
		{
			return this.ambienceDayAssetOverrides;
		}
	}

	public List<AssetReferenceOverride> AmbienceNightAssetOverrides
	{
		get
		{
			return this.ambienceNightAssetOverrides;
		}
	}

	public string YarnRootNode
	{
		get
		{
			return this.yarnRootNode;
		}
	}

	public string ProgressTitleLocalizationKey
	{
		get
		{
			return this.progressTitleLocalizationKey;
		}
	}

	public string ProgressValueLocalizationKey
	{
		get
		{
			return this.progressValueLocalizationKey;
		}
	}

	public DockProgressType DockProgressType
	{
		get
		{
			return this.dockProgressType;
		}
	}

	public List<SpeakerData> Speakers
	{
		get
		{
			return this.speakers;
		}
	}

	public bool HasCameraOverride
	{
		get
		{
			return this.hasCameraOverride;
		}
	}

	public float CameraOverrideX
	{
		get
		{
			return this.cameraOverrideX;
		}
	}

	public float CameraOverrideY
	{
		get
		{
			return this.cameraOverrideY;
		}
	}

	[SerializeField]
	private LocalizedString dockNameKey;

	[SerializeField]
	private string id;

	[SerializeField]
	private AssetReference musicAssetReference;

	[SerializeField]
	private List<AssetReferenceOverride> musicAssetOverrides;

	[SerializeField]
	private AssetReference ambienceDayAssetReference;

	[SerializeField]
	private AssetReference ambienceNightAssetReference;

	[SerializeField]
	private List<AssetReferenceOverride> ambienceDayAssetOverrides;

	[SerializeField]
	private List<AssetReferenceOverride> ambienceNightAssetOverrides;

	[SerializeField]
	private string yarnRootNode;

	[SerializeField]
	private string progressTitleLocalizationKey;

	[SerializeField]
	private string progressValueLocalizationKey;

	[SerializeField]
	private DockProgressType dockProgressType;

	[SerializeField]
	private List<SpeakerData> speakers;

	[SerializeField]
	private bool hasCameraOverride;

	[SerializeField]
	private float cameraOverrideX = 0.5f;

	[SerializeField]
	private float cameraOverrideY = 0.5f;
}
