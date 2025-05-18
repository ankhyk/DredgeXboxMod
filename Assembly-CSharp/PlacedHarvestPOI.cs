using System;
using UnityEngine;

public class PlacedHarvestPOI : HarvestPOI
{
	private void Awake()
	{
		if (!this.isDeployed)
		{
			if (GameSceneInitializer.Instance.IsDone())
			{
				this.animator.SetTrigger("deploy");
			}
			this.isDeployed = true;
		}
	}

	protected override void AdjustStockLevels(float newGameTime)
	{
		base.AdjustStockLevels(newGameTime);
		this.didJustBreak = this.harvestable.AdjustDurability(newGameTime);
		if (this.didJustBreak)
		{
			this.OnStockUpdated();
		}
	}

	public override void OnStockUpdated()
	{
		base.OnStockUpdated();
		this.UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		if (this.harvestable.GetDurability() <= 0f)
		{
			this.readyObj.SetActive(false);
			this.idleObj.SetActive(false);
			this.brokenObj.SetActive(true);
			return;
		}
		if (this.harvestable.GetStockCount(false) >= 1f)
		{
			this.readyObj.SetActive(true);
			this.idleObj.SetActive(false);
			this.brokenObj.SetActive(false);
			return;
		}
		this.readyObj.SetActive(false);
		this.idleObj.SetActive(true);
		this.brokenObj.SetActive(false);
	}

	[SerializeField]
	private GameObject idleObj;

	[SerializeField]
	private GameObject readyObj;

	[SerializeField]
	private GameObject brokenObj;

	[SerializeField]
	private Animator animator;

	private bool didJustBreak;

	private bool isDeployed;
}
