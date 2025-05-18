using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommandTerminal;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : SerializedMonoBehaviour
{
	private RectTransform Cursor
	{
		get
		{
			if (!(this.currentlyFulfilledTooltipRequester == null))
			{
				return GameManager.Instance.UIFocusObject.Container;
			}
			return this.cursorProxyTransform;
		}
	}

	private void Awake()
	{
		GridManager gridManager = GameManager.Instance.GridManager;
		RectTransform rectTransform;
		if (gridManager == null)
		{
			rectTransform = null;
		}
		else
		{
			CursorProxy cursorProxy = gridManager.CursorProxy;
			rectTransform = ((cursorProxy != null) ? cursorProxy.CursorSquare : null);
		}
		this.cursorProxyTransform = rectTransform;
		this.SetVis(false);
	}

	private void SetVis(bool Vis)
	{
		this.containerRect.gameObject.SetActive(Vis);
		this.isActive = Vis;
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("tooltips");
	}

	private void ToggleTooltips(CommandArg[] args)
	{
		this.debugTooltipsDisabled = args[0].Int == 0;
	}

	private void Start()
	{
		Terminal.Shell.AddCommand("tooltips", new Action<CommandArg[]>(this.ToggleTooltips), 1, 1, "Toggles tooltips [0 = off | 1 = on]");
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnItemHoveredChanged += this.OnItemHoveredChanged;
			GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
			GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
			GameEvents.Instance.OnActivelyHarvestingChanged += this.OnActivelyHarvestingChanged;
		}
		ApplicationEvents.Instance.OnUITooltipRequested += this.OnUITooltipRequested;
		ApplicationEvents.Instance.OnUITooltipClearRequested += this.OnUITooltipClearRequested;
	}

	private void OnDisable()
	{
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnItemHoveredChanged -= this.OnItemHoveredChanged;
			GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
			GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
			GameEvents.Instance.OnActivelyHarvestingChanged -= this.OnActivelyHarvestingChanged;
		}
		ApplicationEvents.Instance.OnUITooltipRequested -= this.OnUITooltipRequested;
		ApplicationEvents.Instance.OnUITooltipClearRequested -= this.OnUITooltipClearRequested;
	}

	private void OnActivelyHarvestingChanged(bool isHarvesting)
	{
		if (isHarvesting)
		{
			this.RefreshTooltipContent(null, TooltipUI.TooltipMode.NONE);
		}
	}

	private void OnGameWindowToggled()
	{
		if (!(GameManager.Instance.GridManager.CurrentlyHeldObject != null))
		{
			this.RefreshTooltipContent(null, TooltipUI.TooltipMode.NONE);
		}
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.RefreshTooltipContent(null, TooltipUI.TooltipMode.NONE);
	}

	private void OnItemHoveredChanged(GridObject gridObject)
	{
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			this.currentlyFulfilledTooltipRequester = null;
			this.RefreshTooltipContent(gridObject, TooltipUI.TooltipMode.HOVER);
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.currentlyFulfilledTooltipRequester = null;
		this.RefreshTooltipContent(gridObject, TooltipUI.TooltipMode.HOLD);
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			this.currentlyFulfilledTooltipRequester = null;
			this.RefreshTooltipContent(gridObject, TooltipUI.TooltipMode.NONE);
		}
	}

	private void OnUITooltipRequested(TooltipRequester tooltipRequester)
	{
		this.tooltipShowArea = null;
		if (this.currentlyFulfilledTooltipRequester == tooltipRequester)
		{
			return;
		}
		this.currentlyFulfilledTooltipRequester = tooltipRequester;
		if (tooltipRequester is NonSpatialItemTooltipRequester)
		{
			this.ConstructNonSpatialItemTooltip((tooltipRequester as NonSpatialItemTooltipRequester).NonSpatialItemInstance, (tooltipRequester as NonSpatialItemTooltipRequester).NonSpatialItemInstance.GetItemData<NonSpatialItemData>(), TooltipUI.TooltipMode.HOVER);
			return;
		}
		if (tooltipRequester is SpatialItemTooltipRequester)
		{
			this.ConstructSpatialItemTooltip(null, (tooltipRequester as SpatialItemTooltipRequester).SpatialItemData, (tooltipRequester as SpatialItemTooltipRequester).TooltipMode, (tooltipRequester as SpatialItemTooltipRequester).RecipeData);
			return;
		}
		if (tooltipRequester is UpgradeTooltipRequester)
		{
			this.ConstructUpgradeTooltip((tooltipRequester as UpgradeTooltipRequester).upgradeData);
			return;
		}
		if (tooltipRequester is TextTooltipRequester)
		{
			this.ConstructTextTooltip(tooltipRequester as TextTooltipRequester, TooltipUI.TooltipMode.HOVER);
			return;
		}
		if (tooltipRequester is AbilityTooltipRequester)
		{
			this.ConstructAbilityTooltip(tooltipRequester as AbilityTooltipRequester, TooltipUI.TooltipMode.HOVER, (tooltipRequester as AbilityTooltipRequester).RecipeData);
		}
	}

	private void OnUITooltipClearRequested(TooltipRequester tooltipRequester, bool force = false)
	{
		if (this.currentlyFulfilledTooltipRequester != tooltipRequester && !force)
		{
			return;
		}
		this.SetVis(false);
		this.currentlyFulfilledTooltipRequester = null;
	}

	private void DoRefresh()
	{
		if (this.debugTooltipsDisabled)
		{
			this.SetVis(false);
			return;
		}
		if (!GameManager.Instance.GridManager.IsShowingGrid)
		{
			this.SetVis(false);
			return;
		}
		if (GameManager.Instance.Player && GameManager.Instance.Player.IsFishing)
		{
			this.SetVis(false);
			return;
		}
		if (this.gridObject == null)
		{
			this.SetVis(false);
			return;
		}
		if (this.tooltipMode == TooltipUI.TooltipMode.NONE)
		{
			this.SetVis(false);
			return;
		}
		if (GameManager.Instance.GridManager.IsInRepairMode && !this.gridObject.ItemData.isUnderlayItem && this.gridObject.ItemData.damageMode != DamageMode.DURABILITY)
		{
			this.SetVis(false);
			return;
		}
		if (this.gridObject.state == GridObjectState.IS_HINT)
		{
			this.tooltipMode = TooltipUI.TooltipMode.HINT;
		}
		else if (this.gridObject.state == GridObjectState.MYSTERY)
		{
			this.tooltipMode = TooltipUI.TooltipMode.MYSTERY;
		}
		if (this.gridObject)
		{
			this.tooltipShowArea = GameManager.Instance.UI.GetTooltipAreaForShowingGrids();
			this.tooltipShowArea.GetWorldCorners(this.worldCorners);
		}
		else
		{
			this.tooltipShowArea = null;
		}
		this.ConstructSpatialItemTooltip(this.gridObject.SpatialItemInstance, this.gridObject.ItemData, this.tooltipMode, null);
	}

	private async Task RefreshTooltipContent(GridObject gridObjectIn, TooltipUI.TooltipMode tooltipModeIn)
	{
		this.tooltipMode = tooltipModeIn;
		this.gridObject = gridObjectIn;
		this.DoRefresh();
		return;
		TaskAwaiter taskAwaiter2;
		TaskAwaiter taskAwaiter = taskAwaiter2;
		taskAwaiter2 = default(TaskAwaiter);
		taskAwaiter.GetResult();
	}

	private async Task RunCooldown()
	{
		await Awaiters.Seconds(this.CoolDown);
		if (this.OnCoolDown)
		{
			this.DoRefresh();
		}
		this.OnCoolDown = false;
	}

	private void ConstructSpatialItemTooltip(SpatialItemInstance itemInstance, ItemData itemData, TooltipUI.TooltipMode tooltipMode, RecipeData recipeData = null)
	{
		this.PrepareForTooltipShow();
		this.activeTooltipSections.Add(this.itemHeaderWithIcon);
		this.itemHeaderWithIcon.Init<ItemData>(itemData, tooltipMode);
		this.itemHeaderWithIcon.SetObscured(tooltipMode == TooltipUI.TooltipMode.MYSTERY);
		if (tooltipMode == TooltipUI.TooltipMode.HINT && itemData.itemSubtype == ItemSubtype.FISH)
		{
			this.activeTooltipSections.Add(this.fishHarvestDetails);
			this.fishHarvestDetails.Init<FishItemData>(itemData as FishItemData, tooltipMode);
		}
		if (tooltipMode == TooltipUI.TooltipMode.HOVER)
		{
			if (itemData.itemSubtype == ItemSubtype.FISH)
			{
				this.activeTooltipSections.Add(this.fishDetails);
				this.fishDetails.Init<FishItemInstance>(itemInstance as FishItemInstance, tooltipMode);
			}
			if (itemData.itemType == ItemType.EQUIPMENT && itemData.itemSubtype != ItemSubtype.POT)
			{
				this.activeTooltipSections.Add(this.equipmentDetails);
				this.equipmentDetails.Init<SpatialItemInstance>(itemInstance, tooltipMode);
			}
		}
		if (tooltipMode == TooltipUI.TooltipMode.HOVER || tooltipMode == TooltipUI.TooltipMode.RESEARCH_PREVIEW || tooltipMode == TooltipUI.TooltipMode.MYSTERY)
		{
			if (itemData.itemSubtype == ItemSubtype.ROD)
			{
				this.activeTooltipSections.Add(this.rodDetails);
				this.rodDetails.Init<RodItemData>(itemData as RodItemData, itemInstance, tooltipMode);
			}
			if (itemData.itemSubtype == ItemSubtype.DREDGE)
			{
				this.activeTooltipSections.Add(this.dredgeDetails);
				this.dredgeDetails.Init<DredgeItemData>(itemData as DredgeItemData, tooltipMode);
			}
			if (itemData.itemSubtype == ItemSubtype.ENGINE && tooltipMode != TooltipUI.TooltipMode.MYSTERY)
			{
				this.activeTooltipSections.Add(this.engineDetails);
				this.engineDetails.Init<EngineItemData>(itemData as EngineItemData, itemInstance, tooltipMode);
			}
			if (itemData.itemSubtype == ItemSubtype.LIGHT)
			{
				this.activeTooltipSections.Add(this.lightDetails);
				this.lightDetails.Init<LightItemData>(itemData as LightItemData, itemInstance, tooltipMode);
			}
			if (itemData.itemSubtype == ItemSubtype.POT && tooltipMode != TooltipUI.TooltipMode.MYSTERY)
			{
				this.activeTooltipSections.Add(this.deployableDetails);
				this.deployableDetails.Init<DeployableItemData>(itemData as DeployableItemData, itemInstance, tooltipMode);
			}
			if (itemData.itemSubtype == ItemSubtype.NET)
			{
				this.activeTooltipSections.Add(this.deployableDetails);
				this.deployableDetails.Init<DeployableItemData>(itemData as DeployableItemData, itemInstance, tooltipMode);
			}
			if (itemData is DurableItemData)
			{
				this.activeTooltipSections.Add(this.durability);
				this.durability.Init<DurableItemData>(itemData as DurableItemData, itemInstance, tooltipMode);
			}
			if (itemData is GadgetItemData)
			{
				this.activeTooltipSections.Add(this.gadgetDetails);
				this.gadgetDetails.Init<GadgetItemData>(itemData as GadgetItemData, itemInstance, tooltipMode);
			}
			if (tooltipMode != TooltipUI.TooltipMode.MYSTERY)
			{
				this.activeTooltipSections.Add(this.description);
				this.description.Init<ItemData>(itemData, tooltipMode);
			}
			if (itemData.hasAdditionalNote)
			{
				this.activeTooltipSections.Add(this.additionalNote);
				this.additionalNote.Init<ItemData>(itemData, tooltipMode);
			}
		}
		if (recipeData != null)
		{
			this.activeTooltipSections.Add(this.upgradeCost);
			this.upgradeCost.Init(recipeData);
		}
		if (tooltipMode != TooltipUI.TooltipMode.HINT)
		{
			this.activeTooltipSections.Add(this.controlPrompts);
			this.controlPrompts.Init();
		}
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
		}
		this.layoutCoroutine = base.StartCoroutine(this.DoUpdateLayoutGroups());
	}

	private void ConstructNonSpatialItemTooltip(NonSpatialItemInstance itemInstance, ItemData itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.PrepareForTooltipShow();
		this.activeTooltipSections.Add(this.itemHeaderWithoutIcon);
		this.itemHeaderWithoutIcon.Init<ItemData>(itemData, tooltipMode);
		if (itemInstance is ResearchableItemInstance)
		{
			this.activeTooltipSections.Add(this.researchableItemDescription);
			this.researchableItemDescription.Init<ResearchableItemInstance>(itemInstance as ResearchableItemInstance, tooltipMode);
		}
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
		}
		this.layoutCoroutine = base.StartCoroutine(this.DoUpdateLayoutGroups());
	}

	private void ConstructUpgradeTooltip(UpgradeData upgradeData)
	{
		this.PrepareForTooltipShow();
		this.activeTooltipSections.Add(this.itemHeaderWithIcon);
		this.itemHeaderWithIcon.Init(upgradeData);
		this.activeTooltipSections.Add(this.description);
		this.description.Init(upgradeData);
		if (!GameManager.Instance.SaveData.GetIsUpgradeOwned(upgradeData))
		{
			this.activeTooltipSections.Add(this.upgradeCost);
			this.upgradeCost.Init(upgradeData);
			this.activeTooltipSections.Add(this.controlPrompts);
			this.controlPrompts.Init();
		}
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
		}
		this.layoutCoroutine = base.StartCoroutine(this.DoUpdateLayoutGroups());
	}

	private void ConstructTextTooltip(TextTooltipRequester tooltipRequester, TooltipUI.TooltipMode tooltipMode)
	{
		this.PrepareForTooltipShow();
		this.activeTooltipSections.Add(this.itemHeaderWithoutIcon);
		this.itemHeaderWithoutIcon.Init(tooltipRequester);
		this.activeTooltipSections.Add(this.description);
		this.description.Init(tooltipRequester);
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
		}
		this.layoutCoroutine = base.StartCoroutine(this.DoUpdateLayoutGroups());
	}

	private void ConstructAbilityTooltip(AbilityTooltipRequester tooltipRequester, TooltipUI.TooltipMode tooltipMode, RecipeData recipeData = null)
	{
		this.PrepareForTooltipShow();
		this.activeTooltipSections.Add(this.itemHeaderWithoutIcon);
		this.itemHeaderWithoutIcon.Init(tooltipRequester);
		this.activeTooltipSections.Add(this.description);
		this.description.Init(tooltipRequester);
		if (recipeData != null && (!(recipeData is AbilityRecipeData) || !GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked((recipeData as AbilityRecipeData).abilityData)))
		{
			this.activeTooltipSections.Add(this.upgradeCost);
			this.upgradeCost.Init(recipeData);
		}
		this.activeTooltipSections.Add(this.controlPrompts);
		this.controlPrompts.Init();
		if (this.layoutCoroutine != null)
		{
			base.StopCoroutine(this.layoutCoroutine);
		}
		this.layoutCoroutine = base.StartCoroutine(this.DoUpdateLayoutGroups());
	}

	private void PrepareForTooltipShow()
	{
		if (this.canvasGroupFadeTween != null)
		{
			this.canvasGroupFadeTween.Kill(false);
			this.canvasGroupFadeTween = null;
		}
		this.canvasGroup.alpha = 0f;
		this.SetVis(true);
		foreach (object obj in this.containerRect)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		this.activeTooltipSections.Clear();
	}

	private IEnumerator DoUpdateLayoutGroups()
	{
		do
		{
			yield return new WaitForEndOfFrame();
		}
		while (!this.activeTooltipSections.TrueForAll((ILayoutable x) => x.IsLayedOut));
		this.activeTooltipSections.ForEach(delegate(ILayoutable x)
		{
			x.GameObject.SetActive(true);
		});
		this.verticalLayoutGroup.enabled = false;
		yield return new WaitForEndOfFrame();
		this.verticalLayoutGroup.enabled = true;
		yield return new WaitForEndOfFrame();
		this.canvasGroupFadeTween = this.canvasGroup.DOFade(1f, 0.35f);
		this.canvasGroupFadeTween.SetUpdate(true);
		this.canvasGroupFadeTween.OnComplete(delegate
		{
			this.canvasGroupFadeTween = null;
		});
		this.layoutCoroutine = null;
		yield break;
	}

	private void LateUpdate()
	{
		this.RunLayoutChange();
	}

	private void RunLayoutChange()
	{
		if (this.isActive)
		{
			this.cachedCursorRef = this.Cursor;
			if (this.cachedCursorRef == null)
			{
				Debug.LogWarning("[TooltipUI] LateUpdate() Attempting to show a tooltip with a null cursor position. Exiting.");
				return;
			}
			this.x = this.cachedCursorRef.position.x;
			this.y = this.cachedCursorRef.position.y;
			this.xMin = this.cachedCursorRef.rect.xMin;
			this.xMax = this.cachedCursorRef.rect.xMax;
			this.yMin = this.cachedCursorRef.rect.yMin;
			this.yMax = this.cachedCursorRef.rect.yMax;
			this.boxYMin = 0f;
			this.boxYMax = (float)Screen.height;
			if (this.tooltipShowArea == null)
			{
				this.boxXMin = 0f;
				this.boxXMax = (float)Screen.width;
			}
			else
			{
				this.boxXMin = this.worldCorners[0].x;
				this.boxXMax = this.worldCorners[2].x;
			}
			this.scaleFactor = GameManager.Instance.ScaleFactor;
			bool flag = this.x < (float)Screen.width * 0.5f;
			float num = (flag ? 0f : 1f);
			this.containerRect.pivot = new Vector2(num, 1f);
			Vector2 vector = new Vector2(this.x + this.xMax * this.scaleFactor, this.y - this.yMin * this.scaleFactor);
			if (flag)
			{
				vector.x = this.x + this.xMax * this.scaleFactor;
			}
			else
			{
				vector.x = this.x + this.xMin * this.scaleFactor;
			}
			if (vector.x + this.containerRect.rect.xMax * this.scaleFactor * this.containerRect.pivot.x > this.boxXMax)
			{
				vector.x = this.boxXMax;
			}
			else if (vector.x + this.containerRect.rect.xMin * this.scaleFactor * this.containerRect.pivot.x < this.boxXMin)
			{
				vector.x = this.boxXMin;
			}
			if (vector.y + this.containerRect.rect.yMax * this.scaleFactor > this.boxYMax)
			{
				vector.y = this.boxYMax;
			}
			else if (vector.y - this.containerRect.rect.height * this.scaleFactor < this.boxYMin)
			{
				vector.y = this.containerRect.rect.height * this.scaleFactor;
			}
			this.containerRect.position = vector;
		}
	}

	[SerializeField]
	private RectTransform containerRect;

	[SerializeField]
	private VerticalLayoutGroup verticalLayoutGroup;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private TooltipSectionHeader itemHeaderWithIcon;

	[SerializeField]
	private TooltipSectionHeader itemHeaderWithoutIcon;

	[SerializeField]
	private TooltipSectionDescription description;

	[SerializeField]
	private TooltipSectionAdditionalNote additionalNote;

	[SerializeField]
	private TooltipSectionDemoNote demoNote;

	[SerializeField]
	private TooltipSectionFishDetails fishDetails;

	[SerializeField]
	private TooltipSectionFishHarvestDetails fishHarvestDetails;

	[SerializeField]
	private TooltipSectionEquipmentDetails equipmentDetails;

	[SerializeField]
	private TooltipSectionRodDetails rodDetails;

	[SerializeField]
	private TooltipSectionDredgeDetails dredgeDetails;

	[SerializeField]
	private TooltipSectionEngineDetails engineDetails;

	[SerializeField]
	private TooltipSectionLightDetails lightDetails;

	[SerializeField]
	private TooltipSectionDeployableDetails deployableDetails;

	[SerializeField]
	private TooltipSectionUpgradeCost upgradeCost;

	[SerializeField]
	private TooltipSectionControlPrompts controlPrompts;

	[SerializeField]
	private TooltipSectionResearchableDescription researchableItemDescription;

	[SerializeField]
	private TooltipSectionDurabilityDetails durability;

	[SerializeField]
	private TooltipSectionGadgetDetails gadgetDetails;

	private Tweener canvasGroupFadeTween;

	private Coroutine layoutCoroutine;

	private List<ILayoutable> activeTooltipSections = new List<ILayoutable>();

	private TooltipRequester currentlyFulfilledTooltipRequester;

	private RectTransform cursorProxyTransform;

	private bool debugTooltipsDisabled;

	private bool isActive;

	private float CoolDown = 0.7f;

	private bool OnCoolDown;

	private GridObject gridObject;

	private TooltipUI.TooltipMode tooltipMode;

	private float scaleFactor;

	private float x;

	private float y;

	private float xMax;

	private float xMin;

	private float yMax;

	private float yMin;

	private float boxXMin;

	private float boxXMax;

	private float boxYMin;

	private float boxYMax;

	private RectTransform cachedCursorRef;

	private RectTransform tooltipShowArea;

	private Vector3[] worldCorners = new Vector3[4];

	public enum TooltipMode
	{
		NONE,
		HOVER,
		HOLD,
		RESEARCH_PREVIEW,
		MYSTERY,
		HINT,
		RECIPE
	}
}
