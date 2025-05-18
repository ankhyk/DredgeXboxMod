using System;
using UnityEngine;

public class TeleportAnchorPOIHandler : MonoBehaviour
{
	public bool IsHandlerActive { get; set; }

	public TeleportAnchorPOI TeleportAnchorPOI
	{
		get
		{
			return this.teleportAnchorPOI;
		}
		set
		{
			this.teleportAnchorPOI = value;
		}
	}

	private void Awake()
	{
		this.retrieveAction = new DredgePlayerActionPress("prompt.retrieve", GameManager.Instance.Input.Controls.InteractSecondary);
		DredgePlayerActionPress dredgePlayerActionPress = this.retrieveAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressRetrieveComplete));
		this.retrieveAction.showInControlArea = true;
		this.retrieveAction.allowPreholding = false;
		this.teleportAction = new DredgePlayerActionHold("prompt.travel", GameManager.Instance.Input.Controls.Interact, 0.5f);
		DredgePlayerActionHold dredgePlayerActionHold = this.teleportAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnPressTeleportComplete));
		this.teleportAction.showInControlArea = true;
		this.teleportAction.allowPreholding = false;
	}

	private void OnDestroy()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.retrieveAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressRetrieveComplete));
		DredgePlayerActionHold dredgePlayerActionHold = this.teleportAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold.OnPressComplete, new Action(this.OnPressTeleportComplete));
	}

	public void Activate(TeleportAnchorPOI teleportAnchorPOI)
	{
		this.IsHandlerActive = true;
		this.teleportAnchorPOI = teleportAnchorPOI;
		DredgePlayerActionBase[] array;
		if (teleportAnchorPOI.CanBeRetrieved)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.retrieveAction };
			input.AddActionListener(array, ActionLayer.BASE);
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionHold[] { this.teleportAction };
		input2.AddActionListener(array, ActionLayer.BASE);
	}

	public void Deactivate()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.teleportAction, this.retrieveAction }, ActionLayer.BASE);
		this.teleportAnchorPOI = null;
		this.IsHandlerActive = false;
	}

	private void OnPressRetrieveComplete()
	{
		if (GameManager.Instance.SaveData.Inventory.FindSpaceAndAddObjectToGridData(GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>("teleport-anchor"), true, null))
		{
			this.RemoveTeleportAnchor();
			return;
		}
		GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.quick-move-failed");
	}

	private void OnPressTeleportComplete()
	{
		if (this.teleportAnchorPOI && this.teleportAnchorPOI.PairedAnchor)
		{
			GameManager.Instance.Player.PlayerTeleport.Teleport(this.teleportAnchorPOI.PairedAnchor.transform.position, -0.5f, null);
		}
	}

	private void RemoveTeleportAnchor()
	{
		Cullable component = this.teleportAnchorPOI.GetComponent<Cullable>();
		if (component)
		{
			GameManager.Instance.CullingBrain.RemoveCullable(component);
		}
		this.teleportAnchorPOI.PendingDelete = true;
		global::UnityEngine.Object.Destroy(this.teleportAnchorPOI.gameObject);
		GameEvents.Instance.TriggerPlayerInteractedWithPOI();
		GameEvents.Instance.TriggerTeleportAnchorRemoved();
		this.Deactivate();
		GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.TELEPORT_ANCHOR_RETRIEVED, "notification.teleport-anchor-retrieved", this.itemData.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL));
		GameManager.Instance.SaveData.SetHasTeleportAnchor(false);
	}

	[SerializeField]
	private SpatialItemData itemData;

	private TeleportAnchorPOI teleportAnchorPOI;

	private DredgePlayerActionPress retrieveAction;

	private DredgePlayerActionHold teleportAction;
}
