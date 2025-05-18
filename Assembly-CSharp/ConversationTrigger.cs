using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour, IConversationStarter
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
			return true;
		}
	}

	public bool ReleaseCameraOnComplete
	{
		get
		{
			return true;
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

	public void OnConversationStarted()
	{
		this.RefreshStatus();
	}

	public void OnConversationCompleted()
	{
		this.RefreshStatus();
	}

	public bool RefreshStatus()
	{
		bool flag = true;
		if (GameManager.Instance.DialogueRunner.GetHasVisitedNode(this.conversationNodeName))
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
		return flag;
	}

	[SerializeField]
	private string conversationNodeName;

	[SerializeField]
	private CinemachineVirtualCamera vCam;

	[SerializeField]
	private bool shouldDisableOnOtherNodeVisit;

	[SerializeField]
	private List<string> otherNodeNames;
}
