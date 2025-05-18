using System;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

public abstract class BaseDestination : SerializedMonoBehaviour
{
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public LocalizedString TitleKey
	{
		get
		{
			return this.titleKey;
		}
	}

	public virtual bool AlwaysShow
	{
		get
		{
			return this.alwaysShow;
		}
	}

	public CinemachineVirtualCamera VCam
	{
		get
		{
			return this.vCam;
		}
	}

	public SpeakerData SpeakerData
	{
		get
		{
			return this.speakerData;
		}
	}

	public string SpeakerRootNodeOverride
	{
		get
		{
			return this.speakerRootNodeOverride;
		}
	}

	public AssetReference VisitSFX
	{
		get
		{
			return this.visitSFX;
		}
	}

	public AudioClip LoopSFX
	{
		get
		{
			return this.loopSFX;
		}
	}

	public bool IsIndoors
	{
		get
		{
			return this.isIndoors;
		}
	}

	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	public List<int> PlayerInventoryTabIndexesToShow
	{
		get
		{
			return this.playerInventoryTabIndexesToShow;
		}
	}

	public List<HighlightCondition> HighlightConditions
	{
		get
		{
			return this.highlightConditions;
		}
	}

	[SerializeField]
	private string id;

	[SerializeField]
	private LocalizedString titleKey;

	[SerializeField]
	private bool alwaysShow;

	[SerializeField]
	private CinemachineVirtualCamera vCam;

	[SerializeField]
	private SpeakerData speakerData;

	[SerializeField]
	private string speakerRootNodeOverride;

	[SerializeField]
	private AssetReference visitSFX;

	[SerializeField]
	private AudioClip loopSFX;

	[SerializeField]
	private bool isIndoors;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private List<int> playerInventoryTabIndexesToShow;

	[SerializeField]
	private List<HighlightCondition> highlightConditions;

	[SerializeField]
	public BaseDestination useThisDestinationInsteadIfConstructed;

	[SerializeField]
	public bool useFixedScreenPosition;

	[SerializeField]
	public Transform transformToPointTo;

	[SerializeField]
	public Vector2 screenPosition;

	[SerializeField]
	public List<BaseDestination> selectOnLeft = new List<BaseDestination>();

	[SerializeField]
	public List<BaseDestination> selectOnRight = new List<BaseDestination>();

	[SerializeField]
	public List<BaseDestination> selectOnUp = new List<BaseDestination>();

	[SerializeField]
	public List<BaseDestination> selectOnDown = new List<BaseDestination>();
}
