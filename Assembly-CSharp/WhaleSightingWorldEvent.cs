using System;
using System.Collections;
using UnityEngine;

public class WhaleSightingWorldEvent : WorldEvent
{
	private void OnEnable()
	{
		base.StartCoroutine(this.DelayedEventFinish());
	}

	public override void Activate()
	{
		Vector3 zero = Vector3.zero;
		zero.y = GameManager.Instance.Player.transform.eulerAngles.y + 180f;
		base.transform.eulerAngles = zero;
	}

	public override void RequestEventFinish()
	{
		base.RequestEventFinish();
		this.EventFinished();
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.finishDelaySec);
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private float finishDelaySec;
}
