using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridCell : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
{
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

	public GridObject OccupyingObject
	{
		get
		{
			return this.occupyingObject;
		}
	}

	public GridObject OccupyingHintObject
	{
		get
		{
			return this.occupyingHintObject;
		}
	}

	public GridObject OccupyingUnderlayObject
	{
		get
		{
			return this.occupyingUnderlayObject;
		}
		set
		{
			this.occupyingUnderlayObject = value;
		}
	}

	public GridCellData GridCellData
	{
		get
		{
			return this.gridCellData;
		}
	}

	public Vector2Int Coord
	{
		get
		{
			return new Vector2Int(this.gridCellData.x, this.gridCellData.y);
		}
	}

	public void SetCellState(CellState cellState)
	{
		this.cellState = cellState;
		this._isDirty = true;
	}

	public void Init(GridCellData gcd, string name, GridUI parentGrid, GridObjectState gridCellState, ItemType itemType, ItemSubtype itemSubtype)
	{
		this.gridCellData = gcd;
		base.name = name;
		this.parentGrid = parentGrid;
		this.gridCellState = gridCellState;
		this.acceptedItemType = itemType;
		this.acceptedItemSubtype = itemSubtype;
		if (gridCellState == GridObjectState.IN_INVENTORY && (this.acceptedItemSubtype.HasFlag(ItemSubtype.ENGINE) || this.acceptedItemSubtype.HasFlag(ItemSubtype.ROD) || this.acceptedItemSubtype.HasFlag(ItemSubtype.LIGHT)))
		{
			GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
			GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnItemHoveredChanged += this.OnItemHoveredChanged;
			this.didAddHighlightListeners = true;
		}
		this.RefreshAcceptedItemTypeImage();
		if (this.gridCellData.IsHidden)
		{
			this.hitImage.raycastTarget = false;
			this.fillImage.color = this.hiddenColor;
			this.backplateImage.color = this.hiddenColor;
		}
		if (GameManager.Instance.GridManager.CurrentlyHeldObject)
		{
			this.OnItemPickedUp(GameManager.Instance.GridManager.CurrentlyHeldObject);
		}
	}

	private void OnDestroy()
	{
		if (this.didAddHighlightListeners)
		{
			GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
			GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnItemHoveredChanged -= this.OnItemHoveredChanged;
			this.didAddHighlightListeners = false;
		}
	}

	private void OnItemHoveredChanged(GridObject gridObject)
	{
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			if (gridObject && gridObject.HintMode == PresetGridMode.NONE && gridObject.state != GridObjectState.IN_INVENTORY && (gridObject.ItemData.itemSubtype == ItemSubtype.LIGHT || gridObject.ItemData.itemSubtype == ItemSubtype.ROD || gridObject.ItemData.itemSubtype == ItemSubtype.ENGINE || gridObject.ItemData.itemSubtype == ItemSubtype.NET) && this.acceptedItemSubtype.HasFlag(gridObject.ItemData.itemSubtype))
			{
				this.ToggleHighlightAcceptedItemTypeImage(true);
				return;
			}
			this.ToggleHighlightAcceptedItemTypeImage(false);
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		if ((gridObject.ItemData.itemSubtype == ItemSubtype.LIGHT || gridObject.ItemData.itemSubtype == ItemSubtype.ROD || gridObject.ItemData.itemSubtype == ItemSubtype.ENGINE || gridObject.ItemData.itemSubtype == ItemSubtype.NET) && this.acceptedItemSubtype.HasFlag(gridObject.ItemData.itemSubtype))
		{
			this.ToggleHighlightAcceptedItemTypeImage(true);
		}
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			this.ToggleHighlightAcceptedItemTypeImage(false);
		}
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.ToggleHighlightAcceptedItemTypeImage(false);
	}

	private void ToggleHighlightAcceptedItemTypeImage(bool show)
	{
		if (this.acceptedItemType != ItemType.ALL && this.gridCellState == GridObjectState.IN_INVENTORY)
		{
			if (show)
			{
				this.acceptedTypeImage.gameObject.SetActive(true);
				this.acceptedTypeImage.color = this.acceptedTypeImageHighlightTint;
				return;
			}
			this.RefreshAcceptedItemTypeImage();
		}
	}

	public void ToggleUpgradePreviewImage(bool show, ItemSubtype itemSubtype)
	{
		this.isShowingPreviewImage = show;
		if (show)
		{
			this.acceptedTypeImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			this.acceptedTypeImage.sprite = this.GetSpriteForItemType(itemSubtype);
			this.acceptedTypeImage.gameObject.SetActive(true);
			return;
		}
		this.acceptedTypeImage.color = this.acceptedTypeImageIdleTint;
		this.RefreshAcceptedItemTypeImage();
	}

	public void SetOccupyingObject(GridObject gridObject)
	{
		this.occupyingObject = gridObject;
		if (gridObject == null)
		{
			this.occupationImage.gameObject.SetActive(false);
		}
		else
		{
			if (gridObject.SpatialItemInstance is FishItemInstance)
			{
				if ((gridObject.ItemData as FishItemData).IsAberration)
				{
					this.occupationImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL);
				}
				else if ((gridObject.SpatialItemInstance as FishItemInstance).IsTrophySize())
				{
					Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) * this.trophyBackgroundMultiplier;
					this.occupationImage.color = new Color(color.r, color.g, color.b, 1f);
				}
				else
				{
					this.occupationImage.color = gridObject.ItemData.itemColor;
				}
			}
			else
			{
				this.occupationImage.color = gridObject.ItemData.itemColor;
			}
			this.occupationImage.gameObject.SetActive(true);
		}
		this.RefreshAcceptedItemTypeImage();
	}

	public void SetOccupyingHintObject(GridObject gridObject)
	{
		this.occupyingHintObject = gridObject;
	}

	private void RefreshAcceptedItemTypeImage()
	{
		if (this.isShowingPreviewImage)
		{
			return;
		}
		if (this.gridCellData.IsHidden || !this.ShouldShowItemTypes())
		{
			this.acceptedTypeImage.gameObject.SetActive(false);
			return;
		}
		if (this.occupyingObject == null && this.acceptedItemType != ItemType.ALL && this.gridCellState == GridObjectState.IN_INVENTORY)
		{
			this.acceptedTypeImage.sprite = this.GetSpriteForItemType(this.acceptedItemSubtype);
			this.acceptedTypeImage.gameObject.SetActive(true);
			this.acceptedTypeImage.color = this.acceptedTypeImageIdleTint;
			return;
		}
		this.acceptedTypeImage.gameObject.SetActive(false);
	}

	private bool ShouldShowItemTypes()
	{
		return GameManager.Instance.Player.CanMoveInstalledItems || GameManager.Instance.GridManager.IsPreviewingSlotUpgrade;
	}

	private Sprite GetSpriteForItemType(ItemSubtype subtype)
	{
		if (subtype.HasFlag(ItemSubtype.ENGINE))
		{
			return this.engineSprite;
		}
		if (subtype.HasFlag(ItemSubtype.NET))
		{
			return this.trawlSprite;
		}
		if (subtype.HasFlag(ItemSubtype.ROD))
		{
			return this.tackleSprite;
		}
		if (subtype.HasFlag(ItemSubtype.LIGHT))
		{
			return this.lightSprite;
		}
		return this.regularSprite;
	}

	private void Update()
	{
		if (this._isDirty)
		{
			if (this.acceptedItemType != ItemType.NONE)
			{
				switch (this.cellState)
				{
				case CellState.VALID:
					this.fillImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
					goto IL_009D;
				case CellState.INVALID:
					this.fillImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
					goto IL_009D;
				case CellState.SEMI_VALID:
					this.fillImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.WARNING);
					goto IL_009D;
				}
				this.fillImage.color = this.regularColor;
			}
			IL_009D:
			this._isDirty = false;
		}
	}

	public void OnSelectUpdate()
	{
		EventSystem.current.SetSelectedGameObject(base.gameObject);
		GameManager.Instance.GridManager.OnSelectUpdate(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		GameManager.Instance.GridManager.OnSelectUpdate(null);
	}

	public void OnSelected()
	{
		GridObject currentlyHeldObject = GameManager.Instance.GridManager.CurrentlyHeldObject;
		float num = GameManager.Instance.GridManager.ScaledCellSize * 0.5f;
		Vector3 vector = new Vector3(base.transform.position.x + num, base.transform.position.y - num, 0f);
		if (currentlyHeldObject)
		{
			bool flag = currentlyHeldObject.ItemData.GetWidth() % 2 == 0;
			bool flag2 = currentlyHeldObject.ItemData.GetHeight() % 2 == 0;
			float x = GameManager.Instance.GridManager.CurrentHeldObjectSelectionOffset.x;
			float y = GameManager.Instance.GridManager.CurrentHeldObjectSelectionOffset.y;
			float num2 = (float)Mathf.Abs(currentlyHeldObject.RootCoord.z - currentlyHeldObject.CurrentRotation);
			num2 = (num2 + 360f) % 360f;
			bool flag3 = num2 != 0f && num2 != 180f;
			if (flag && flag2)
			{
				if (x < 0f)
				{
					vector.x -= num;
				}
				else if (x > 0f)
				{
					vector.x += num;
				}
				if (y < 0f)
				{
					vector.y -= num;
				}
				else if (y > 0f)
				{
					vector.y += num;
				}
			}
			else if (flag3)
			{
				vector.x -= y;
				vector.y -= x;
			}
			else
			{
				vector.x += x;
				vector.y += y;
			}
		}
		GameManager.Instance.GridManager.CursorProxy.MoveTo(vector);
		GameManager.Instance.GridManager.OnSelectUpdate(this);
	}

	protected GridUI parentGrid;

	private GridObject occupyingObject;

	private GridObject occupyingUnderlayObject;

	private GridObject occupyingHintObject;

	public GridObjectState gridCellState;

	public Image hitImage;

	public Image backplateImage;

	public Image fillImage;

	public Image occupationImage;

	public Button button;

	public float trophyBackgroundMultiplier;

	private ItemType acceptedItemType = ItemType.GENERAL;

	private ItemSubtype acceptedItemSubtype;

	public Color hiddenColor;

	public Color regularColor;

	public Image acceptedTypeImage;

	public Color acceptedTypeImageIdleTint;

	public Color acceptedTypeImageHighlightTint;

	public Sprite regularSprite;

	public Sprite trawlSprite;

	public Sprite tackleSprite;

	public Sprite engineSprite;

	public Sprite lightSprite;

	private CellState cellState;

	private bool _isDirty = true;

	private GridCellData gridCellData;

	private bool didAddHighlightListeners;

	private bool isShowingPreviewImage;
}
