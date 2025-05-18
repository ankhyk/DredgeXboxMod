using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class GridObject : MonoBehaviour
{
	public SpatialItemInstance SpatialItemInstance
	{
		get
		{
			return this.spatialItemInstance;
		}
	}

	public int CurrentRotation
	{
		get
		{
			return this.currentRotation;
		}
		set
		{
			this.currentRotation = value;
		}
	}

	public SpatialItemData ItemData
	{
		get
		{
			SpatialItemInstance spatialItemInstance = this.spatialItemInstance;
			if (spatialItemInstance == null)
			{
				return null;
			}
			return spatialItemInstance.GetItemData<SpatialItemData>();
		}
	}

	public GridUI ParentGrid
	{
		get
		{
			return this.parentGrid;
		}
		set
		{
			this.parentGrid = value;
		}
	}

	public Vector3Int RootCoord
	{
		get
		{
			return new Vector3Int(this.spatialItemInstance.x, this.spatialItemInstance.y, this.spatialItemInstance.z);
		}
	}

	public PresetGridMode HintMode
	{
		get
		{
			return this.hintMode;
		}
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnMaintenanceModeToggled += this.OnMaintenanceModeToggled;
	}

	private void OnDisable()
	{
		SpatialItemInstance spatialItemInstance = this.spatialItemInstance;
		spatialItemInstance.OnDamagedStatusChanged = (Action)Delegate.Remove(spatialItemInstance.OnDamagedStatusChanged, new Action(this.OnDamagedStatusChanged));
		SpatialItemInstance spatialItemInstance2 = this.spatialItemInstance;
		spatialItemInstance2.OnDurabilityRepaired = (Action)Delegate.Remove(spatialItemInstance2.OnDurabilityRepaired, new Action(this.OnDurabilityRepaired));
		SpatialItemInstance spatialItemInstance3 = this.spatialItemInstance;
		spatialItemInstance3.OnInfected = (Action)Delegate.Remove(spatialItemInstance3.OnInfected, new Action(this.AddInfectionVFX));
		GameEvents.Instance.OnMaintenanceModeToggled -= this.OnMaintenanceModeToggled;
	}

	private void Start()
	{
		this.OnMaintenanceModeToggled(GameManager.Instance.GridManager.IsInRepairMode);
	}

	public void SetIsPickedUp(bool isPickedUp)
	{
		if (isPickedUp && this.spatialItemInstance.GetItemData<SpatialItemData>().squishFactor > 0f)
		{
			this.image.material = this.squishyMaterial;
			this.image.material.SetFloat("_SquishFactor", this.spatialItemInstance.GetItemData<SpatialItemData>().squishFactor);
			this.image.useSpriteMesh = true;
			this.virtualPosition = base.transform.position;
			this.velocity = Vector3.zero;
		}
		if (!isPickedUp)
		{
			this.gridObjectImage.transform.eulerAngles = new Vector3(0f, 0f, (float)this.CurrentRotation);
			if (this.spatialItemInstance.GetItemData<SpatialItemData>().squishFactor > 0f)
			{
				this.image.material = null;
			}
		}
		this.isPickedUp = isPickedUp;
	}

	public void SetRootCoord(Vector3Int newRootCoord)
	{
		this.spatialItemInstance.x = newRootCoord.x;
		this.spatialItemInstance.y = newRootCoord.y;
		this.spatialItemInstance.z = newRootCoord.z;
	}

	public void SetPosition(Vector3 position, bool overrideImagePosition)
	{
		(base.transform as RectTransform).anchoredPosition = position;
		if (overrideImagePosition)
		{
			this.gridObjectImage.transform.position = base.transform.position;
		}
	}

	public void SetRotation(float rotation, bool instant)
	{
		base.transform.eulerAngles = new Vector3(0f, 0f, rotation);
		this.CurrentRotation = (int)base.transform.rotation.eulerAngles.z;
		if (instant)
		{
			this.interpolatedImageRotation = (float)this.CurrentRotation;
			this.gridObjectImage.transform.eulerAngles = new Vector3(0f, 0f, this.interpolatedImageRotation);
			return;
		}
		this.velocity = new Vector3(60f, 0f, 0f);
	}

	private void OnMaintenanceModeToggled(bool isActive)
	{
		this.RefreshImageColours();
	}

	public void OnHoverChanged(bool hover)
	{
		GameManager.Instance.SaveData.SetHasSeenItem(this.ItemData.id, true);
		if (this.shine != null)
		{
			this.shine.enabled = false;
		}
	}

	private void RefreshImageColours()
	{
		if (this.ItemData == null || this.ItemData.id == "dmg")
		{
			return;
		}
		if (this.hintMode != PresetGridMode.NONE)
		{
			if (this.hintMode == PresetGridMode.SILHOUETTE || this.hintMode == PresetGridMode.MYSTERY)
			{
				this.image.material = this.silhouetteMaterial;
			}
			return;
		}
		if (GameManager.Instance.GridManager.IsInRepairMode)
		{
			this.image.color = ((this.ItemData.damageMode == DamageMode.DURABILITY && (this.state == GridObjectState.IN_INVENTORY || this.state == GridObjectState.IN_STORAGE)) ? Color.white : this.maintenanceModeFadedColor);
			return;
		}
		this.image.color = Color.white;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(this.virtualPosition, 5f);
	}

	private void FixedUpdate()
	{
		if (this.isPickedUp)
		{
			float num = Vector3.Distance(this.virtualPosition, base.transform.position);
			Vector3 normalized = (base.transform.position - this.virtualPosition).normalized;
			float num2 = num * Time.fixedDeltaTime * this.squishForce;
			this.velocity += normalized * num2;
			this.velocity = Vector3.Lerp(this.velocity, Vector3.zero, Time.fixedDeltaTime * this.squishDamping);
			this.virtualPosition += this.velocity;
			Vector3 vector = Vector3.ClampMagnitude(this.velocity, 20f);
			this.image.material.SetVector("_SquishVelocity", new Vector4(vector.x, vector.y, 0f, 0f));
			this.interpolatedImageRotation = Mathf.LerpAngle(this.interpolatedImageRotation, (float)this.CurrentRotation, Time.deltaTime * this.rotationSpeed);
			this.gridObjectImage.transform.eulerAngles = new Vector3(0f, 0f, this.interpolatedImageRotation);
		}
	}

	public void Init(SpatialItemInstance spatialItemInstance, GridUI parentGrid, GridObjectState state)
	{
		this.spatialItemInstance = spatialItemInstance;
		this.parentGrid = parentGrid;
		this.state = state;
		this.image.sprite = this.ItemData.GetSprite();
		this.image.alphaHitTestMinimumThreshold = 0.1f;
		float cellSize = GameManager.Instance.GridManager.ScaledCellSize;
		this.ItemData.dimensions.ForEach(delegate(Vector2Int cell)
		{
			Vector3 vector = new Vector3(this.transform.position.x + ((float)cell.x * cellSize + cellSize * 0.5f), this.transform.position.y - ((float)cell.y * cellSize + cellSize * 0.5f), this.transform.position.z);
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.gridObjectCellPrefab, this.transform);
			(gameObject.transform as RectTransform).position = vector;
			GridObjectCell component = gameObject.GetComponent<GridObjectCell>();
			if (component)
			{
				component.ParentObject = this;
				this.gridObjectCells.Add(component);
			}
		});
		this.hasAddedInfectionVFX = false;
		this.AddInfectionVFX();
		spatialItemInstance.OnInfected = (Action)Delegate.Combine(spatialItemInstance.OnInfected, new Action(this.AddInfectionVFX));
		this.OnDamagedStatusChanged();
		spatialItemInstance.OnDamagedStatusChanged = (Action)Delegate.Combine(spatialItemInstance.OnDamagedStatusChanged, new Action(this.OnDamagedStatusChanged));
		if (this.ItemData.damageMode == DamageMode.DURABILITY)
		{
			spatialItemInstance.OnDurabilityRepaired = (Action)Delegate.Combine(spatialItemInstance.OnDurabilityRepaired, new Action(this.ShowRepairVFX));
		}
		if (state == GridObjectState.IN_SHOP && this.shine != null && this.ItemData.itemType == ItemType.EQUIPMENT && !this.ItemData.BuyableWithoutResearch && !GameManager.Instance.SaveData.GetHasSeenItem(this.ItemData.id))
		{
			this.shine.enabled = true;
		}
	}

	public void AddInfectionVFX()
	{
		if (this.hasAddedInfectionVFX)
		{
			return;
		}
		float scaledCellSize = GameManager.Instance.GridManager.ScaledCellSize;
		if (this.spatialItemInstance is FishItemInstance && (this.spatialItemInstance as FishItemInstance).isInfected)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			Vector2 pivot = rectTransform.pivot;
			Vector2 vector = rectTransform.localScale;
			Vector3 vector2 = rectTransform.anchoredPosition;
			Quaternion rotation = rectTransform.rotation;
			rectTransform.SetPivot(new Vector2(0f, 1f));
			rectTransform.rotation = Quaternion.identity;
			FishItemData itemData = (this.spatialItemInstance as FishItemInstance).GetItemData<FishItemData>();
			for (int i = 0; i < this.ItemData.dimensions.Count; i++)
			{
				Vector2Int vector2Int = this.ItemData.dimensions[i];
				if (!itemData.cellsExcludedFromDisplayingInfection.Contains(vector2Int))
				{
					Vector3 vector3 = new Vector3(base.transform.position.x * vector.x + ((float)vector2Int.x * scaledCellSize + scaledCellSize * 0.5f), base.transform.position.y * vector.y - ((float)vector2Int.y * scaledCellSize + scaledCellSize * 0.5f));
					(global::UnityEngine.Object.Instantiate<GameObject>(this.infectedObjectCellPrefab, base.transform).transform as RectTransform).position = vector3;
				}
			}
			rectTransform.pivot = pivot;
			rectTransform.anchoredPosition = vector2;
			rectTransform.rotation = rotation;
			this.hasAddedInfectionVFX = true;
		}
	}

	public void ShowRepairVFX()
	{
		if (this.shine)
		{
			this.shine.m_Player.initialPlayDelay = 0f;
			this.shine.m_Player.loop = false;
			this.shine.enabled = true;
			this.shine.Play(true);
		}
	}

	public void SetHintMode(PresetGridMode presetGridMode)
	{
		this.hintMode = presetGridMode;
		this.RefreshImageColours();
	}

	private void OnDamagedStatusChanged()
	{
		if (this.ItemData.damageMode == DamageMode.OPERATION)
		{
			this.image.material = (this.spatialItemInstance.GetIsOnDamagedCell() ? this.greyscaleMaterial : null);
		}
	}

	private void OnDurabilityRepaired()
	{
		SpatialItemInstance spatialItemInstance = this.spatialItemInstance;
		spatialItemInstance.OnDurabilityRepaired = (Action)Delegate.Remove(spatialItemInstance.OnDurabilityRepaired, new Action(this.ShowRepairVFX));
		this.ShowRepairVFX();
	}

	public GridCell GetCurrentRootCell()
	{
		GridCell gridCell;
		GridObject gridObject;
		this.gridObjectCells[0].DoHitTest(out gridCell, out gridObject);
		return gridCell;
	}

	public bool GetCurrentRootPositionWithRotation(out Vector3Int currentRootPosition)
	{
		currentRootPosition = default(Vector3Int);
		GridCell currentRootCell = this.GetCurrentRootCell();
		if (currentRootCell)
		{
			currentRootPosition = new Vector3Int(currentRootCell.GridCellData.x, currentRootCell.GridCellData.y, this.currentRotation);
			int x = this.ItemData.dimensions[0].x;
			int y = this.ItemData.dimensions[0].y;
			if (this.currentRotation == 0)
			{
				currentRootPosition.x -= x;
				currentRootPosition.y -= y;
			}
			else if (this.currentRotation == 90)
			{
				currentRootPosition.x -= y;
				currentRootPosition.y -= x;
			}
			else if (this.currentRotation == 180)
			{
				currentRootPosition.x -= x;
				currentRootPosition.y += y;
			}
			else if (this.currentRotation == 270)
			{
				currentRootPosition.x += y;
				currentRootPosition.y -= x;
			}
			return true;
		}
		return false;
	}

	public Vector3Int GetRootPositionWithRotation()
	{
		return new Vector3Int(this.rootCoord.x, this.rootCoord.y, this.rootCoord.z);
	}

	public GridObjectPlacementResult GetPlacementResult()
	{
		List<GridCell> list = null;
		List<GridObject> list2 = null;
		this.DoHitTest(out list, out list2);
		GridObjectPlacementResult gridObjectPlacementResult = default(GridObjectPlacementResult);
		gridObjectPlacementResult.objects = list2;
		gridObjectPlacementResult.cells = list;
		gridObjectPlacementResult.placementCellsAreUndamaged = !list.Exists(delegate(GridCell gc)
		{
			GridObject occupyingUnderlayObject = gc.OccupyingUnderlayObject;
			return occupyingUnderlayObject != null && occupyingUnderlayObject.ItemData.itemType == ItemType.DAMAGE;
		});
		gridObjectPlacementResult.placementUnobstructed = gridObjectPlacementResult.objects.Count == 0;
		gridObjectPlacementResult.placementCellsValid = gridObjectPlacementResult.cells.Count == this.ItemData.GetSize();
		gridObjectPlacementResult.placementCellsAcceptObject = gridObjectPlacementResult.cells.TrueForAll((GridCell gc) => gc.GridCellData.DoesCellAcceptItem(this.ItemData));
		gridObjectPlacementResult.placementCellsAreInStorageTray = gridObjectPlacementResult.cells.Any((GridCell gc) => gc.gridCellState == GridObjectState.IN_TRAY);
		return gridObjectPlacementResult;
	}

	public void DoHitTest(out List<GridCell> cellsHit, out List<GridObject> objectsHit)
	{
		cellsHit = new List<GridCell>();
		objectsHit = new List<GridObject>();
		for (int i = 0; i < this.gridObjectCells.Count; i++)
		{
			GridObjectCell gridObjectCell = this.gridObjectCells[i];
			GridCell gridCell = null;
			GridObject gridObject = null;
			gridObjectCell.DoHitTest(out gridCell, out gridObject);
			if (gridCell && !cellsHit.Contains(gridCell))
			{
				cellsHit.Add(gridCell);
			}
			if (gridObject && !objectsHit.Contains(gridObject))
			{
				objectsHit.Add(gridObject);
			}
		}
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private GameObject gridObjectImage;

	[SerializeField]
	private GameObject gridObjectCellPrefab;

	[SerializeField]
	private GameObject infectedObjectCellPrefab;

	[SerializeField]
	private Material squishyMaterial;

	[SerializeField]
	private UIShiny shine;

	[SerializeField]
	public GridObjectState state;

	[SerializeField]
	private Gradient harvestColorGradient;

	[SerializeField]
	private Material greyscaleMaterial;

	[SerializeField]
	private Material silhouetteMaterial;

	[SerializeField]
	private Color maintenanceModeFadedColor;

	[SerializeField]
	private float rotationSpeed = 25f;

	[SerializeField]
	private float squishForce = 25f;

	[SerializeField]
	private float squishDamping = 15f;

	private SpatialItemInstance spatialItemInstance;

	private GridUI parentGrid;

	private bool isPickedUp;

	private Vector3Int rootCoord;

	private float interpolatedImageRotation;

	private List<GridObjectCell> gridObjectCells = new List<GridObjectCell>();

	private int currentRotation;

	private Vector3 velocity = Vector3.zero;

	private Vector3 virtualPosition;

	private PresetGridMode hintMode;

	private bool hasAddedInfectionVFX;
}
