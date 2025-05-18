using System;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class Dock : SerializedMonoBehaviour
{
	public DockData Data
	{
		get
		{
			return this.dockData;
		}
	}

	public Transform LookAtTarget
	{
		get
		{
			return this.lookAtTarget;
		}
	}

	public Dictionary<string, CinemachineVirtualCamera> SpeakerVCams
	{
		get
		{
			return this.speakerVCams;
		}
	}

	public virtual List<BaseDestination> GetDestinations()
	{
		return this.destinations;
	}

	public virtual CinemachineVirtualCamera GetVCam()
	{
		return this.dockVCam;
	}

	public void RefreshVCams()
	{
		this.DisableVCam();
		this.EnableVCam();
	}

	public virtual void EnableVCam()
	{
		this.dockVCam.enabled = true;
	}

	public virtual void DisableVCam()
	{
		this.dockVCam.enabled = false;
	}

	[SerializeField]
	private DockData dockData;

	[SerializeField]
	protected CinemachineVirtualCamera dockVCam;

	[SerializeField]
	protected List<BaseDestination> destinations;

	[SerializeField]
	public BoatActionsDestination boatActionsDestination;

	[SerializeField]
	private Transform lookAtTarget;

	[SerializeField]
	private Dictionary<string, CinemachineVirtualCamera> speakerVCams = new Dictionary<string, CinemachineVirtualCamera>();
}
