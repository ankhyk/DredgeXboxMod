using System;
using UnityEngine;

public class ItemPOI : POI
{
	public IHarvestable Harvestable
	{
		get
		{
			return this.harvestable ?? this.itemPOIData;
		}
		set
		{
			this.harvestable = value;
		}
	}

	public float Stock
	{
		get
		{
			return this.Harvestable.GetStockCount(true);
		}
	}

	public bool IsPlacedPOI
	{
		get
		{
			return false;
		}
	}

	private void Start()
	{
		GameObject particlePrefab = this.Harvestable.GetParticlePrefab();
		if (particlePrefab)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(particlePrefab, base.transform);
			this.harvestParticles = gameObject.GetComponent<HarvestableParticles>();
			this.harvestParticles.Init(false);
		}
		this.OnStockUpdated();
	}

	public override bool CanBeGhostWindTarget()
	{
		return this.canBeGhostWindTarget && this.Harvestable.GetStockCount(false) >= 1f;
	}

	public void OnHarvested(bool deductFromStock)
	{
		if (deductFromStock)
		{
			this.Harvestable.AddStock(-1f, false);
			this.OnStockUpdated();
		}
	}

	public virtual void OnStockUpdated()
	{
		this.poiCollider.enabled = this.Harvestable.IsHarvestable() == HarvestQueryEnum.VALID;
		this.intermittentSFXPlayer.enabled = this.poiCollider.enabled;
		this.harvestParticles.ParticlesAmount = Mathf.FloorToInt(this.Harvestable.GetStockCount(true));
		Action onStockUpdatedAction = this.OnStockUpdatedAction;
		if (onStockUpdatedAction == null)
		{
			return;
		}
		onStockUpdatedAction();
	}

	private void OnDrawGizmos()
	{
		string text = "";
		if (this.Harvestable != null && this.Harvestable.GetFirstItem() != null)
		{
			text = this.Harvestable.GetFirstItem().id;
		}
		if (text.ToLower().Contains("message"))
		{
			Gizmos.DrawIcon(base.transform.position, "message", true);
		}
	}

	private void OnValidate()
	{
		if (this.Harvestable == null)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < this.Harvestable.GetItems().Count; i++)
		{
			text = text + this.Harvestable.GetItems()[i].id + " ";
		}
		base.gameObject.name = string.Format("[{0}] {1}{2}/{3}", new object[]
		{
			this.Harvestable.GetId(),
			text,
			this.Harvestable.GetStartStock(),
			this.Harvestable.GetMaxStock()
		});
	}

	public void CreateData()
	{
	}

	[SerializeField]
	private IntermittentSFXPlayer intermittentSFXPlayer;

	[SerializeField]
	private Collider poiCollider;

	private HarvestableParticles harvestParticles;

	[SerializeField]
	protected ItemPOIDataModel itemPOIData;

	[HideInInspector]
	[SerializeField]
	protected IHarvestable harvestable;

	[HideInInspector]
	public Action OnStockUpdatedAction;
}
