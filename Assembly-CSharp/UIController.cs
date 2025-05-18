using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[DefaultExecutionOrder(-900)]
public class UIController : MonoBehaviour
{
	public GridUI InventoryGrid
	{
		get
		{
			return this.inventoryGrid;
		}
	}

	public GridUI StorageGrid
	{
		get
		{
			return this.storageGrid;
		}
	}

	public BaseDestination CurrentDestination
	{
		get
		{
			return this.currentDestination;
		}
	}

	public BaseDestinationUI CurrentDestinationUI
	{
		get
		{
			return this.currentDestinationUI;
		}
	}

	public TabbedPanelContainer PlayerTabbedPanel
	{
		get
		{
			return this.playerTabbedPanel;
		}
	}

	public InventoryPanelTab InventoryPanelTab
	{
		get
		{
			return this.inventoryPanelTab;
		}
	}

	public List<UIWindowType> ShowingWindowTypes
	{
		get
		{
			return this.showingWindowTypes;
		}
	}

	public DockUI DockUI
	{
		get
		{
			return this.dockUI;
		}
	}

	public bool IsHarvesting { get; set; }

	public bool IsShowingQuestGrid { get; private set; }

	public void SetIsShowingQuestGrid(bool val)
	{
		this.IsShowingQuestGrid = val;
		GameEvents.Instance.TriggerQuestGridViewChange();
	}

	public bool IsShowingRadialMenu { get; set; }

	public bool IsInCutscene { get; set; }

	public HarvestMinigameView HarvestMinigameView
	{
		get
		{
			return this.harvestMinigameView;
		}
	}

	public SlidePanel InventorySlidePanel
	{
		get
		{
			return this.inventorySlidePanel;
		}
	}

	public PopupWindow MapWindow
	{
		get
		{
			return this.mapWindow;
		}
	}

	public PopupWindow JournalWindow
	{
		get
		{
			return this.journalWindow;
		}
	}

	public PopupWindow MessagesWindow
	{
		get
		{
			return this.messagesWindow;
		}
	}

	public PopupWindow EncyclopediaWindow
	{
		get
		{
			return this.encyclopediaWindow;
		}
	}

	public PopupWindow UpgradeWindow
	{
		get
		{
			return this.upgradeWindow;
		}
	}

	public AbilityBarUI AbilityBarUI
	{
		get
		{
			return this.abilityBarUI;
		}
	}

	public SpyglassUI SpyglassUI
	{
		get
		{
			return this.spyglassUI;
		}
	}

	public OccasionalGridPanel OccasionalGridPanel
	{
		get
		{
			return this.occasionalGridPanel;
		}
	}

	public QuickHarvestGridPanel QuickHarvestGridPanel
	{
		get
		{
			return this.quickHarvestGridPanel;
		}
	}

	public ResearchWindow ResearchWindow
	{
		get
		{
			return this.researchWindow;
		}
	}

	public TextBannerWithSubtitleUI RepairModeBanner
	{
		get
		{
			return this.repairModeBanner;
		}
	}

	public bool ShowingWindowTypesChangedThisFrame
	{
		get
		{
			return this.showingWindowTypesChangedThisFrame;
		}
	}

	public bool ExitDestinationRequested
	{
		get
		{
			return this.exitDestinationRequested;
		}
		set
		{
			this.exitDestinationRequested = value;
		}
	}

	public QuestGridPanel QuestGridPanel
	{
		get
		{
			return this.questGridPanel;
		}
	}

	public UpgradeGridPanel UpgradeGridPanel
	{
		get
		{
			return this.upgradeGridPanel;
		}
	}

	public InteractPointUI InteractPointUI { get; set; }

	public void CheckInventoryPanelTabShouldShow()
	{
		if (!this.inventoryPanelTab || !this.inventoryPanelTab.gameObject)
		{
			return;
		}
		bool flag = this.ShowingWindowTypes.Count == 1 && this.ShowingWindowTypes[0] == UIWindowType.DOCK;
		bool flag2 = this.ShowingWindowTypes.Contains(UIWindowType.DIALOGUE);
		bool flag3 = !this.IsHarvesting && GameManager.Instance.Player && !GameManager.Instance.Player.IsDocked;
		bool flag4 = (flag || flag3 || this.inventorySlidePanel.WillShow) && !this.isAtDestination && !flag2 && !this.IsInCutscene;
		this.inventoryPanelTab.gameObject.SetActive(flag4);
	}

	public RectTransform GetTooltipAreaForShowingGrids()
	{
		if (GameManager.Instance.GridManager.GetNumActiveGrids() <= 1)
		{
			return this.tooltipAreaSingleGrid;
		}
		return this.tooltipAreaDoubleGrid;
	}

	private void Awake()
	{
		this.cachedSelectedObject = null;
		this.cachedWasGameCanvasActive = true;
		GameManager.Instance.UI = this;
		this.AddTerminalCommands();
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
		GameEvents.Instance.OnPauseChange += this.OnPauseChanged;
		GameEvents.Instance.OnHasUnseenItemsChanged += this.OnHasUnseenItemsChanged;
		GameEvents.Instance.OnPopupWindowToggled += this.OnPopupWindowToggled;
		this.topUICanvasGroup.alpha = 0f;
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnUIWindowToggled += this.OnUIWindowToggled;
		ApplicationEvents.Instance.OnDemoEndToggled += this.OnDemoEndToggled;
		GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
		GameEvents.Instance.OnCutsceneToggled += this.OnCutsceneToggled;
		GameEvents.Instance.OnMaintenanceModeToggled += this.OnMaintenanceModeToggled;
		GameEvents.Instance.OnTopUIToggleRequested += this.OnTopUIToggleRequested;
		GameEvents.Instance.OnRadialMenuShowingToggled += this.OnRadialMenuShowingToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
		ApplicationEvents.Instance.OnUIWindowToggled -= this.OnUIWindowToggled;
		ApplicationEvents.Instance.OnDemoEndToggled -= this.OnDemoEndToggled;
		GameEvents.Instance.OnCutsceneToggled -= this.OnCutsceneToggled;
		GameEvents.Instance.OnMaintenanceModeToggled -= this.OnMaintenanceModeToggled;
		GameEvents.Instance.OnTopUIToggleRequested -= this.OnTopUIToggleRequested;
		GameEvents.Instance.OnPopupWindowToggled -= this.OnPopupWindowToggled;
		GameEvents.Instance.OnRadialMenuShowingToggled -= this.OnRadialMenuShowingToggled;
	}

	private void OnDemoEndToggled(bool showing)
	{
		if (showing)
		{
			this.demoEndWindow.Show();
			return;
		}
		this.demoEndWindow.Hide(PopupWindow.WindowHideMode.CLOSE);
	}

	private void OnRadialMenuShowingToggled(bool showing)
	{
		this.IsShowingRadialMenu = showing;
	}

	public void OnTopUIToggleRequested(bool show)
	{
		DOTween.Kill(this.topUICanvasGroup, false);
		this.topUICanvasGroup.DOFade((float)(show ? 1 : 0), 0.35f);
	}

	public void ToggleInventorySolo(bool showing)
	{
		if (showing)
		{
			GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(new List<int> { 1 });
			this.inventorySlidePanel.Toggle(true, false);
			return;
		}
		this.inventorySlidePanel.Toggle(false, false);
	}

	public void ToggleInventoryAll(bool showing, bool canInterrupt = false, List<int> playerTabsToShow = null)
	{
		if (showing)
		{
			GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(playerTabsToShow);
			this.inventorySlidePanel.Toggle(true, canInterrupt);
			return;
		}
		this.inventorySlidePanel.Toggle(false, canInterrupt);
	}

	public void ToggleInventoryAndStorage(bool showing)
	{
		if (showing)
		{
			GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(new List<int> { 1, 3 });
			this.inventorySlidePanel.Toggle(true, false);
			return;
		}
		this.inventorySlidePanel.Toggle(false, false);
	}

	public void ToggleInventoryAdHoc(bool showing, List<int> showablePanels = null)
	{
		if (showing)
		{
			GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(showablePanels);
			this.inventorySlidePanel.Toggle(true, false);
			return;
		}
		this.inventorySlidePanel.Toggle(false, false);
	}

	public void ToggleInventory(bool show)
	{
		GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(new List<int> { 0, 1, 2 });
		this.inventorySlidePanel.Toggle(show, false);
		if (GameManager.Instance.Player.CurrentDock != null)
		{
			if (show)
			{
				this.dockUI.HideUI();
				return;
			}
			this.dockUI.Show(GameManager.Instance.Player.CurrentDock, 0.2f);
		}
	}

	private void OnMaintenanceModeToggled(bool enabled)
	{
		if (enabled)
		{
			this.repairModeBanner.AnimateIn();
			return;
		}
		this.repairModeBanner.AnimateOut();
	}

	private void OnCutsceneToggled(bool showing)
	{
		this.IsInCutscene = showing;
		this.CheckInventoryPanelTabShouldShow();
	}

	private void OnGameWindowToggled()
	{
		int count = this.ShowingWindowTypes.Count;
		if (count == 0)
		{
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
			return;
		}
		if (count == 1 && this.ShowingWindowTypes.Contains(UIWindowType.DOCK))
		{
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DOCKED);
		}
	}

	private void OnUIWindowToggled(UIWindowType window, bool showing)
	{
		bool flag = false;
		if (window == UIWindowType.NONE)
		{
			return;
		}
		if (showing)
		{
			if (!this.showingWindowTypes.Contains(window))
			{
				this.showingWindowTypes.Add(window);
				flag = true;
			}
		}
		else if (this.showingWindowTypes.Contains(window))
		{
			this.showingWindowTypes.Remove(window);
			flag = true;
		}
		if (flag)
		{
			this.showingWindowTypesChangedThisFrame = true;
			if (window != UIWindowType.SETTINGS)
			{
				GameEvents.Instance.TriggerGameWindowToggled();
			}
		}
		this.CheckInventoryPanelTabShouldShow();
	}

	private void Start()
	{
		this.OnHasUnseenItemsChanged();
	}

	private void LateUpdate()
	{
		if (this.showingWindowTypesChangedThisFrame)
		{
			this.showingWindowTypesChangedThisFrame = false;
		}
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
		GameEvents.Instance.OnPauseChange -= this.OnPauseChanged;
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.OnHasUnseenItemsChanged;
		this.RemoveTerminalCommands();
	}

	private void OnHasUnseenItemsChanged()
	{
		bool flag = GameManager.Instance.SaveData.GetMessages().Any((NonSpatialItemInstance x) => x.isNew);
		bool flag2 = GameManager.Instance.SaveData.questEntries.Values.Any((SerializedQuestEntry x) => x.hasUnseenUpdate);
		bool flag3 = GameManager.Instance.SaveData.ownedNonSpatialItems.Any((NonSpatialItemInstance x) => x.isNew);
		bool flag4 = GameManager.Instance.SaveData.mapMarkers.Any((SerializedMapMarker m) => !m.seen);
		bool flag5 = GameManager.Instance.SaveData.LastUnseenCaughtSpecies != "";
		this.messagesUnseenIcon.gameObject.SetActive(flag);
		this.questsUnseenIcon.gameObject.SetActive(flag2);
		this.mapUnseenIcon.gameObject.SetActive(flag4);
		this.encyclopediaUnseenIcon.gameObject.SetActive(flag5);
		this.cabinPanelUnseenIcon.gameObject.SetActive(flag || flag2 || flag3 || flag4 || flag5);
		this.playerTabUnseenIcon.SetActive(flag || flag2 || flag3 || flag4 || flag5);
	}

	private void OnPauseChanged(bool isPaused)
	{
		if (isPaused)
		{
			this.DisableGameCanvas();
			return;
		}
		this.EnableGameCanvas(false);
	}

	private void OnTimeForcefullyPassingChanged(bool isPassing, string reasonKey, TimePassageMode mode)
	{
		if (isPassing)
		{
			this.DisableGameCanvas();
			return;
		}
		this.EnableGameCanvas(true);
	}

	public void OnPopupWindowToggled(bool showing)
	{
		this.gameCanvasGroup.gameObject.SetActive(!showing);
		this.showingWindowTypesChangedThisFrame = true;
	}

	public void DisableGameCanvas()
	{
		this.cachedSelectedObject = EventSystem.current.currentSelectedGameObject;
		this.cachedWasGameCanvasActive = this.gameCanvasGroup.interactable;
		this.gameCanvasGroup.interactable = false;
		this.gameCanvasGroup.blocksRaycasts = false;
	}

	private void EnableGameCanvas(bool forceEnable)
	{
		this.gameCanvasGroup.interactable = this.cachedWasGameCanvasActive || forceEnable;
		this.gameCanvasGroup.blocksRaycasts = this.cachedWasGameCanvasActive || forceEnable;
		EventSystem.current.SetSelectedGameObject(this.cachedSelectedObject);
	}

	public void ToggleGameCanvasShow(bool showing)
	{
		this.gameCanvasGroup.alpha = (showing ? 1f : 0f);
	}

	public void TogglePhotoModeCanvasShow(bool showing)
	{
		this.photoModeCanvasGroup.alpha = (showing ? 1f : 0f);
	}

	public void PrepareItemNameForSellNotification(NotificationType notificationType, LocalizedString itemNameKey, decimal incomeAmount, decimal debtRepaymentAmount, bool isRefund)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(itemNameKey.TableReference, itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				if (debtRepaymentAmount > 0m)
				{
					GameManager.Instance.UI.ShowNotification(notificationType, "notification.sell-fish-debt", new object[]
					{
						op.Result,
						string.Concat(new string[]
						{
							"<color=#",
							GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
							">+$",
							incomeAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
							"</color>"
						}),
						string.Concat(new string[]
						{
							"<color=#",
							GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
							">-$",
							debtRepaymentAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
							"</color>"
						})
					});
					return;
				}
				if (isRefund)
				{
					GameManager.Instance.UI.ShowNotification(notificationType, "notification.refund-item", new object[]
					{
						op.Result,
						string.Concat(new string[]
						{
							"<color=#",
							GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
							">+$",
							incomeAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
							"</color>"
						})
					});
					return;
				}
				GameManager.Instance.UI.ShowNotification(notificationType, "notification.sell-item", new object[]
				{
					op.Result,
					string.Concat(new string[]
					{
						"<color=#",
						GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
						">+$",
						incomeAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
						"</color>"
					})
				});
			}
		};
	}

	public void ShowNotificationWithItemName(NotificationType notificationType, string key, LocalizedString itemNameKey, Color itemNameColor)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(itemNameKey.TableReference, itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			this.ShowNotification(notificationType, key, new object[] { string.Concat(new string[]
			{
				"<color=#",
				ColorUtility.ToHtmlStringRGB(itemNameColor),
				">",
				op.Result,
				"</color>"
			}) });
		};
	}

	public void ShowNotificationWithSubstitution(NotificationType notificationType, string key, string substitutionKey)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, substitutionKey, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			this.ShowNotification(notificationType, key, new object[] { op.Result });
		};
	}

	public void ShowNotification(NotificationType notificationType, string key)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, key, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				GameEvents.Instance.TriggerNotification(notificationType, op.Result);
			}
		};
	}

	public void ShowNotificationWithColor(NotificationType notificationType, string key, string colorCode)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, key, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				GameEvents.Instance.TriggerNotification(notificationType, string.Concat(new string[] { "<color=#", colorCode, ">", op.Result, "</color>" }));
			}
		};
	}

	public void ShowNotification(NotificationType notificationType, string key, object[] arguments)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, key, null, FallbackBehavior.UseProjectSettings, arguments).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				GameEvents.Instance.TriggerNotification(notificationType, op.Result);
			}
		};
	}

	public void ShowDestination(BaseDestination destination)
	{
		BaseDestinationUI baseDestinationUI = null;
		if (destination is ShipyardDestination)
		{
			baseDestinationUI = this.shipyardDestinationUI;
		}
		else if (destination is MarketDestination)
		{
			baseDestinationUI = this.marketDestinationUI;
		}
		else if (destination is StorageDestination)
		{
			baseDestinationUI = this.storageDestinationUI;
		}
		else if (destination is CharacterDestination)
		{
			baseDestinationUI = this.characterDestinationUI;
		}
		else if (destination is RestDestination)
		{
			baseDestinationUI = this.restDestinationUI;
		}
		else if (destination is ResearchDestination)
		{
			baseDestinationUI = this.researchDestinationUI;
		}
		else if (destination is UpgradeDestination)
		{
			baseDestinationUI = this.upgradeDestinationUI;
		}
		else if (destination is ConstructableDestination)
		{
			baseDestinationUI = this.constructableDestinationUI;
		}
		else if (destination is UndockDestination)
		{
			this.dockUI.Leave();
		}
		else if (destination is OverflowStorageDestination)
		{
			baseDestinationUI = this.overflowStorageUI;
		}
		if (baseDestinationUI != null)
		{
			this.isAtDestination = true;
			this.currentDestination = destination;
			this.currentDestinationUI = baseDestinationUI;
			this.dockUI.HideUI();
			baseDestinationUI.Show(destination);
		}
		this.CheckInventoryPanelTabShouldShow();
	}

	public void HideDialogueView()
	{
		if (this.ShowingWindowTypes.Contains(UIWindowType.DIALOGUE) && this.dialogueView != null)
		{
			this.dialogueView.Hide();
			ApplicationEvents.Instance.TriggerUIWindowToggled(UIWindowType.DIALOGUE, false);
		}
	}

	public void ToggleGameUI(bool enabled)
	{
		this.hudElements.ForEach(delegate(GameObject c)
		{
			c.SetActive(enabled);
		});
	}

	public void ShowDialoguePortrait(string id)
	{
		if (this.dialogueView != null)
		{
			this.dialogueView.ShowPortrait(id);
		}
	}

	public void HideDialoguePortrait()
	{
		if (this.dialogueView != null)
		{
			this.dialogueView.HidePortrait(true);
		}
	}

	public void ClearDialogue()
	{
		if (this.dialogueView != null)
		{
			this.dialogueView.ClearDialogue();
		}
	}

	public void ClearDialogueViewListener()
	{
		if (this.dialogueView != null)
		{
			this.dialogueView.ClearDialogueViewListener();
		}
	}

	public void HideCurrentDestination()
	{
		if (this.currentDestinationUI != null)
		{
			this.currentDestinationUI.Hide();
			base.StartCoroutine(this.RemoveWindow(this.currentDestinationUI.WindowType));
		}
		this.currentDestination = null;
		this.currentDestinationUI = null;
		this.isAtDestination = false;
		this.HideDialogueView();
		this.dockUI.Show(GameManager.Instance.Player.CurrentDock, 0.2f);
		this.CheckInventoryPanelTabShouldShow();
	}

	private IEnumerator RemoveWindow(UIWindowType windowType)
	{
		yield return new WaitForEndOfFrame();
		ApplicationEvents.Instance.TriggerUIWindowToggled(windowType, false);
		yield break;
	}

	public void ShowGameOverDialog()
	{
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "dialog.game-over";
		dialogOptions.disableGameCanvas = true;
		dialogOptions.showScrim = true;
		dialogOptions.useDeathScreenPopup = true;
		DialogButtonOptions dialogButtonOptions = default(DialogButtonOptions);
		dialogButtonOptions.buttonString = "button.return-to-menu";
		dialogButtonOptions.id = 1;
		dialogButtonOptions.hideOnButtonPress = false;
		DialogButtonOptions dialogButtonOptions2 = new DialogButtonOptions
		{
			buttonString = "button.load-last-save",
			id = 0,
			hideOnButtonPress = false
		};
		dialogOptions.buttonOptions = new DialogButtonOptions[] { dialogButtonOptions, dialogButtonOptions2 };
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnConfirmationResult));
	}

	public void ShowDemoOverDialog()
	{
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "dialog.demo-over";
		dialogOptions.disableGameCanvas = true;
		dialogOptions.showScrim = true;
		dialogOptions.buttonOptions = new DialogButtonOptions[]
		{
			new DialogButtonOptions
			{
				buttonString = "button.return-to-menu",
				id = 1,
				hideOnButtonPress = false
			}
		};
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnConfirmationResult));
	}

	private void OnConfirmationResult(DialogButtonOptions result)
	{
		if (result.id == 1)
		{
			GameManager.Instance.Loader.LoadTitleFromGame();
			return;
		}
		if (result.id == 0)
		{
			GameManager.Instance.Loader.ReloadGame();
		}
	}

	public void ToggleHUDCoverScrim(bool enabled, float durationSec)
	{
		if (enabled)
		{
			this.HUDCoverScrim.enabled = true;
		}
		this.HUDCoverScrim.DOFade(enabled ? 1f : 0f, durationSec).From(enabled ? 0f : 1f, true, false).OnComplete(delegate
		{
			if (!enabled)
			{
				this.HUDCoverScrim.enabled = false;
			}
		});
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("ui", new Action<CommandArg[]>(this.ToggleUI), 1, 1, "'ui 0' turns the ui off, 'ui 1' turns the ui back on");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("ui");
		}
	}

	private void ToggleUI(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		ApplicationEvents.Instance.TriggerUIDebugToggled(flag);
	}

	public static float TRANSITION_IN_DURATION_SEC = 0.5f;

	public static float TRANSITION_OUT_DURATION_SEC = 0.35f;

	[SerializeField]
	private CanvasGroup parentCanvasGroup;

	[SerializeField]
	private CanvasGroup gameCanvasGroup;

	[SerializeField]
	private CanvasGroup photoModeCanvasGroup;

	[SerializeField]
	private GameObject playerTabUnseenIcon;

	[SerializeField]
	private GameObject cabinPanelUnseenIcon;

	[SerializeField]
	private GameObject messagesUnseenIcon;

	[SerializeField]
	private GameObject questsUnseenIcon;

	[SerializeField]
	private GameObject mapUnseenIcon;

	[SerializeField]
	private GameObject encyclopediaUnseenIcon;

	[SerializeField]
	private List<GameObject> hudElements;

	[Header("Grids")]
	[SerializeField]
	private GridUI inventoryGrid;

	[SerializeField]
	private GridUI storageGrid;

	[Header("Panels")]
	[SerializeField]
	private SlidePanel inventorySlidePanel;

	[SerializeField]
	private TabbedPanelContainer playerTabbedPanel;

	[SerializeField]
	private DockUI dockUI;

	[SerializeField]
	private DredgeDialogueView dialogueView;

	[SerializeField]
	private HarvestMinigameView harvestMinigameView;

	[SerializeField]
	private OccasionalGridPanel occasionalGridPanel;

	[SerializeField]
	private QuickHarvestGridPanel quickHarvestGridPanel;

	[SerializeField]
	private QuestGridPanel questGridPanel;

	[SerializeField]
	private UpgradeGridPanel upgradeGridPanel;

	[SerializeField]
	private PopupWindow mapWindow;

	[SerializeField]
	private PopupWindow journalWindow;

	[SerializeField]
	private PopupWindow messagesWindow;

	[SerializeField]
	private PopupWindow encyclopediaWindow;

	[SerializeField]
	private PopupWindow upgradeWindow;

	[SerializeField]
	private PopupWindow demoEndWindow;

	[SerializeField]
	private ResearchWindow researchWindow;

	[Header("Destination Panels")]
	[SerializeField]
	private GameObject destinationUIContainer;

	[SerializeField]
	private BaseDestinationUI characterDestinationUI;

	[SerializeField]
	private BaseDestinationUI restDestinationUI;

	[SerializeField]
	private BaseDestinationUI storageDestinationUI;

	[SerializeField]
	private BaseDestinationUI marketDestinationUI;

	[SerializeField]
	private BaseDestinationUI shipyardDestinationUI;

	[SerializeField]
	private BaseDestinationUI researchDestinationUI;

	[SerializeField]
	private BaseDestinationUI upgradeDestinationUI;

	[SerializeField]
	private BaseDestinationUI constructableDestinationUI;

	[SerializeField]
	private BaseDestinationUI overflowStorageUI;

	[Header("Other")]
	[SerializeField]
	private AbilityBarUI abilityBarUI;

	[SerializeField]
	private SpyglassUI spyglassUI;

	[SerializeField]
	private CanvasGroup topUICanvasGroup;

	[SerializeField]
	private InventoryPanelTab inventoryPanelTab;

	[SerializeField]
	private TextBannerWithSubtitleUI repairModeBanner;

	[SerializeField]
	private Image HUDCoverScrim;

	[SerializeField]
	private RectTransform tooltipAreaSingleGrid;

	[SerializeField]
	private RectTransform tooltipAreaDoubleGrid;

	private BaseDestination currentDestination;

	private BaseDestinationUI currentDestinationUI;

	private List<UIWindowType> showingWindowTypes = new List<UIWindowType>();

	private bool showingWindowTypesChangedThisFrame;

	private GameObject cachedSelectedObject;

	private bool cachedWasGameCanvasActive;

	private bool isAtDestination;

	private bool exitDestinationRequested;

	private enum GameOverDialogResult
	{
		RELOAD,
		MENU,
		CREDITS
	}
}
