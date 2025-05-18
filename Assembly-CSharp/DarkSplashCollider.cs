using System;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class DarkSplashCollider : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (this.hasTriggered)
		{
			return;
		}
		if (other.gameObject.tag == "Player")
		{
			base.gameObject.SetActive(false);
			this.hasTriggered = true;
			GameManager.Instance.VibrationManager.Vibrate(this.explodeVibration, VibrationRegion.WholeBody, true).Run();
			GameManager.Instance.GridManager.TryAddDarkSplashToInventory();
			GameManager.Instance.Player.Sanity.ChangeSanity(this.sanityLossOnHit);
		}
	}

	[SerializeField]
	private VibrationData explodeVibration;

	[SerializeField]
	private float sanityLossOnHit = -0.2f;

	private bool hasTriggered;
}
