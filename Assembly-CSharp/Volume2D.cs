using System;
using UnityEngine;

public class Volume2D : MonoBehaviour
{
	public float CloseDist
	{
		get
		{
			return this.closeDist;
		}
		set
		{
			this.closeDist = value;
		}
	}

	public float AttenuationDistance
	{
		get
		{
			return this.attenuationDistance;
		}
		set
		{
			this.attenuationDistance = value;
		}
	}

	public float CloseVolume
	{
		get
		{
			return this.closeVolume;
		}
		set
		{
			this.closeVolume = value;
		}
	}

	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (!this.usePlayerDistance)
		{
			this.listenerTransform = Camera.main.transform;
		}
	}

	private void Update()
	{
		if (this.listenerTransform == null)
		{
			if (this.usePlayerDistance && GameManager.Instance.Player)
			{
				this.listenerTransform = GameManager.Instance.Player.transform;
			}
			return;
		}
		float num = Vector3.Distance(base.transform.position, this.listenerTransform.position);
		this.farDist = this.closeDist + this.attenuationDistance;
		if (num < this.closeDist)
		{
			this.audioSource.volume = this.closeVolume;
			this.shouldBePlaying = true;
		}
		else if (num > this.farDist)
		{
			this.audioSource.volume = 0f;
			this.shouldBePlaying = false;
		}
		else
		{
			this.distanceProp = Mathf.InverseLerp(this.closeDist, this.farDist, num);
			this.audioSource.volume = Mathf.Lerp(this.closeVolume, this.farVolume, this.distanceProp);
			this.shouldBePlaying = true;
		}
		if (this.smartToggle && this.shouldBePlaying != this.audioSource.isPlaying)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.Stop();
				this.hasStoppedAutomatically = true;
				return;
			}
			if (this.hasStoppedAutomatically)
			{
				this.audioSource.Play();
				this.hasStoppedAutomatically = false;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.closeDist);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, this.closeDist + this.attenuationDistance);
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float closeDist = 50f;

	[SerializeField]
	private float attenuationDistance = 50f;

	[SerializeField]
	private float closeVolume = 1f;

	[SerializeField]
	private float farVolume;

	[SerializeField]
	private bool usePlayerDistance = true;

	[SerializeField]
	private bool smartToggle = true;

	private Transform listenerTransform;

	private float farDist;

	private bool hasStoppedAutomatically;

	private bool shouldBePlaying;

	private float distanceProp;
}
