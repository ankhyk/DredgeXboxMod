using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeleportAnchorPOI : POI
{
	public TeleportAnchorPOI PairedAnchor
	{
		get
		{
			return this.pairedAnchor;
		}
	}

	public bool CanBeRetrieved
	{
		get
		{
			return this.canBeRetrieved;
		}
		set
		{
			this.canBeRetrieved = value;
		}
	}

	public bool PendingDelete
	{
		get
		{
			return this._pendingDelete;
		}
		set
		{
			this._pendingDelete = value;
		}
	}

	private void Awake()
	{
		this.LookForAnchorPair();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnTeleportAnchorAdded += this.LookForAnchorPair;
		GameEvents.Instance.OnTeleportAnchorRemoved += this.ClearAnchorPair;
		GameEvents.Instance.OnFinaleCutsceneStarted += this.OnFinaleCutsceneStarted;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTeleportAnchorAdded -= this.LookForAnchorPair;
		GameEvents.Instance.OnTeleportAnchorRemoved -= this.ClearAnchorPair;
		GameEvents.Instance.OnFinaleCutsceneStarted -= this.OnFinaleCutsceneStarted;
	}

	private void LookForAnchorPair()
	{
		List<TeleportAnchorPOI> list = global::UnityEngine.Object.FindObjectsOfType<TeleportAnchorPOI>(false).ToList<TeleportAnchorPOI>();
		this.pairedAnchor = list.FirstOrDefault((TeleportAnchorPOI a) => a != this && !a.PendingDelete);
		this.effects.SetActive(this.pairedAnchor != null);
	}

	private void ClearAnchorPair()
	{
		this.pairedAnchor = null;
		this.effects.SetActive(false);
	}

	private void OnFinaleCutsceneStarted()
	{
		this.ClearAnchorPair();
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private bool canBeRetrieved;

	[SerializeField]
	private GameObject effects;

	private TeleportAnchorPOI pairedAnchor;

	private bool _pendingDelete;
}
