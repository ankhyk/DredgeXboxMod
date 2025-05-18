using System;
using UnityEngine;

public class HarvestPOI : POI
{
	public IHarvestable Harvestable
	{
		get
		{
			return this.harvestable ?? this.harvestPOIData;
		}
		set
		{
			this.harvestable = value;
		}
	}

	public HarvestQueryEnum IsHarvestable
	{
		get
		{
			return this.Harvestable.IsHarvestable();
		}
	}

	public float Stock
	{
		get
		{
			return this.Harvestable.GetStockCount(false);
		}
	}

	public float MaxStock
	{
		get
		{
			return this.Harvestable.GetMaxStock();
		}
	}

	public bool IsCrabPotPOI
	{
		get
		{
			return this.Harvestable is SerializedCrabPotPOIData;
		}
	}

	public bool IsBaitPOI
	{
		get
		{
			return this.Harvestable is BaitPOIDataModel;
		}
	}

	public bool IsDredgePOI
	{
		get
		{
			return this.harvestable.GetHarvestType() == HarvestableType.DREDGE;
		}
	}

	public bool IsCurrentlySpecial
	{
		get
		{
			return this.isCurrentlySpecial;
		}
	}

	public HarvestPOIDataModel HarvestPOIData
	{
		get
		{
			return this.harvestPOIData;
		}
		set
		{
			this.harvestPOIData = value;
		}
	}

	private void Awake()
	{
		if (this.harvestPOIData != null)
		{
			this.harvestable = this.harvestPOIData;
		}
	}

	private void Start()
	{
		if (this.harvestPOIData != null)
		{
			this.harvestable = this.harvestPOIData;
		}
		GameObject particlePrefab = this.harvestParticlePrefab;
		if (particlePrefab == null)
		{
			particlePrefab = this.Harvestable.GetParticlePrefab();
		}
		if (particlePrefab != null)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(particlePrefab, base.transform);
			this.harvestParticles = gameObject.GetComponent<HarvestableParticles>();
			HarvestableItemData harvestableItemData = null;
			if (this.harvestPOIData != null)
			{
				harvestableItemData = this.harvestPOIData.GetFirstHarvestableItem();
			}
			if (harvestableItemData)
			{
				this.harvestParticles.Init(harvestableItemData.requiresAdvancedEquipment);
			}
			else
			{
				this.harvestParticles.Init(false);
			}
		}
		if (this.harvestParticles)
		{
			ItemData firstItem = this.harvestable.GetFirstItem();
			if (firstItem && firstItem.overrideHarvestParticleDepth)
			{
				this.harvestParticles.SetHarvestParticleOverride(firstItem.harvestParticleDepthOffset, firstItem.flattenParticleShape);
			}
		}
		this.OnDayNightChanged();
		this.prevRealTimeOfStockCheck = float.NegativeInfinity;
		this.cachedWasDaytime = GameManager.Instance.Time.IsDaytime;
		this.cachedDaytimeCheckTime = GameManager.Instance.Time.TimeAndDay;
		if (this.harvestable != null)
		{
			this.harvestable.Init(this.cachedDaytimeCheckTime);
		}
	}

	public override bool CanBeGhostWindTarget()
	{
		return this.canBeGhostWindTarget && this.Harvestable.GetStockCount(false) >= 1f;
	}

	private void OnDayNightChanged()
	{
		if (this.Harvestable == null)
		{
			Debug.LogWarning("OnDayNightChanged() harvestable is null, exiting early.");
			return;
		}
		this.OnStockUpdated();
		bool flag = this.isCurrentlySpecial;
		this.isCurrentlySpecial = this.Harvestable.RollForSpecial();
		this.shouldUpdateSpecialParticles = flag != this.isCurrentlySpecial;
	}

	public void SetIsCurrentlySpecial(bool val)
	{
		this.isCurrentlySpecial = val;
		this.shouldUpdateSpecialParticles = true;
	}

	private void OnEnable()
	{
		this.shouldUpdateSpecialParticles = true;
	}

	private void LateUpdate()
	{
		if (this.shouldUpdateSpecialParticles && this.harvestParticles)
		{
			this.harvestParticles.SetSpecialStatus(this.isCurrentlySpecial);
			this.shouldUpdateSpecialParticles = false;
		}
	}

	public void OnHarvested(bool deductFromStock)
	{
		if (deductFromStock)
		{
			this.Harvestable.OnHarvested(deductFromStock, !this.IsDredgePOI);
			this.OnStockUpdated();
		}
	}

	public void AddStock(float count, bool countTowardsDepletionAchievement = true)
	{
		this.Harvestable.AddStock(count, countTowardsDepletionAchievement && !this.IsDredgePOI);
		this.OnStockUpdated();
	}

	public virtual void OnStockUpdated()
	{
		this.poiCollider.enabled = this.IsHarvestable == HarvestQueryEnum.VALID;
		if (this.harvestParticles)
		{
			this.harvestParticles.ParticlesAmount = Mathf.FloorToInt(this.Harvestable.GetStockCount(false));
		}
		Action onStockUpdatedAction = this.OnStockUpdatedAction;
		if (onStockUpdatedAction == null)
		{
			return;
		}
		onStockUpdatedAction();
	}

	private void Update()
	{
		if (Time.time > this.prevRealTimeOfStockCheck + this.realTimeBetweenStockChecksSec)
		{
			this.prevRealTimeOfStockCheck = Time.time;
			if (this.Harvestable.GetDoesRestock())
			{
				this.AdjustStockLevels(GameManager.Instance.Time.TimeAndDay);
			}
			if (!this.IsCrabPotPOI && (this.Harvestable.GetUsesTimeSpecificStock() || this.Harvestable.CanBeSpecial()))
			{
				if (GameManager.Instance.Time.IsDaytime != this.cachedWasDaytime || GameManager.Instance.Time.TimeAndDay > this.cachedDaytimeCheckTime + 0.5f)
				{
					this.cachedWasDaytime = GameManager.Instance.Time.IsDaytime;
					this.OnDayNightChanged();
				}
				this.cachedDaytimeCheckTime = GameManager.Instance.Time.TimeAndDay;
			}
		}
	}

	protected virtual void AdjustStockLevels(float newGameTime)
	{
		if (this.Harvestable.AdjustStockLevels(newGameTime))
		{
			this.OnStockUpdated();
		}
	}

	[SerializeField]
	private Collider poiCollider;

	[SerializeField]
	private HarvestPOIDataModel harvestPOIData;

	[SerializeField]
	protected GameObject harvestParticlePrefab;

	[HideInInspector]
	[SerializeField]
	protected IHarvestable harvestable;

	private HarvestableParticles harvestParticles;

	private bool isCurrentlySpecial;

	private float prevRealTimeOfStockCheck;

	private float realTimeBetweenStockChecksSec = 1f;

	private bool cachedWasDaytime;

	private float cachedDaytimeCheckTime;

	private bool shouldUpdateSpecialParticles;

	[HideInInspector]
	public Action OnStockUpdatedAction;
}
