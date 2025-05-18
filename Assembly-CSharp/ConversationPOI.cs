using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public abstract class ConversationPOI : POI, IConversationStarter
{
	public string ConversationNodeName
	{
		get
		{
			return this.conversationNodeName;
		}
	}

	public bool IsOneTimeOnly
	{
		get
		{
			return this.isOneTimeOnly;
		}
	}

	public bool ReleaseCameraOnComplete
	{
		get
		{
			return this.releaseCameraOnComplete;
		}
	}

	public CinemachineVirtualCamera VCam
	{
		get
		{
			return this.vCam;
		}
	}

	private void Start()
	{
		this.RefreshStatus();
	}

	private void Update()
	{
		if (this.isDueRefresh)
		{
			this.RefreshStatus();
		}
	}

	public virtual bool RefreshStatus()
	{
		if (!GameManager.Instance || !GameManager.Instance.DialogueRunner)
		{
			return false;
		}
		bool flag = false;
		if (this.enabledByOtherNodeVisit)
		{
			if (!this.enabledByOtherNodeVisit)
			{
				goto IL_005F;
			}
			if (!this.enableNodeNames.All((string x) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(x)))
			{
				goto IL_005F;
			}
		}
		flag = true;
		IL_005F:
		if (this.isOneTimeOnly && GameManager.Instance.DialogueRunner.GetHasVisitedNode(this.conversationNodeName))
		{
			flag = false;
			base.gameObject.SetActive(false);
		}
		if (this.shouldDisableOnOtherNodeVisit)
		{
			bool flag2;
			if (flag)
			{
				flag2 = !this.otherNodeNames.Any((string x) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(x));
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
			if (!flag)
			{
				base.gameObject.SetActive(false);
			}
		}
		this.isDueRefresh = false;
		this.interactCollider.enabled = flag;
		return flag;
	}

	public override bool CanBeGhostWindTarget()
	{
		return this.canBeGhostWindTarget && this.interactCollider.enabled;
	}

	public abstract void OnConversationStarted();

	public abstract void OnConversationCompleted();

	[SerializeField]
	protected bool isOneTimeOnly = true;

	[SerializeField]
	protected bool releaseCameraOnComplete = true;

	[SerializeField]
	protected string conversationNodeName;

	[SerializeField]
	protected bool enabledByOtherNodeVisit;

	[SerializeField]
	protected List<string> enableNodeNames;

	[SerializeField]
	protected bool shouldDisableOnOtherNodeVisit;

	[SerializeField]
	protected List<string> otherNodeNames;

	[SerializeField]
	private CinemachineVirtualCamera vCam;

	[SerializeField]
	private Collider interactCollider;

	protected bool isDueRefresh;
}
