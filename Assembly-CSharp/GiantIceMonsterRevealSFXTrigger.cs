using System;
using UnityEngine;

public class GiantIceMonsterRevealSFXTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		this.TryPlaySFX();
		base.gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision other)
	{
		this.TryPlaySFX();
		base.gameObject.SetActive(false);
	}

	private void TryPlaySFX()
	{
		if (!GameManager.Instance.DialogueRunner.GetHasVisitedNode("DLC1_DockSouth") && GameManager.Instance.SettingsSaveData.heartbeatSFX == 0)
		{
			this.audioSource.Play();
		}
	}

	[SerializeField]
	private AudioSource audioSource;
}
