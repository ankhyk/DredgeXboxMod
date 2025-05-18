using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DLC1FinaleScript : MonoBehaviour
{
	private void Awake()
	{
		ApplicationEvents.Instance.OnGameLoaded += this.OnStartup;
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnGameLoaded -= this.OnStartup;
	}

	private void OnStartup()
	{
		if (GameManager.Instance.SaveData.TPRWorldPhase >= 4)
		{
			this.SkipToFinalState();
		}
	}

	private void OnEnable()
	{
		this.meshRenderers.ForEach(delegate(MeshRenderer m)
		{
			Material material = new Material(m.material);
			m.material = material;
		});
	}

	public void MakeIceOpaque()
	{
		this.currentIceOpacityVal = this.meshRenderers[0].material.GetFloat("_IceOpacity");
		DOTween.To(() => this.currentIceOpacityVal, delegate(float x)
		{
			this.currentIceOpacityVal = x;
		}, this.destinationOpacity, this.opacityChangeDuration).OnUpdate(delegate
		{
			this.meshRenderers.ForEach(delegate(MeshRenderer m)
			{
				m.material.SetFloat("_IceOpacity", this.currentIceOpacityVal);
			});
		});
	}

	public void AddStock()
	{
		this.harvestPOI.AddStock(1f, true);
	}

	public void SkipToFinalState()
	{
		this.regularHead.SetActive(false);
		this.cinematicHead.SetActive(true);
		this.meshRenderers.ForEach(delegate(MeshRenderer m)
		{
			m.material.SetFloat("_IceOpacity", this.destinationOpacity);
		});
		this.animator.SetTrigger("skip");
		this.cinematicHead.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>().ForEach(delegate(ParticleSystem p)
		{
			global::UnityEngine.Object.Destroy(p.gameObject);
		});
	}

	public void OnVibrationType1SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType1);
	}

	public void OnVibrationType2SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType2);
	}

	public void OnVibrationType3SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType3);
	}

	public void OnCameraShake1SignalReceived()
	{
		this.OnCameraShakeSignalReceived(0);
	}

	public void OnCameraShake2SignalReceived()
	{
		this.OnCameraShakeSignalReceived(1);
	}

	public void OnCameraShake3SignalReceived()
	{
		this.OnCameraShakeSignalReceived(2);
	}

	private void OnCameraShakeSignalReceived(int index)
	{
		if (index < this.cameraShakes.Count)
		{
			this.cameraShakes[index].GenerateImpulse();
		}
	}

	private void OnVibrationSignalReceived(VibrationData vibrationData)
	{
		GameManager.Instance.VibrationManager.Vibrate(vibrationData, VibrationRegion.WholeBody, true);
	}

	[SerializeField]
	private List<MeshRenderer> meshRenderers;

	[SerializeField]
	private float destinationOpacity;

	[SerializeField]
	private float opacityChangeDuration;

	[SerializeField]
	private GameObject regularHead;

	[SerializeField]
	private GameObject cinematicHead;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private HarvestPOI harvestPOI;

	private float currentIceOpacityVal;

	[Header("Vibration Data")]
	[SerializeField]
	private VibrationData vibrationType1;

	[SerializeField]
	private VibrationData vibrationType2;

	[SerializeField]
	private VibrationData vibrationType3;

	[Header("Camera Shakes")]
	[SerializeField]
	private List<CinemachineImpulseSource> cameraShakes = new List<CinemachineImpulseSource>();
}
