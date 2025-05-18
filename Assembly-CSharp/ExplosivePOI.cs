using System;
using System.Linq;
using Cinemachine;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class ExplosivePOI : ConversationPOI, IConversationStarter
{
	public override bool RefreshStatus()
	{
		bool flag = true;
		if (GameManager.Instance.SaveData.GetBoolVariable(this.id + ExplosivePOI.SAVE_DATA_SUFFIX, false))
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

	public override void OnConversationStarted()
	{
		GameManager.Instance.DialogueRunner.CurrentExplosivePOI = this;
	}

	public override void OnConversationCompleted()
	{
		GameManager.Instance.DialogueRunner.CurrentExplosivePOI = null;
		this.RefreshStatus();
	}

	public void Detonate()
	{
		this.impulseSource.GenerateImpulse();
		this.animator.SetTrigger("explode");
		GameManager.Instance.SaveData.SetBoolVariable(this.id + ExplosivePOI.SAVE_DATA_SUFFIX, true);
		GameManager.Instance.VibrationManager.Vibrate(this.ExplodeVibration, VibrationRegion.WholeBody, true).Run();
	}

	public static string SAVE_DATA_SUFFIX = "-detonated";

	[SerializeField]
	private string id;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private CinemachineImpulseSource impulseSource;

	[SerializeField]
	private VibrationData ExplodeVibration;
}
