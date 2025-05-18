using System;
using UnityEngine;

public class AutoMovePOI : ConversationPOI, IConversationStarter
{
	private void OnEnable()
	{
		this.isDueRefresh = true;
	}

	public override void OnConversationStarted()
	{
		this.RefreshStatus();
		GameManager.Instance.Player.Controller.SetAutoMoveTarget(this.autoMoveDestination);
		if (this.includeRotation)
		{
			GameManager.Instance.Player.Controller.SetAutoRotateTarget(this.autoMoveDestination.forward);
		}
	}

	public override void OnConversationCompleted()
	{
		this.RefreshStatus();
	}

	[SerializeField]
	private Transform autoMoveDestination;

	[SerializeField]
	private bool includeRotation;
}
