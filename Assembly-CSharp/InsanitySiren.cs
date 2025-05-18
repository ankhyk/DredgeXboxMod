using System;
using System.Collections;
using UnityEngine;

public class InsanitySiren : MonoBehaviour
{
	private void Start()
	{
		this.camTransform = Camera.main.gameObject.transform;
		base.StartCoroutine(this.CheckLineOfSight());
	}

	private IEnumerator CheckLineOfSight()
	{
		while (this.canPlay)
		{
			Vector3 position = this.camTransform.position;
			position.y = 0f;
			Vector3 vector = position - base.transform.position;
			float magnitude = vector.magnitude;
			if (magnitude < this.minDistance)
			{
				this.canPlay = false;
			}
			if (this.isVisible && Time.time - this.timeOfLastPlay > this.delayBetweenOccurances && magnitude < this.maxDistance && !Physics.Raycast(base.transform.position, vector.normalized, vector.magnitude, this.visibilityRaycastLayerMask))
			{
				this.timeOfLastPlay = Time.time;
				this.audioSource.pitch = global::UnityEngine.Random.Range(0.75f, 1f);
				if (!this.audioOnly)
				{
					base.Invoke("PlayAudioDelayed", magnitude / 500f);
					this.particlesToPlay.Play();
				}
				else
				{
					this.PlayAudioDelayed();
				}
			}
			yield return new WaitForSeconds(3f);
		}
		yield break;
	}

	private void PlayAudioDelayed()
	{
		this.audioSource.Play();
	}

	private void OnBecameVisible()
	{
		this.isVisible = true;
	}

	private void OnBecameInvisible()
	{
		this.isVisible = false;
	}

	[SerializeField]
	private bool audioOnly;

	[SerializeField]
	private float maxDistance = 150f;

	[SerializeField]
	private float minDistance = 30f;

	[SerializeField]
	private LayerMask visibilityRaycastLayerMask;

	[SerializeField]
	private float delayBetweenOccurances = 20f;

	[SerializeField]
	private ParticleSystem particlesToPlay;

	[SerializeField]
	private AudioSource audioSource;

	private bool canPlay = true;

	private float timeOfLastPlay = -1000f;

	private bool isVisible;

	private Transform camTransform;
}
