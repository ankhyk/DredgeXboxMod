using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

public class ConstructableDestinationUI : BaseDestinationUI
{
	public ConstructableDestinationData ConstructableDestinationData
	{
		get
		{
			return this.constructableDestinationData;
		}
		set
		{
			this.constructableDestinationData = value;
		}
	}

	protected override void OnEnable()
	{
		TabbedPanelContainer tabbedPanelContainer = this.buildingTabbedPanel;
		tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Combine(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnBuildingTabChanged));
		RecipeListPanel recipeListPanel = this.recipeListPanel;
		recipeListPanel.OnRecipeSelected = (Action<RecipeData>)Delegate.Combine(recipeListPanel.OnRecipeSelected, new Action<RecipeData>(this.OnRecipeSelected));
		ItemProductPanel itemProductPanel = this.itemProductPanel;
		itemProductPanel.OnGridComplete = (Action)Delegate.Combine(itemProductPanel.OnGridComplete, new Action(this.OnItemProductPanelComplete));
		RecipeGridPanel recipeGridPanel = this.recipeGridPanel;
		recipeGridPanel.OnRecipeComplete = (Action<RecipeData>)Delegate.Combine(recipeGridPanel.OnRecipeComplete, new Action<RecipeData>(this.OnRecipeGridPanelExitEvent));
		GameEvents.Instance.OnQuestStepCompleted += this.OnQuestStepCompleted;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		TabbedPanelContainer tabbedPanelContainer = this.buildingTabbedPanel;
		tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Remove(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnBuildingTabChanged));
		RecipeListPanel recipeListPanel = this.recipeListPanel;
		recipeListPanel.OnRecipeSelected = (Action<RecipeData>)Delegate.Remove(recipeListPanel.OnRecipeSelected, new Action<RecipeData>(this.OnRecipeSelected));
		ItemProductPanel itemProductPanel = this.itemProductPanel;
		itemProductPanel.OnGridComplete = (Action)Delegate.Remove(itemProductPanel.OnGridComplete, new Action(this.OnItemProductPanelComplete));
		RecipeGridPanel recipeGridPanel = this.recipeGridPanel;
		recipeGridPanel.OnRecipeComplete = (Action<RecipeData>)Delegate.Remove(recipeGridPanel.OnRecipeComplete, new Action<RecipeData>(this.OnRecipeGridPanelExitEvent));
		GameEvents.Instance.OnQuestStepCompleted -= this.OnQuestStepCompleted;
		base.OnDisable();
	}

	private void OnQuestStepCompleted(QuestStepData questStepData)
	{
		if (this.constructableDestinationData != null)
		{
			this.attentionCallouts.ForEach(delegate(AttentionCallout a)
			{
				a.Evaluate();
			});
		}
	}

	protected override void ShowMainUI()
	{
		this.isTryingToShowJustConstructedDialogue = false;
		this.currentViewState = ConstructableDestinationUI.ConstructionViewState.NONE;
		this.constructableDestination = this.destination as ConstructableDestination;
		this.constructableDestinationData = this.constructableDestination.constructableDestinationData;
		this.showablePanels = new List<int>();
		for (int i = 0; i < this.constructableDestinationData.tiers.Count; i++)
		{
			this.showablePanels.Add(i);
			this.buildingTabbedPanel.TabbedPanels[i].tab.TabLocalizedString.StringReference = this.constructableDestinationData.tiers[i].tierNameKey;
		}
		this.buildingTabbedPanel.RequestShowablePanels(this.showablePanels);
		this.localizedTitleString.StringReference = this.constructableDestination.TitleKey;
		this.currentTier = this.GetCurrentTier(this.buildingTabbedPanel.CurrentIndex);
		base.ShowMainUI();
		this.itemProductPanel.SetQuestGridConfig(this.constructableDestinationData.productQuestGrid);
		this.OnBuildingTabChanged(this.buildingTabbedPanel.CurrentIndex);
	}

	private void OnRecipeGridPanelExitEvent(RecipeData recipeData)
	{
		if (recipeData == null)
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.RECIPE_LIST;
			this.RefreshUI();
			return;
		}
		this.dialogueNodeName = recipeData.onRecipeBuiltDialogueNodeName;
		this.isTryingToShowJustConstructedDialogue = true;
		if (recipeData is BuildingRecipeData)
		{
			GameManager.Instance.ConstructableBuildingManager.SetIsBuildingConstructed(this.currentTier.tierId, true, false);
			this.didJustConstructBuilding = true;
			this.tabIndexOfJustConstructedBuilding = this.buildingTabbedPanel.CurrentIndex;
			this.OnBuildingTabChanged(this.buildingTabbedPanel.CurrentIndex);
			return;
		}
		if (recipeData is ItemRecipeData)
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE_COMPLETE;
			for (int i = 0; i < recipeData.quantityProduced; i++)
			{
				this.itemProductPanel.AddItemToGrid((recipeData as ItemRecipeData).itemProduced);
			}
			GameManager.Instance.AudioPlayer.PlaySFX(this.itemConstructSFX, AudioLayer.SFX_UI, 1f, 1f);
			GameManager.Instance.VibrationManager.Vibrate(this.constructItemVibrationData, VibrationRegion.WholeBody, true);
			this.RefreshUI();
			return;
		}
		if (recipeData is AbilityRecipeData)
		{
			GameManager.Instance.PlayerAbilities.UnlockAbility((recipeData as AbilityRecipeData).abilityData);
			this.OnBuildingTabChanged(this.buildingTabbedPanel.CurrentIndex);
			return;
		}
		if (recipeData is HullRecipeData)
		{
			GameManager.Instance.UpgradeManager.AddUpgrade((recipeData as HullRecipeData).hullUpgradeData, false);
			this.OnBuildingTabChanged(this.buildingTabbedPanel.CurrentIndex);
		}
	}

	protected override void ConfigureActionHandlers()
	{
		base.ConfigureActionHandlers();
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.EQUIPMENT);
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.STORAGE);
	}

	protected override void OnLeavePressComplete()
	{
		if (this.currentViewState == ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE)
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.RECIPE_LIST;
			this.RefreshUI();
			return;
		}
		GameManager.Instance.UI.InventorySlidePanel.Toggle(false, false);
		this.buildingSlidePanel.Toggle(false, false);
		if (GameManager.Instance.DialogueRunner.IsDialogueRunning)
		{
			GameManager.Instance.DialogueRunner.Stop();
		}
		GameManager.Instance.DialogueRunner.onDialogueComplete.RemoveListener(new UnityAction(this.OnLeavePressComplete));
		this.buildingTabbedPanel.ForgetLastPanelIndex();
		base.OnLeavePressComplete();
	}

	private void OnRecipeSelected(RecipeData recipeData)
	{
		this.currentViewState = ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE;
		this.currentRecipeData = recipeData;
		this.RefreshUI();
	}

	private void OnItemProductPanelComplete()
	{
		this.OnBuildingTabChanged(this.buildingTabbedPanel.CurrentIndex);
	}

	private BaseDestinationTier GetCurrentTier(int tabIndex)
	{
		this.actualTierBeingShown = this.showablePanels[tabIndex];
		return this.constructableDestinationData.tiers[this.actualTierBeingShown];
	}

	private void OnBuildingTabChanged(int tabIndex)
	{
		this.currentTier = this.GetCurrentTier(tabIndex);
		RecipeData recipeToCreateThis = this.currentTier.recipeToCreateThis;
		if (GameManager.Instance.SaveData.GetIsBuildingConstructed(this.currentTier.tierId))
		{
			if (this.didJustConstructBuilding)
			{
				this.currentViewState = ConstructableDestinationUI.ConstructionViewState.BUILDING_CONSTRUCTION_COMPLETE;
				this.didJustConstructBuilding = false;
			}
			else
			{
				this.currentViewState = this.currentTier.viewAfterConstruction;
			}
		}
		else if (GameManager.Instance.ConstructableBuildingManager.GetCanBuildingBeConstructed(this.currentTier.tierId))
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.CONSTRUCTION_RECIPE;
			this.currentRecipeData = this.constructableDestinationData.tiers[this.actualTierBeingShown].recipeToCreateThis;
		}
		else
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.CANNOT_BE_CONSTRUCTED;
		}
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.recipeGridPanel.gameObject.SetActive(false);
		this.recipeListPanel.gameObject.SetActive(false);
		this.itemProductPanel.gameObject.SetActive(false);
		this.buildingTabbedPanel.RequestShowablePanels(this.showablePanels);
		int num = 0;
		while (num < this.buildingTabbedPanel.TabbedPanels.Count && num < this.constructableDestinationData.tiers.Count)
		{
			(this.buildingTabbedPanel.TabbedPanels[num].tab.TabShowQuery as BuildingTabShowQuery).BuildingTierId = this.constructableDestinationData.tiers[num].tierId;
			num++;
		}
		bool flag = !this.buildingSlidePanel.IsShowing;
		bool flag2 = true;
		if (this.itemProductPanel.HasItemsToPickUp())
		{
			this.currentViewState = ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE_COMPLETE;
		}
		IScreenSideSwitchResponder screenSideSwitchResponder = null;
		switch (this.currentViewState)
		{
		case ConstructableDestinationUI.ConstructionViewState.CONSTRUCTION_RECIPE:
		case ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE:
			this.dialogueNodeName = this.currentRecipeData.onRecipeShownDialogueNodeName;
			this.recipeGridPanel.gameObject.SetActive(true);
			this.recipeGridPanel.Show(this.currentRecipeData, true);
			screenSideSwitchResponder = this.recipeGridPanel;
			GameManager.Instance.AudioPlayer.PlaySFX(this.showBlueprintSFX, AudioLayer.SFX_UI, 0.65f, 1f);
			break;
		case ConstructableDestinationUI.ConstructionViewState.RECIPE_LIST:
			if (!this.isTryingToShowJustConstructedDialogue)
			{
				this.dialogueNodeName = this.currentTier.descriptionDialogueNodeName;
			}
			this.recipeListPanel.gameObject.SetActive(true);
			this.recipeListPanel.Init(this.currentTier as RecipeListDestinationTier, true);
			this.recipeListPanel.ShowStart();
			screenSideSwitchResponder = this.recipeListPanel;
			break;
		case ConstructableDestinationUI.ConstructionViewState.ITEM_RECIPE_COMPLETE:
			if (!this.isTryingToShowJustConstructedDialogue)
			{
				this.dialogueNodeName = this.constructableDestinationData.itemProductPickupReminderDialogueNodeName;
			}
			this.itemProductPanel.gameObject.SetActive(true);
			this.itemProductPanel.Show();
			screenSideSwitchResponder = this.itemProductPanel;
			break;
		case ConstructableDestinationUI.ConstructionViewState.BUILDING_CONSTRUCTION_COMPLETE:
			flag2 = false;
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
			if (this.currentTier.viewAfterConstruction == ConstructableDestinationUI.ConstructionViewState.NONE)
			{
				GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnLeavePressComplete));
			}
			else
			{
				GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnPostConstructionDialogueComplete));
			}
			break;
		case ConstructableDestinationUI.ConstructionViewState.DIALOGUE:
			this.hasInput = false;
			flag2 = false;
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
			this.dialogueNodeName = this.currentTier.postConstructionDialogueNodeName;
			GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnLeavePressComplete));
			break;
		}
		this.hasInput = this.currentViewState != ConstructableDestinationUI.ConstructionViewState.DIALOGUE;
		GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(this.destination.PlayerInventoryTabIndexesToShow);
		if (GameManager.Instance.UI.InventorySlidePanel.WillShow != flag2)
		{
			GameManager.Instance.UI.InventorySlidePanel.Toggle(flag2, false);
		}
		if (this.buildingSlidePanel.WillShow != flag2)
		{
			this.buildingSlidePanel.Toggle(flag2, false);
		}
		if (!string.IsNullOrEmpty(this.dialogueNodeName))
		{
			GameManager.Instance.UI.ClearDialogueViewListener();
			GameManager.Instance.DialogueRunner.Stop();
			GameManager.Instance.DialogueRunner.ShouldAutoResolveNextLine = false;
			base.StartCoroutine(this.DoStartDialogue());
		}
		if (GameManager.Instance.Input.IsUsingController && screenSideSwitchResponder != null && !flag)
		{
			screenSideSwitchResponder.SwitchToSide();
		}
		if (this.constructableDestinationData != null)
		{
			this.attentionCallouts.ForEach(delegate(AttentionCallout a)
			{
				a.Evaluate();
			});
		}
	}

	private IEnumerator DoStartDialogue()
	{
		yield return new WaitForEndOfFrame();
		if (!GameManager.Instance.DialogueRunner.IsDialogueRunning && this.dialogueNodeName != GameManager.Instance.DialogueRunner.PreviousNodeName)
		{
			GameManager.Instance.DialogueRunner.StartDialogue(this.dialogueNodeName);
			this.dialogueNodeName = "";
			if (this.isTryingToShowJustConstructedDialogue)
			{
				this.isTryingToShowJustConstructedDialogue = false;
			}
		}
		yield break;
	}

	private void OnPostConstructionDialogueComplete()
	{
		GameManager.Instance.DialogueRunner.onDialogueComplete.RemoveListener(new UnityAction(this.OnPostConstructionDialogueComplete));
		this.currentViewState = this.currentTier.viewAfterConstruction;
		if (GameManager.Instance.UI.ExitDestinationRequested)
		{
			GameManager.Instance.UI.ExitDestinationRequested = false;
			this.OnLeavePressComplete();
			return;
		}
		this.RefreshUI();
	}

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	private SlidePanel buildingSlidePanel;

	[SerializeField]
	private TabbedPanelContainer buildingTabbedPanel;

	[SerializeField]
	private RecipeGridPanel recipeGridPanel;

	[SerializeField]
	private ItemProductPanel itemProductPanel;

	[SerializeField]
	private RecipeListPanel recipeListPanel;

	[SerializeField]
	private LocalizeStringEvent localizedTitleString;

	[SerializeField]
	private List<AttentionCallout> attentionCallouts = new List<AttentionCallout>();

	[SerializeField]
	private VibrationData constructItemVibrationData;

	[SerializeField]
	private AssetReference itemConstructSFX;

	[SerializeField]
	private AssetReference showBlueprintSFX;

	private List<int> showablePanels;

	private ConstructableDestinationUI.ConstructionViewState currentViewState;

	private int actualTierBeingShown;

	private RecipeData currentRecipeData;

	private ConstructableDestination constructableDestination;

	private ConstructableDestinationData constructableDestinationData;

	private string dialogueNodeName;

	private bool isTryingToShowJustConstructedDialogue;

	private BaseDestinationTier currentTier;

	private bool didJustConstructBuilding;

	private int tabIndexOfJustConstructedBuilding;

	public enum ConstructionViewState
	{
		NONE,
		CONSTRUCTION_RECIPE,
		RECIPE_LIST,
		ITEM_RECIPE,
		ITEM_RECIPE_COMPLETE,
		CANNOT_BE_CONSTRUCTED,
		BUILDING_CONSTRUCTION_COMPLETE,
		DIALOGUE,
		CUTSCENE
	}
}
