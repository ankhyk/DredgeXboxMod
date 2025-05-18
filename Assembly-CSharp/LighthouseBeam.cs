using System;
using DG.Tweening;
using UnityEngine;

public class LighthouseBeam : MonoBehaviour
{
	private void Awake()
	{
		this.origScale = base.transform.localScale;
		this.origFrontFacingPos = this.frontFacingBeam.localPosition;
	}

	private void Start()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler<bool>("TogglePointToFinalePOI", new Action<bool>(this.TogglePointToFinalePOI));
	}

	public void TogglePointToFinalePOI(bool point)
	{
		this.isPointing = point;
		this.constantlyRotateOnY.enabled = !point;
		this.lookAtTarget.enabled = point;
		this.lookAtTarget.target = this.finalePOI;
		if (this.scaleTweener != null)
		{
			this.scaleTweener.Kill(false);
			this.scaleTweener = null;
		}
		this.scaleTweener = base.transform.DOScale(point ? this.pointingScale : this.origScale, 2.5f);
		this.frontFacingBeam.DOLocalMove(point ? this.pointingFrontFacingTransform : this.origFrontFacingPos, 2.5f, false);
	}

	[SerializeField]
	private string nodeTrigger;

	[SerializeField]
	private Transform finalePOI;

	[SerializeField]
	private Transform frontFacingBeam;

	[SerializeField]
	private LookAtTarget lookAtTarget;

	[SerializeField]
	private ConstantlyRotateOnY constantlyRotateOnY;

	[SerializeField]
	private Vector3 pointingScale;

	[SerializeField]
	private Vector3 pointingFrontFacingTransform;

	private Tweener scaleTweener;

	private bool isPointing;

	private Vector3 origScale;

	private Vector3 origFrontFacingPos;
}
