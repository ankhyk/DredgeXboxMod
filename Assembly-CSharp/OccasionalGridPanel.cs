using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class OccasionalGridPanel : MonoBehaviour
{
	private void OnEnable()
	{
		this.slidePanel.OnShowFinish.AddListener(new UnityAction(this.OnShowFinish));
		this.slidePanel.OnHideStart.AddListener(new UnityAction(this.OnHideStart));
		this.slidePanel.OnHideFinish.AddListener(new UnityAction(this.OnHideFinish));
	}

	private void OnDisable()
	{
		this.slidePanel.OnShowFinish.RemoveListener(new UnityAction(this.OnShowFinish));
		this.slidePanel.OnHideStart.RemoveListener(new UnityAction(this.OnHideStart));
		this.slidePanel.OnHideFinish.RemoveListener(new UnityAction(this.OnHideFinish));
	}

	private void OnTakeAllButtonClicked()
	{
		this.EmptyDeployableIntoInventory();
		if (this.placedPOIData.grid.spatialItems.Count > 0)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.crab-pot-pickup-failed");
			return;
		}
		GameManager.Instance.Player.Harvester.RequestExit();
	}

	private void OnPickupButtonClicked()
	{
		if (this.placedPOIData.grid.spatialItems.Count > 0)
		{
			this.EmptyDeployableIntoInventory();
		}
		if (this.placedPOIData.grid.spatialItems.Count > 0)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.crab-pot-pickup-failed");
			return;
		}
		Vector3Int vector3Int;
		if (GameManager.Instance.SaveData.Inventory.FindPositionForObject(this.deployableItemData, out vector3Int, 0, false))
		{
			this.RemoveDeployable();
			SpatialItemInstance spatialItemInstance = new SpatialItemInstance
			{
				id = this.deployableItemData.id,
				durability = this.placedPOIData.durability
			};
			GameManager.Instance.ItemManager.AddObjectToInventory(spatialItemInstance, vector3Int, true);
			GameManager.Instance.Player.Harvester.RequestExit();
			return;
		}
		GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.crab-pot-pickup-failed");
	}

	private void EmptyDeployableIntoInventory()
	{
		for (int i = this.placedPOIData.grid.spatialItems.Count - 1; i >= 0; i--)
		{
			SpatialItemInstance spatialItemInstance = this.placedPOIData.grid.spatialItems[i];
			Vector3Int vector3Int;
			if (GameManager.Instance.SaveData.Inventory.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out vector3Int, 0, false))
			{
				this.placedPOIData.grid.RemoveObjectFromGridData(spatialItemInstance, true);
				GameManager.Instance.ItemManager.AddObjectToInventory(spatialItemInstance, vector3Int, true);
			}
		}
		this.harvestPOI.OnStockUpdated();
	}

	private void RemoveDeployable()
	{
		GameManager.Instance.SaveData.serializedCrabPotPOIs.Remove(this.placedPOIData);
		Cullable component = this.harvestPOI.GetComponent<Cullable>();
		if (component)
		{
			GameManager.Instance.CullingBrain.RemoveCullable(component);
		}
		global::UnityEngine.Object.Destroy(this.harvestPOI.gameObject);
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		this.harvestPOI.OnStockUpdated();
		if (success)
		{
			this.RefreshButtonValidity();
		}
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.harvestPOI.OnStockUpdated();
		this.RefreshButtonValidity();
	}

	private void RefreshButtonValidity()
	{
		this.takeAllButton.Interactable = this.gridUI.linkedGrid.spatialItems.Count > 0;
	}

	public void Show(HarvestPOI harvestPOI)
	{
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		this.harvestPOI = harvestPOI;
		this.placedPOIData = harvestPOI.Harvestable as SerializedCrabPotPOIData;
		this.deployableItemData = GameManager.Instance.ItemManager.GetItemDataById<DeployableItemData>(this.placedPOIData.deployableItemId);
		this.deployableNameTextField.StringReference = this.deployableItemData.itemNameKey;
		this.deployableNameTextField.RefreshString();
		this.placedPOIData.grid.spatialItems.ForEach(delegate(SpatialItemInstance itemInstance)
		{
			if (!itemInstance.seen && itemInstance.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.FISH)
			{
				SaveData saveData = GameManager.Instance.SaveData;
				int potCrabsCaught = saveData.PotCrabsCaught;
				saveData.PotCrabsCaught = potCrabsCaught + 1;
			}
		});
		this.container.SetActive(true);
		this.gridUI.SetLinkedGrid(this.placedPOIData.grid);
		this.slidePanel.Toggle(true, false);
		GameEvents.Instance.TriggerFishCaught();
		float depth = this.placedPOIData.depth;
		this.depthBar.rectTransform.sizeDelta = new Vector2(this.depthBar.rectTransform.rect.width, this.depthBarFullHeight * depth - this.depthBarIconOffsetY);
		this.depthBarIcon.rectTransform.anchoredPosition = new Vector2(this.depthBarIcon.rectTransform.anchoredPosition.x, this.depthBar.rectTransform.anchoredPosition.y - this.depthBar.rectTransform.rect.height);
		this.depthBarText.text = GameManager.Instance.ItemManager.GetFormattedDepthString(this.placedPOIData.depth);
		this.RefreshButtonValidity();
		this.RefreshDurabilityUI();
		GameManager.Instance.AudioPlayer.PlaySFX(this.openSFX, AudioLayer.SFX_UI, 1f, 1f);
	}

	public void TryRepairCurrentCrabPot()
	{
		if (this.placedPOIData != null)
		{
			this.placedPOIData.SetDurability(this.deployableItemData.MaxDurabilityDays);
			this.RefreshDurabilityUI();
		}
	}

	private void RefreshDurabilityUI()
	{
		GameManager.Instance.LanguageManager.FormatTimeStringForDurability(this.placedPOIData.durability, this.deployableItemData.MaxDurabilityDays, this.durabilityTextField, "label.deployable.durability-value");
		this.durabilityTextField.enabled = true;
		this.durabilityBar.fillAmount = this.placedPOIData.durability / this.deployableItemData.MaxDurabilityDays;
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
		this.placedPOIData = null;
	}

	private void OnHideStart()
	{
		BasicButtonWrapper basicButtonWrapper = this.takeAllButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnTakeAllButtonClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.pickupButton;
		basicButtonWrapper2.OnClick = (Action)Delegate.Remove(basicButtonWrapper2.OnClick, new Action(this.OnPickupButtonClicked));
	}

	private void OnShowFinish()
	{
		BasicButtonWrapper basicButtonWrapper = this.takeAllButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnTakeAllButtonClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.pickupButton;
		basicButtonWrapper2.OnClick = (Action)Delegate.Combine(basicButtonWrapper2.OnClick, new Action(this.OnPickupButtonClicked));
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private SlidePanel slidePanel;

	[SerializeField]
	private GridUI gridUI;

	[SerializeField]
	private LocalizeStringEvent deployableNameTextField;

	[SerializeField]
	private LocalizeStringEvent durabilityTextField;

	[SerializeField]
	private BasicButtonWrapper takeAllButton;

	[SerializeField]
	private BasicButtonWrapper pickupButton;

	[SerializeField]
	private Image durabilityBar;

	[SerializeField]
	private Image depthBar;

	[SerializeField]
	private Image depthBarIcon;

	[SerializeField]
	private float depthBarFullHeight;

	[SerializeField]
	private TextMeshProUGUI depthBarText;

	[SerializeField]
	private float depthBarIconOffsetY;

	[SerializeField]
	private AssetReference openSFX;

	private DeployableItemData deployableItemData;

	private HarvestPOI harvestPOI;

	private SerializedCrabPotPOIData placedPOIData;
}
