using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
	public CinemachineVirtualCamera VirtualCamera
	{
		get
		{
			return this.virtualCamera;
		}
	}

	private void Awake()
	{
		this.dollyCart = base.GetComponent<CinemachineDollyCart>();
		this.virtualCamera = base.GetComponent<CinemachineVirtualCamera>();
	}

	public void Play(float speed, float delay)
	{
		this.dollyCart.m_Position = 0f;
		this.virtualCamera.enabled = true;
		if (delay > 0f)
		{
			this.dollyCart.m_Speed = 0f;
			base.StartCoroutine(this.DoPlay(speed, delay));
			return;
		}
		this.dollyCart.m_Speed = speed;
	}

	private IEnumerator DoPlay(float speed, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.dollyCart.m_Speed = speed;
		yield break;
	}

	private void Update()
	{
		if (this.virtualCamera.enabled)
		{
			Shader.SetGlobalVector("_FogCenter", new Vector4(this.virtualCamera.transform.position.x, this.virtualCamera.transform.position.y, this.virtualCamera.transform.position.z, 0f));
		}
	}

	[SerializeField]
	public string id;

	private CinemachineDollyCart dollyCart;

	private CinemachineVirtualCamera virtualCamera;
}
