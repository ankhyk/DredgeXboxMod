using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TabbedPanelContainer : MonoBehaviour, IScreenSideActivationResponder, IScreenSideSwitchResponder
{
	public List<TabConfig> TabbedPanels
	{
		get
		{
			return this.tabbedPanels;
		}
	}

	public int CurrentIndex
	{
		get
		{
			return this.currentIndex;
		}
	}

	public void RequestShowablePanels(List<int> showablePanelIndexes)
	{
		this.showablePanelIndexes.Clear();
		for (int i = 0; i < showablePanelIndexes.Count; i++)
		{
			this.showablePanelIndexes.Add(showablePanelIndexes[i]);
		}
	}

	private void ResetShowablePanels()
	{
		this.showablePanelIndexes.Clear();
		for (int i = 0; i < this.tabbedPanels.Count; i++)
		{
			this.showablePanelIndexes.Add(i);
		}
	}

	private void Awake()
	{
		if (!this.isInit)
		{
			this.Init();
		}
	}

	private void OnEnable()
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(false);
		}
		ApplicationEvents.Instance.OnSliderFocusToggled += this.OnSliderFocusToggled;
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnDetailPopupShowChange += this.OnDetailPopupShowChange;
		}
	}

	private void OnDisable()
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(false);
		}
		ApplicationEvents.Instance.OnSliderFocusToggled -= this.OnSliderFocusToggled;
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnDetailPopupShowChange -= this.OnDetailPopupShowChange;
		}
	}

	private void OnSliderFocusToggled(bool hasFocus)
	{
		if (hasFocus && GameManager.Instance.Input.IsUsingController)
		{
			this.RemoveTabInput();
			return;
		}
		this.AddTabInput();
	}

	private void OnDetailPopupShowChange(bool isShowing)
	{
		if (isShowing)
		{
			this.RemoveTabInput();
			return;
		}
		this.AddTabInput();
	}

	private void Init()
	{
		if (this.isInit)
		{
			return;
		}
		this.currentIndex = this.defaultPanelIndex;
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			t.panel.ScreenSide = this.screenSide;
		});
		this.leftActionPress = new DredgePlayerActionPress("Tab Left", GameManager.Instance.Input.Controls.TabLeft);
		this.leftActionPress.evaluateWhenPaused = this.evaluateWhenPaused;
		this.rightActionPress = new DredgePlayerActionPress("Tab Right", GameManager.Instance.Input.Controls.TabRight);
		this.rightActionPress.evaluateWhenPaused = this.evaluateWhenPaused;
		for (int i = 0; i < this.tabbedPanels.Count; i++)
		{
			int cachedIndex = i;
			this.tabbedPanels[i].tab.Button.onClick.AddListener(delegate
			{
				if (this.CanNavigateToTab(cachedIndex))
				{
					this.ShowNewPanel(cachedIndex, true);
				}
			});
		}
		this.isInit = true;
	}

	private void OnGridChangedActiveStatus(bool isActive)
	{
		if (isActive)
		{
			this.AddTabInput();
			return;
		}
		this.RemoveTabInput();
	}

	public void ForgetLastPanelIndex()
	{
		this.currentIndex = 0;
	}

	private void OnLeftPressComplete()
	{
		if (this.showablePanelIndexes.Count <= 1)
		{
			return;
		}
		int num = this.currentIndex - 1;
		int num2 = 0;
		while (num2 < 10 && !this.CanNavigateToTab(num))
		{
			num2++;
			num--;
			if (num < 0)
			{
				num = this.showablePanelIndexes[this.showablePanelIndexes.Count - 1];
			}
		}
		if (this.currentIndex != num)
		{
			this.ShowNewPanel(num, true);
			GameManager.Instance.AudioPlayer.PlaySFX(this.panelSwapSFX, AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	private void OnRightPressComplete()
	{
		if (this.showablePanelIndexes.Count <= 1)
		{
			return;
		}
		int num = this.currentIndex + 1;
		int num2 = 0;
		while (num2 < 10 && !this.CanNavigateToTab(num))
		{
			num2++;
			num++;
			if (num > this.showablePanelIndexes[this.showablePanelIndexes.Count - 1])
			{
				num = this.showablePanelIndexes[0];
			}
		}
		if (this.currentIndex != num)
		{
			this.ShowNewPanel(num, true);
			GameManager.Instance.AudioPlayer.PlaySFX(this.panelSwapSFX, AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	private bool CanNavigateToTab(int index)
	{
		return this.showablePanelIndexes.IndexOf(index) != -1 && this.tabbedPanels[index].tab.TabShowQuery.GetCanNavigate();
	}

	private bool CanShowTab(int index)
	{
		return this.showablePanelIndexes.IndexOf(index) != -1 && this.tabbedPanels[index].tab.TabShowQuery.GetCanShow();
	}

	public void ShowStart()
	{
		if (!this.isInit)
		{
			this.Init();
		}
		GameManager.Instance.ScreenSideSwitcher.RegisterActivationResponder(this, this.screenSide);
		if (this.handleSideSwitching)
		{
			GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, this.screenSide);
		}
		this.leftControlPrompt.Init(this.leftActionPress, this.leftActionPress.GetPrimaryPlayerAction());
		this.rightControlPrompt.Init(this.rightActionPress, this.rightActionPress.GetPrimaryPlayerAction());
		this.leftControlPrompt.gameObject.SetActive(this.showablePanelIndexes.Count > 1);
		this.rightControlPrompt.gameObject.SetActive(this.showablePanelIndexes.Count > 1);
		if (!this.rememberLastPanelVisited)
		{
			this.currentIndex = this.defaultPanelIndex;
		}
		if (this.showablePanelIndexes.IndexOf(this.currentIndex) == -1 || !this.CanNavigateToTab(this.currentIndex))
		{
			this.currentIndex = this.showablePanelIndexes[0];
		}
		this.ShowNewPanel(this.currentIndex, false);
		this.currentlyShowingTabConfig.panel.ShowStart();
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		GameManager.Instance.Input.SetActiveActionLayer(this.controlPromptActionLayer);
	}

	public void ShowFinish()
	{
		this.currentlyShowingTabConfig.panel.ShowFinish();
		this.AddTabInput();
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			TabShowQuery tabShowQuery = t.tab.TabShowQuery;
			tabShowQuery.canNavigateChanged = (Action<bool>)Delegate.Combine(tabShowQuery.canNavigateChanged, new Action<bool>(this.CanNavigateChanged));
		});
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			TabShowQuery tabShowQuery2 = t.tab.TabShowQuery;
			tabShowQuery2.canShowChanged = (Action<bool>)Delegate.Combine(tabShowQuery2.canShowChanged, new Action<bool>(this.CanShowChanged));
		});
	}

	public void HideStart()
	{
		GameManager.Instance.ScreenSideSwitcher.UnregisterActivationResponder(this, this.screenSide);
		if (this.handleSideSwitching)
		{
			GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, this.screenSide);
		}
		this.tabbedPanels[this.currentIndex].panel.HideStart();
		this.RemoveTabInput();
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			TabShowQuery tabShowQuery = t.tab.TabShowQuery;
			tabShowQuery.canNavigateChanged = (Action<bool>)Delegate.Remove(tabShowQuery.canNavigateChanged, new Action<bool>(this.CanNavigateChanged));
		});
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			TabShowQuery tabShowQuery2 = t.tab.TabShowQuery;
			tabShowQuery2.canShowChanged = (Action<bool>)Delegate.Remove(tabShowQuery2.canShowChanged, new Action<bool>(this.CanShowChanged));
		});
	}

	public void HideFinish()
	{
		this.tabbedPanels[this.currentIndex].panel.HideFinish();
		this.ResetShowablePanels();
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
	}

	private void CanNavigateChanged(bool canShow)
	{
		this.RefreshTabUIStates();
	}

	private void CanShowChanged(bool canShow)
	{
		this.RefreshTabUIStates();
	}

	public void EnableTabInput()
	{
		if (this.screenSide == ScreenSide.NONE || GameManager.Instance.ScreenSideSwitcher.CanSideBeActive(this.screenSide))
		{
			this.EnableTabShortcuts();
		}
		this.EnableTabButtons();
	}

	public void EnableTabShortcuts()
	{
		this.leftActionPress.ClearListeners();
		DredgePlayerActionPress dredgePlayerActionPress = this.leftActionPress;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnLeftPressComplete));
		this.leftActionPress.Enable();
		this.rightActionPress.ClearListeners();
		DredgePlayerActionPress dredgePlayerActionPress2 = this.rightActionPress;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnRightPressComplete));
		this.rightActionPress.Enable();
	}

	public void EnableTabButtons()
	{
		for (int i = 0; i < this.tabbedPanels.Count; i++)
		{
			this.tabbedPanels[i].tab.Button.interactable = this.showablePanelIndexes.IndexOf(i) != -1;
		}
	}

	public void DisableTabInput()
	{
		this.DisableTabShortcuts();
		this.DisableTabButtons();
	}

	public void DisableTabShortcuts()
	{
		this.leftActionPress.Disable(true);
		this.leftActionPress.ClearListeners();
		this.rightActionPress.Disable(true);
		this.rightActionPress.ClearListeners();
	}

	public void DisableTabButtons()
	{
		this.tabbedPanels.ForEach(delegate(TabConfig t)
		{
			t.tab.Button.interactable = false;
		});
	}

	private void AddTabInput()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.leftActionPress, this.rightActionPress };
		input.AddActionListener(array, this.controlPromptActionLayer);
		this.EnableTabInput();
	}

	private void RemoveTabInput()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.leftActionPress, this.rightActionPress };
		input.RemoveActionListener(array, this.controlPromptActionLayer);
		this.DisableTabInput();
	}

	public void ShowNewPanel(int newPanelIndex, bool showImmediate)
	{
		bool flag = newPanelIndex != this.currentIndex;
		this.currentIndex = newPanelIndex;
		if (!this.forbidSameTabReselection || (this.forbidSameTabReselection && flag))
		{
			Action<int> onTabChanged = this.OnTabChanged;
			if (onTabChanged != null)
			{
				onTabChanged(this.currentIndex);
			}
		}
		for (int i = 0; i < this.tabbedPanels.Count; i++)
		{
			if (i != this.currentIndex)
			{
				TabConfig tabConfig = this.tabbedPanels[i];
				if (tabConfig.panel.IsShowing())
				{
					tabConfig.panel.HideStart();
					tabConfig.panel.HideFinish();
				}
			}
		}
		this.currentlyShowingTabConfig = this.tabbedPanels[this.currentIndex];
		if (showImmediate)
		{
			this.currentlyShowingTabConfig.panel.ShowStart();
			this.currentlyShowingTabConfig.panel.ShowFinish();
		}
		this.RefreshTabUIStates();
	}

	private void RefreshTabUIStates()
	{
		for (int i = 0; i < this.tabbedPanels.Count; i++)
		{
			if (i == this.currentIndex)
			{
				this.tabbedPanels[i].tab.Button.image.sprite = this.tabbedPanels[i].tab.SelectedSprite;
			}
			else
			{
				bool flag = this.CanShowTab(i);
				bool flag2 = this.CanNavigateToTab(i);
				if (flag)
				{
					this.tabbedPanels[i].tab.Button.image.sprite = (flag2 ? this.tabbedPanels[i].tab.UnselectedSprite : this.tabbedPanels[i].tab.LockedSprite);
					this.tabbedPanels[i].tab.gameObject.SetActive(true);
				}
				else
				{
					this.tabbedPanels[i].tab.gameObject.SetActive(false);
				}
			}
		}
	}

	public void SetTabImage(int index, Sprite sprite)
	{
		this.tabbedPanels[index].tab.TabImage.sprite = sprite;
	}

	public void OnSideBecomeActive()
	{
		if (GameManager.Instance.GridManager && GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			return;
		}
		this.EnableTabShortcuts();
	}

	public void OnSideBecomeInactive()
	{
		this.DisableTabShortcuts();
	}

	private void OnDestroy()
	{
		GameManager.Instance.ScreenSideSwitcher.UnregisterActivationResponder(this, this.screenSide);
	}

	public void SwitchToSide()
	{
		this.tabbedPanels[this.currentIndex].panel.SwitchToSide();
	}

	public void ToggleSwitchIcon(bool show)
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(show);
		}
	}

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	private ControlPromptIcon leftControlPrompt;

	[SerializeField]
	private ControlPromptIcon rightControlPrompt;

	[SerializeField]
	private ActionLayer controlPromptActionLayer;

	[SerializeField]
	private bool evaluateWhenPaused;

	[SerializeField]
	private List<TabConfig> tabbedPanels;

	[SerializeField]
	private UIWindowType windowType;

	[SerializeField]
	private int defaultPanelIndex;

	[SerializeField]
	private bool rememberLastPanelVisited;

	[SerializeField]
	private AssetReference panelSwapSFX;

	[SerializeField]
	private ScreenSide screenSide;

	[SerializeField]
	private GameObject sideSwitchIcon;

	[SerializeField]
	private bool handleSideSwitching = true;

	[SerializeField]
	private bool forbidSameTabReselection;

	public Action<int> OnTabChanged;

	private DredgePlayerActionPress leftActionPress;

	private DredgePlayerActionPress rightActionPress;

	private int currentIndex;

	private TabConfig currentlyShowingTabConfig;

	private bool isInit;

	public List<int> showablePanelIndexes = new List<int>();
}
