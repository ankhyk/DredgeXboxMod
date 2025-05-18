using System;

public class InspectPOI : ConversationPOI, IConversationStarter
{
	private void OnEnable()
	{
		this.isDueRefresh = true;
	}

	public override void OnConversationStarted()
	{
		this.RefreshStatus();
	}

	public override void OnConversationCompleted()
	{
		this.RefreshStatus();
	}
}
