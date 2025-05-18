using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class VFXVolumeFader : MonoBehaviour
{
	public float BlendDurationSec
	{
		get
		{
			return this.blendDurationSec;
		}
		set
		{
			this.blendDurationSec = value;
		}
	}

	private void Awake()
	{
		if (this.fadeInOnAwake)
		{
			base.StartCoroutine(this.FadeIn());
		}
	}

	public IEnumerator FadeIn()
	{
		yield return base.StartCoroutine(this.Fade(this.maxBlendDistance));
		yield break;
	}

	public IEnumerator FadeOut()
	{
		yield return base.StartCoroutine(this.Fade(this.minBlendDistance));
		yield break;
	}

	private IEnumerator Fade(float blendDistance)
	{
		if (this.tweener != null)
		{
			this.tweener.Kill(false);
			this.tweener = null;
		}
		bool isDone = false;
		this.tweener = DOTween.To(() => this.volume.blendDistance, delegate(float x)
		{
			this.volume.blendDistance = x;
		}, blendDistance, this.blendDurationSec);
		this.tweener.OnComplete(delegate
		{
			isDone = true;
			this.tweener = null;
		});
		yield return new WaitUntil(() => isDone);
		yield break;
	}

	[SerializeField]
	private Volume volume;

	[SerializeField]
	private bool fadeInOnAwake;

	[SerializeField]
	private float minBlendDistance;

	[SerializeField]
	private float maxBlendDistance;

	[SerializeField]
	private float blendDurationSec;

	private Tweener tweener;
}
