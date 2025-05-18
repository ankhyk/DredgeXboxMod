using System;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class SelectableMapMarker : MonoBehaviour
{
	public virtual void RemoveMarkerFromData()
	{
	}

	public void Select()
	{
		if (this.scaleTweener != null)
		{
			this.scaleTweener.Kill(false);
			this.scaleTweener = null;
		}
		if (this.shine)
		{
			this.shine.Play(true);
		}
		this.scaleTweener = this.markerImage.transform.DOScale(this.selectedScaleFactor, 0.35f).SetUpdate(true);
		this.scaleTweener.OnComplete(delegate
		{
			this.scaleTweener = null;
		});
	}

	public void Deselect()
	{
		if (this.scaleTweener != null)
		{
			this.scaleTweener.Kill(false);
			this.scaleTweener = null;
		}
		if (this.shine)
		{
			this.shine.Stop(true);
		}
		this.scaleTweener = this.markerImage.transform.DOScale(1f, 0.35f).SetUpdate(true);
		this.scaleTweener.OnComplete(delegate
		{
			this.scaleTweener = null;
		});
	}

	[SerializeField]
	protected Image markerImage;

	[SerializeField]
	private UIShiny shine;

	[SerializeField]
	private float selectedScaleFactor;

	private Tweener scaleTweener;
}
