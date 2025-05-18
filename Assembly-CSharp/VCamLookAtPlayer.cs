using System;
using Cinemachine;
using UnityEngine;

public class VCamLookAtPlayer : MonoBehaviour
{
	private void Update()
	{
		if (this.virtualCamera.m_LookAt == null && GameManager.Instance.Player != null)
		{
			this.virtualCamera.m_LookAt = GameManager.Instance.Player.transform;
		}
	}

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
}
