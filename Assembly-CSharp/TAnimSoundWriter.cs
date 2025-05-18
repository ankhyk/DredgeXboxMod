using System;
using Febucci.Attributes;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Febucci/TextAnimator/SoundWriter")]
[RequireComponent(typeof(TAnimPlayerBase))]
public class TAnimSoundWriter : MonoBehaviour
{
	private void Awake()
	{
		if (this.source == null || this.sounds.Length == 0)
		{
			return;
		}
		this.source.playOnAwake = false;
		this.source.loop = false;
		TAnimPlayerBase component = base.GetComponent<TAnimPlayerBase>();
		if (component != null)
		{
			component.onCharacterVisible.AddListener(new UnityAction<char>(this.OnCharacter));
		}
		this.clipIndex = (this.randomSequence ? global::UnityEngine.Random.Range(0, this.sounds.Length) : 0);
	}

	private void OnCharacter(char character)
	{
		if (Time.time - this.latestTimePlayed <= this.minSoundDelay)
		{
			return;
		}
		this.source.clip = this.sounds[this.clipIndex];
		this.source.pitch = global::UnityEngine.Random.Range(this.pitchMin, this.pitchMax);
		if (this.interruptPreviousSound)
		{
			this.source.Play();
		}
		else
		{
			this.source.PlayOneShot(this.source.clip);
		}
		if (this.randomSequence)
		{
			this.clipIndex = global::UnityEngine.Random.Range(0, this.sounds.Length);
		}
		else
		{
			this.clipIndex++;
			if (this.clipIndex >= this.sounds.Length)
			{
				this.clipIndex = 0;
			}
		}
		this.latestTimePlayed = Time.time;
	}

	[Header("References")]
	public AudioSource source;

	[Header("Management")]
	[Tooltip("How much time has to pass before playing the next sound")]
	[SerializeField]
	[MinValue(0f)]
	private float minSoundDelay = 0.07f;

	[Tooltip("True if you want the new sound to cut the previous one\nFalse if each sound will continue until its end")]
	[SerializeField]
	private bool interruptPreviousSound = true;

	[Header("Audio Clips")]
	[Tooltip("True if sounds will be picked random from the array\nFalse if they'll be chosen in order")]
	[SerializeField]
	private bool randomSequence;

	[SerializeField]
	private AudioClip[] sounds = new AudioClip[0];

	private float latestTimePlayed = -1f;

	private int clipIndex;

	[SerializeField]
	private float pitchMin;

	[SerializeField]
	private float pitchMax;
}
