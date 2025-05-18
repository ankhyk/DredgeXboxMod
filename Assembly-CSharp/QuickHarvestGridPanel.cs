using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuickHarvestGridPanel : MonoBehaviour
{
	private void OnEnable()
	{
		this.slidePanel.OnHideFinish.AddListener(new UnityAction(this.OnHideFinish));
		BasicButtonWrapper basicButtonWrapper = this.takeAllButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnTakeAllButtonClicked));
	}

	private void OnDisable()
	{
		this.slidePanel.OnHideFinish.RemoveListener(new UnityAction(this.OnHideFinish));
		BasicButtonWrapper basicButtonWrapper = this.takeAllButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnTakeAllButtonClicked));
	}

	private void OnTakeAllButtonClicked()
	{
		this.EmptyGridIntoInventory();
		if (this.gridContents.spatialItems.Count > 0)
		{
			return;
		}
		GameManager.Instance.Player.Harvester.RequestExit();
	}

	private void EmptyGridIntoInventory()
	{
		for (int i = this.gridContents.spatialItems.Count - 1; i >= 0; i--)
		{
			SpatialItemInstance spatialItemInstance = this.gridContents.spatialItems[i];
			Vector3Int vector3Int;
			if (GameManager.Instance.SaveData.Inventory.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out vector3Int, 0, false))
			{
				this.gridContents.RemoveObjectFromGridData(spatialItemInstance, true);
				GameManager.Instance.ItemManager.AddObjectToInventory(spatialItemInstance, vector3Int, true);
			}
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			this.RefreshButtonValidity();
		}
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.RefreshButtonValidity();
	}

	private void RefreshButtonValidity()
	{
		this.takeAllButton.Interactable = this.gridUI.linkedGrid.spatialItems.Count > 0;
	}

	public void Show(HarvestPOI harvestPOI, QuickHarvestGridPanel.QuickHarvestGridMode mode)
	{
		this.mode = mode;
		if (mode == QuickHarvestGridPanel.QuickHarvestGridMode.ATROPHY)
		{
			this.titleTextField.StringReference = this.atrophyTitleText;
			this.descriptionTextField.StringReference = this.atrophyDescriptionText;
			this.gridBackgroundImage.sprite = this.atrophyBackgroundImage;
		}
		else if (mode == QuickHarvestGridPanel.QuickHarvestGridMode.CRAB_BAIT)
		{
			this.titleTextField.StringReference = this.crabBaitTitleText;
			this.descriptionTextField.StringReference = this.crabBaitDescriptionText;
			this.gridBackgroundImage.sprite = this.crabPotBackgroundImage;
		}
		this.titleTextField.RefreshString();
		this.descriptionTextField.RefreshString();
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		this.harvestPOI = harvestPOI;
		this.gridContents = new SerializableGrid();
		this.gridContents.Init(GameManager.Instance.GameConfigData.GetGridConfigForKey(GridKey.ATROPHY), false);
		if (mode == QuickHarvestGridPanel.QuickHarvestGridMode.ATROPHY)
		{
			this.PopulateGridForAtrophy();
		}
		else if (mode == QuickHarvestGridPanel.QuickHarvestGridMode.CRAB_BAIT)
		{
			this.PopulateGridForCrabBait();
		}
		this.container.SetActive(true);
		this.gridUI.SetLinkedGrid(this.gridContents);
		this.slidePanel.Toggle(true, false);
		GameEvents.Instance.TriggerFishCaught();
		this.RefreshButtonValidity();
	}

	private void PopulateGridForAtrophy()
	{
		int guaranteedAberrationCount = GameManager.Instance.GameConfigData.AtrophyGuaranteedAberrationCount;
		bool flag = global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.AtrophyTotalParasiteChance;
		(new float[2])[1] = 1f;
		List<string> list = new List<string>();
		List<FishItemInstance> allFish = new List<FishItemInstance>();
		if (this.harvestPOI.IsBaitPOI)
		{
			int num = 0;
			int numFishInBaitBallMax = GameManager.Instance.GameConfigData.NumFishInBaitBallMax;
			while (this.harvestPOI.HarvestPOIData.GetStockCount(false) > 0f)
			{
				if (num >= numFishInBaitBallMax)
				{
					break;
				}
				HarvestableItemData harvestableItemData = this.harvestPOI.HarvestPOIData.GetNextHarvestableItem();
				if (harvestableItemData != null)
				{
					list.Add(harvestableItemData.id);
				}
				this.harvestPOI.HarvestPOIData.AddStock(-1f, true);
			}
		}
		else
		{
			HarvestableItemData harvestableItemData = this.harvestPOI.HarvestPOIData.GetNextHarvestableItem();
			if (harvestableItemData != null)
			{
				string id2 = harvestableItemData.id;
				int num2 = Mathf.FloorToInt(this.harvestPOI.HarvestPOIData.GetStockCount(false));
				for (int i = 0; i < num2; i++)
				{
					list.Add(id2);
				}
			}
		}
		list.ForEach(delegate(string id)
		{
			FishItemInstance fishItemInstance = GameManager.Instance.ItemManager.CreateFishItem(id, (guaranteedAberrationCount > 0) ? FishAberrationGenerationMode.FORCE : FishAberrationGenerationMode.RANDOM_CHANCE, false, FishSizeGenerationMode.NO_BIG_TROPHY, 1f);
			fishItemInstance.freshness = global::UnityEngine.Random.Range(GameManager.Instance.GameConfigData.AtrophyConditionMin, GameManager.Instance.GameConfigData.AtrophyConditionMax);
			allFish.Add(fishItemInstance);
			int guaranteedAberrationCount2 = guaranteedAberrationCount;
			guaranteedAberrationCount = guaranteedAberrationCount2 - 1;
		});
		if (flag)
		{
			allFish.PickRandom<FishItemInstance>().Infect();
		}
		bool didFindSpace = false;
		int placementAttempts = 0;
		int maxPlacementAttempts = 5;
		FishItemData fishItemData = null;
		Vector3Int pos = Vector3Int.zero;
		int x = 0;
		int y = 0;
		int[] orientations = new int[] { 0, 90, 180, 270 };
		allFish.ForEach(delegate(FishItemInstance fish)
		{
			didFindSpace = false;
			placementAttempts = 0;
			pos = Vector3Int.zero;
			fishItemData = fish.GetItemData<FishItemData>();
			while (!didFindSpace && placementAttempts < maxPlacementAttempts)
			{
				x = global::UnityEngine.Random.Range(0, this.gridContents.GridConfiguration.columns);
				y = global::UnityEngine.Random.Range(0, this.gridContents.GridConfiguration.rows);
				didFindSpace = this.gridContents.TestPos(fishItemData, x, y, orientations.PickRandom<int>(), out pos);
				int placementAttempts2 = placementAttempts;
				placementAttempts = placementAttempts2 + 1;
			}
			if (!didFindSpace)
			{
				didFindSpace = this.gridContents.FindPositionForObject(fishItemData, out pos, 0, false);
			}
			if (didFindSpace)
			{
				this.gridContents.AddObjectToGridData(fish, pos, false, null);
			}
		});
		this.harvestPOI.HarvestPOIData.AtrophyStock();
		this.harvestPOI.OnStockUpdated();
	}

	private void PopulateGridForCrabBait()
	{
		List<string> list = new List<string>();
		List<FishItemInstance> allFish = new List<FishItemInstance>();
		if (this.harvestPOI.IsBaitPOI)
		{
			int num = 0;
			int numFishInBaitBallMax = GameManager.Instance.GameConfigData.NumFishInBaitBallMax;
			while (this.harvestPOI.HarvestPOIData.GetStockCount(false) > 0f)
			{
				if (num >= numFishInBaitBallMax)
				{
					break;
				}
				HarvestableItemData harvestableItemData = this.harvestPOI.HarvestPOIData.GetNextHarvestableItem();
				if (harvestableItemData != null)
				{
					list.Add(harvestableItemData.id);
				}
				this.harvestPOI.HarvestPOIData.AddStock(-1f, true);
			}
		}
		else
		{
			HarvestableItemData harvestableItemData = this.harvestPOI.HarvestPOIData.GetNextHarvestableItem();
			if (harvestableItemData != null)
			{
				string id2 = harvestableItemData.id;
				int num2 = Mathf.FloorToInt(this.harvestPOI.HarvestPOIData.GetStockCount(false));
				for (int i = 0; i < num2; i++)
				{
					list.Add(id2);
				}
			}
		}
		list.ForEach(delegate(string id)
		{
			FishItemInstance fishItemInstance = GameManager.Instance.ItemManager.CreateFishItem(id, FishAberrationGenerationMode.RANDOM_CHANCE, false, FishSizeGenerationMode.ANY, 1f);
			allFish.Add(fishItemInstance);
		});
		bool didFindSpace = false;
		int placementAttempts = 0;
		int maxPlacementAttempts = 5;
		FishItemData fishItemData = null;
		Vector3Int pos = Vector3Int.zero;
		int x = 0;
		int y = 0;
		int[] orientations = new int[] { 0, 90, 180, 270 };
		allFish.ForEach(delegate(FishItemInstance fish)
		{
			didFindSpace = false;
			placementAttempts = 0;
			pos = Vector3Int.zero;
			fishItemData = fish.GetItemData<FishItemData>();
			while (!didFindSpace && placementAttempts < maxPlacementAttempts)
			{
				x = global::UnityEngine.Random.Range(0, this.gridContents.GridConfiguration.columns);
				y = global::UnityEngine.Random.Range(0, this.gridContents.GridConfiguration.rows);
				didFindSpace = this.gridContents.TestPos(fishItemData, x, y, orientations.PickRandom<int>(), out pos);
				int placementAttempts2 = placementAttempts;
				placementAttempts = placementAttempts2 + 1;
			}
			if (!didFindSpace)
			{
				didFindSpace = this.gridContents.FindPositionForObject(fishItemData, out pos, 0, false);
			}
			if (didFindSpace)
			{
				this.gridContents.AddObjectToGridData(fish, pos, false, null);
			}
		});
		this.harvestPOI.HarvestPOIData.AtrophyStock();
		this.harvestPOI.OnStockUpdated();
	}

	public void Hide()
	{
		this.slidePanel.Toggle(false, false);
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
	}

	private void OnHideFinish()
	{
		this.container.SetActive(false);
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private SlidePanel slidePanel;

	[SerializeField]
	private GridUI gridUI;

	[SerializeField]
	private BasicButtonWrapper takeAllButton;

	[SerializeField]
	private LocalizeStringEvent titleTextField;

	[SerializeField]
	private LocalizeStringEvent descriptionTextField;

	[SerializeField]
	private LocalizedString atrophyTitleText;

	[SerializeField]
	private LocalizedString atrophyDescriptionText;

	[SerializeField]
	private LocalizedString crabBaitTitleText;

	[SerializeField]
	private LocalizedString crabBaitDescriptionText;

	[SerializeField]
	private Sprite crabPotBackgroundImage;

	[SerializeField]
	private Sprite atrophyBackgroundImage;

	[SerializeField]
	private Image gridBackgroundImage;

	private QuickHarvestGridPanel.QuickHarvestGridMode mode;

	private HarvestPOI harvestPOI;

	private SerializableGrid gridContents;

	public enum QuickHarvestGridMode
	{
		ATROPHY,
		CRAB_BAIT
	}
}
