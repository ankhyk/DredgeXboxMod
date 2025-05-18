using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class POI : SerializedMonoBehaviour
{
	public Transform GhostWindTargetTransform
	{
		get
		{
			return this.ghostWindTargetTransform;
		}
	}

	public Transform InteractPointTargetTransform
	{
		get
		{
			return this.interactPointTargetTransform;
		}
	}

	public virtual bool CanBeGhostWindTarget()
	{
		return this.canBeGhostWindTarget;
	}

	[SerializeField]
	protected bool canBeGhostWindTarget;

	[SerializeField]
	private Transform ghostWindTargetTransform;

	[SerializeField]
	private Transform interactPointTargetTransform;
}
