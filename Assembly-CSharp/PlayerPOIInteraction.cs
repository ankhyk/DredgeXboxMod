using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPOIInteraction : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnHarvestModeToggled += this.OnHarvestModeToggled;
		GameEvents.Instance.OnFinaleCutsceneStarted += this.OnFinaleCutsceneStarted;
		GameEvents.Instance.OnTeleportAnchorRemoved += this.OnTeleportAnchorRemoved;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnHarvestModeToggled -= this.OnHarvestModeToggled;
		GameEvents.Instance.OnFinaleCutsceneStarted -= this.OnFinaleCutsceneStarted;
		GameEvents.Instance.OnTeleportAnchorRemoved -= this.OnTeleportAnchorRemoved;
	}

	private void OnFinaleCutsceneStarted()
	{
		this.locked = true;
		this.teleportAnchorHandler.Deactivate();
		this.harvestHandler.Deactivate();
		this.itemHandler.Deactivate();
		this.inspectHandler.Deactivate();
		this.teleportAnchorHandler.Deactivate();
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock == null)
		{
			this.OnPOIsChanged(false);
		}
	}

	private void OnHarvestModeToggled(bool showing)
	{
		if (!showing)
		{
			base.StartCoroutine(this.DelayedEnable());
		}
	}

	private void OnTeleportAnchorRemoved()
	{
		base.StartCoroutine(this.DelayedEnable());
	}

	private IEnumerator DelayedEnable()
	{
		yield return new WaitForEndOfFrame();
		this.OnPOIsChanged(true);
		yield break;
	}

	private void OnTriggerEnter(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerEnter(other, base.gameObject, new ReliableOnTriggerExit._OnTriggerExit(this.OnTriggerExit));
		POI component = other.gameObject.GetComponent<POI>();
		if (component)
		{
			this.currentPOIs.Add(component);
			this.OnPOIsChanged(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerExit(other, base.gameObject);
		POI component = other.gameObject.GetComponent<POI>();
		if (component)
		{
			this.currentPOIs.Remove(component);
			this.OnPOIsChanged(true);
		}
	}

	private void OnPOIsChanged(bool showIndicator)
	{
		if (this.locked)
		{
			GameManager.Instance.UI.InteractPointUI.SetCurrentPOI(null);
			return;
		}
		if (!GameManager.Instance.IsPlaying)
		{
			return;
		}
		POI poi = null;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		for (int i = 0; i < this.currentPOIs.Count; i++)
		{
			POI poi2 = this.currentPOIs[i];
			if (!(poi2 == null))
			{
				if (poi2 is TeleportAnchorPOI && (poi2 as TeleportAnchorPOI).PairedAnchor)
				{
					flag = true;
					poi = poi2;
				}
				if (!flag && !flag2 && poi2 is DockPOI)
				{
					flag3 = true;
					poi = poi2;
				}
				if (!flag && !flag2 && poi2 is ItemPOI)
				{
					flag4 = true;
					poi = poi2;
				}
				if (!flag && !flag2 && poi2 is ConversationPOI)
				{
					flag5 = true;
					poi = poi2;
				}
				else if (!flag && poi2 is HarvestPOI && (poi2 as HarvestPOI).IsHarvestable == HarvestQueryEnum.VALID && (!flag2 || (flag2 && (poi2 as HarvestPOI).IsCrabPotPOI)))
				{
					flag2 = true;
					poi = poi2;
				}
			}
		}
		if (flag)
		{
			if (this.dockingHandler.IsHandlerActive)
			{
				this.dockingHandler.Deactivate();
			}
			if (this.itemHandler.IsHandlerActive)
			{
				this.itemHandler.Deactivate();
			}
			if (this.inspectHandler.IsHandlerActive)
			{
				this.inspectHandler.Deactivate();
			}
			if (this.harvestHandler.IsHandlerActive)
			{
				this.harvestHandler.Deactivate();
			}
			this.teleportAnchorHandler.Activate(poi as TeleportAnchorPOI);
		}
		else if (flag2)
		{
			if (this.dockingHandler.IsHandlerActive)
			{
				this.dockingHandler.Deactivate();
			}
			if (this.itemHandler.IsHandlerActive)
			{
				this.itemHandler.Deactivate();
			}
			if (this.inspectHandler.IsHandlerActive)
			{
				this.inspectHandler.Deactivate();
			}
			if (this.teleportAnchorHandler.IsHandlerActive)
			{
				this.teleportAnchorHandler.Deactivate();
			}
			if (!GameManager.Instance.UI.IsHarvesting)
			{
				this.harvestHandler.Activate(poi as HarvestPOI);
			}
		}
		else if (flag4)
		{
			this.itemHandler.Activate(poi as ItemPOI);
		}
		else if (flag3)
		{
			this.dockingHandler.Activate(poi as DockPOI);
		}
		else if (flag5)
		{
			this.inspectHandler.Activate(poi as ConversationPOI);
		}
		if (!flag3 && this.dockingHandler.IsHandlerActive)
		{
			this.dockingHandler.Deactivate();
		}
		if (!flag2 && this.harvestHandler.IsHandlerActive)
		{
			this.harvestHandler.Deactivate();
		}
		if (!flag4 && this.itemHandler.IsHandlerActive)
		{
			this.itemHandler.Deactivate();
		}
		if (!flag5 && this.inspectHandler.IsHandlerActive)
		{
			this.inspectHandler.Deactivate();
		}
		if (!flag && this.teleportAnchorHandler.IsHandlerActive)
		{
			this.teleportAnchorHandler.Deactivate();
		}
		if (showIndicator && GameManager.Instance.UI.InteractPointUI)
		{
			GameManager.Instance.UI.InteractPointUI.SetCurrentPOI(poi);
		}
	}

	[SerializeField]
	private DockPOIHandler dockingHandler;

	[SerializeField]
	private HarvestPOIHandler harvestHandler;

	[SerializeField]
	private ItemPOIHandler itemHandler;

	[SerializeField]
	private InspectPOIHandler inspectHandler;

	[SerializeField]
	private TeleportAnchorPOIHandler teleportAnchorHandler;

	private bool locked;

	private List<POI> currentPOIs = new List<POI>();
}
