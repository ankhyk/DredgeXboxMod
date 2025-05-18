using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandTerminal;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class GameManager : SerializedMonoBehaviour
{
	public bool IsRunnningOnSteamDeck { get; set; }

	public BuildInfo BuildInfo
	{
		get
		{
			return this._buildInfo;
		}
		set
		{
			this._buildInfo = value;
		}
	}

	public GameConfigData GameConfigData
	{
		get
		{
			return this._prodGameConfigData;
		}
	}

	public SaveManager SaveManager
	{
		get
		{
			return this._saveManager;
		}
	}

	public VibrationManager VibrationManager
	{
		get
		{
			return this._vibrationManager;
		}
	}

	public ConsoleManager ConsoleManager
	{
		get
		{
			return this._consoleManager;
		}
	}

	public SceneLoader Loader
	{
		get
		{
			return this._sceneLoader;
		}
	}

	public PauseListener PauseListener { get; set; }

	public LanguageManager LanguageManager
	{
		get
		{
			return this._languageManager;
		}
	}

	public DredgeDialogueRunner DialogueRunner { get; set; }

	public MonsterManager MonsterManager { get; set; }

	public ItemManager ItemManager
	{
		get
		{
			return this._itemManager;
		}
	}

	public AudioPlayer AudioPlayer { get; set; }

	public QuestManager QuestManager { get; set; }

	public PlayerCamera PlayerCamera { get; set; }

	public WorldEventManager WorldEventManager { get; set; }

	public CullingBrain CullingBrain { get; set; }

	public AchievementManager AchievementManager { get; set; }

	public EntitlementManager EntitlementManager { get; set; }

	public ItemLogicHandler ItemLogicHandler { get; set; }

	public DataLoader DataLoader
	{
		get
		{
			return this._dataLoader;
		}
	}

	public UpgradeManager UpgradeManager
	{
		get
		{
			return this._upgradeManager;
		}
	}

	public DredgeInputManager Input
	{
		get
		{
			return this._dredgeInputManager;
		}
		set
		{
			this._dredgeInputManager = value;
		}
	}

	public SaveData SaveData
	{
		get
		{
			return this._saveManager.ActiveSaveData;
		}
	}

	public SettingsSaveData SettingsSaveData
	{
		get
		{
			SaveManager saveManager = this._saveManager;
			if (saveManager == null)
			{
				return null;
			}
			return saveManager.ActiveSettingsData;
		}
	}

	public float ScaleFactor
	{
		get
		{
			return this.canvasScaler.transform.localScale.x;
		}
	}

	public ScreenSideSwitcher ScreenSideSwitcher
	{
		get
		{
			return this.screenSideSwitcher;
		}
	}

	public UIController UI { get; set; }

	public DialogManager DialogManager { get; set; }

	public GridManager GridManager { get; set; }

	public ResearchHelper ResearchHelper { get; set; }

	public TimeController Time { get; set; }

	public Player Player { get; set; }

	public PlayerStats PlayerStats { get; set; }

	public WaveController WaveController { get; set; }

	public PlayerAbilityManager PlayerAbilities { get; set; }

	public WeatherController WeatherController { get; set; }

	public HarvestValidator HarvestPOIManager { get; set; }

	public ChromaManager ChromaManager { get; set; }

	public OozePatchManager OozePatchManager { get; set; }

	public ConstructableBuildingManager ConstructableBuildingManager { get; set; }

	public bool HasLoadedAsyncManagers { get; private set; }

	public bool IsPaused { get; private set; }

	public bool IsPlaying { get; private set; }

	public bool CanPause { get; set; }

	public bool CanUnpause { get; set; }

	public UIFocusObject UIFocusObject { get; set; }

	private void Awake()
	{
		GameManager.Instance = this;
		global::UnityEngine.Debug.unityLogger.logEnabled = false;
		DOTween.logBehaviour = LogBehaviour.ErrorsOnly;
		this.IsRunnningOnSteamDeck = SystemInfo.deviceModel.ToLower().Contains("valve");
		Addressables.LoadAssetAsync<GameObject>(this.managerAudioAssetRef).Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(handle.Result, base.transform);
				return;
			}
			CustomDebug.EditorLogError("[GameManager] Failed to load AudioPlayer prefab.");
		};
		this.LoadBuildInfoAddressable();
		this._consoleManager.Init();
	}

	private void Start()
	{
		Task.Run(new Action(this.WaitForAllAsyncManagers));
		this.AddTerminalCommands();
	}

	public async void WaitForAllAsyncManagers()
	{
		while (!this.ConsoleManager.Initialized)
		{
			await Awaiters.NextFrame;
		}
		global::UnityEngine.Debug.Log("[GameManager] WaitForAllAsyncManagers() After1");
		await Awaiters.MainThread;
		global::UnityEngine.Debug.Log("[GameManager] WaitForAllAsyncManagers() After2");
		this._saveManager.Init();
		this._vibrationManager.Init();
		await this.AchievementManager.Init();
		await this.EntitlementManager.Init();
		this.HasLoadedAsyncManagers = true;
	}

	public void BeginGame()
	{
		this.IsPlaying = true;
		this.CanPause = true;
		Action onGameStarted = this.OnGameStarted;
		if (onGameStarted == null)
		{
			return;
		}
		onGameStarted();
	}

	public void EndGame()
	{
		this.IsPlaying = false;
		this.CanPause = false;
		Action onGameEnded = this.OnGameEnded;
		if (onGameEnded == null)
		{
			return;
		}
		onGameEnded();
	}

	public void Save()
	{
		this.SaveManager.Save();
	}

	public void SaveSettings()
	{
		this.SaveManager.SaveSettings();
	}

	public void PauseAndShowSettings()
	{
		if (!this.IsPaused && this.CanPause)
		{
			global::UnityEngine.Time.timeScale = 0f;
			AudioListener.pause = true;
			this.IsPaused = true;
			this.CanPause = false;
			this.CanUnpause = true;
			GameEvents.Instance.TriggerPauseChange(this.IsPaused);
		}
	}

	public void UnpauseAndDismissSettings()
	{
		if (this.IsPaused && this.CanUnpause)
		{
			global::UnityEngine.Time.timeScale = 1f;
			AudioListener.pause = false;
			this.IsPaused = false;
			this.CanPause = true;
			this.CanUnpause = false;
			GameEvents.Instance.TriggerPauseChange(this.IsPaused);
		}
	}

	public void PauseLite()
	{
		global::UnityEngine.Time.timeScale = 0f;
		AudioListener.pause = true;
		this.IsPaused = true;
	}

	public void UnpauseLite()
	{
		this.IsPaused = false;
		AudioListener.pause = false;
		global::UnityEngine.Time.timeScale = 1f;
	}

	public void GameOver(GameOverMode endMode)
	{
		this.EndGame();
		this.PauseAndShowSettings();
		if (endMode == GameOverMode.DEATH)
		{
			this.UI.ShowGameOverDialog();
			return;
		}
		if (endMode != GameOverMode.DEMO_END)
		{
			return;
		}
		this.UI.ShowDemoOverDialog();
	}

	public void AddFunds(decimal changeAmount)
	{
		if (changeAmount != 0m)
		{
			this.SaveData.Funds += changeAmount;
			this.SaveData.Funds = decimal.Round(this.SaveData.Funds, 2);
			GameEvents.Instance.TriggerPlayerFundsChanged(this.SaveData.Funds, changeAmount);
		}
	}

	private void Update()
	{
		float num = 1000f;
		if (this.Time && this.Time.IsTimePassingForcefully())
		{
			this.gameTime += global::UnityEngine.Time.deltaTime * this.GameConfigData.ForcedTimePassageSpeedModifier;
		}
		else
		{
			this.gameTime += global::UnityEngine.Time.deltaTime;
		}
		if (this.gameTime >= num)
		{
			this.gameTime -= num;
		}
		Shader.SetGlobalFloat("_GameTime", this.gameTime);
		Shader.SetGlobalFloat("_UnscaledTime", global::UnityEngine.Time.unscaledTime);
	}

	public bool hasBankedId
	{
		get
		{
			return this.bankedID != null;
		}
	}

	private void OnActivityCardSelected(string activityId)
	{
		if (!this.ConsoleManager.Initialized)
		{
			this.bankedID = activityId;
		}
	}

	private void LoadBuildInfoAddressable()
	{
		Addressables.LoadAssetAsync<BuildInfo>(this.buildInfoReference).Completed += delegate(AsyncOperationHandle<BuildInfo> response)
		{
			if (response.Status == AsyncOperationStatus.Succeeded)
			{
				this._buildInfo = response.Result;
				ApplicationEvents.Instance.TriggerBuildInfoChanged();
			}
		};
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("func.map", new Action<CommandArg[]>(this.ToggleAdvancedMap), 1, 1, "Enables or disables the advanced map functionality.");
		Terminal.Shell.AddCommand("func.photo", new Action<CommandArg[]>(this.TogglePhotoMode), 1, 1, "Enables or disables photo mode.");
		Terminal.Shell.AddCommand("func.savecrash", new Action<CommandArg[]>(this.ToggleSaveCrashes), 1, 1, "Enables or disables forced save crashes.");
		Terminal.Shell.AddCommand("func.savebad", new Action<CommandArg[]>(this.ToggleSaveBadData), 1, 1, "Enables or disables writing bad data.");
		Terminal.Shell.AddCommand("func.loadbad", new Action<CommandArg[]>(this.ToggleLoadBadData), 1, 1, "Enables or disables loading bad data.");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("func.map");
		Terminal.Shell.RemoveCommand("func.photo");
		Terminal.Shell.RemoveCommand("func.savecrash");
		Terminal.Shell.RemoveCommand("func.savebad");
		Terminal.Shell.RemoveCommand("func.loadbad");
	}

	private void ToggleAdvancedMap(CommandArg[] args)
	{
		if (this.IsPlaying)
		{
			CustomDebug.EditorLogError("Please don't run this command during gameplay! Return to the main menu and apply it there.");
			return;
		}
		this._buildInfo.advancedMap = args[0].Int == 1;
		ApplicationEvents.Instance.TriggerBuildInfoChanged();
	}

	private void TogglePhotoMode(CommandArg[] args)
	{
		if (this.IsPlaying)
		{
			CustomDebug.EditorLogError("Please don't run this command during gameplay! Return to the main menu and apply it there.");
			return;
		}
		this._buildInfo.photoMode = args[0].Int == 1;
		ApplicationEvents.Instance.TriggerBuildInfoChanged();
	}

	private void ToggleSaveCrashes(CommandArg[] args)
	{
		this.SaveManager.forceCrashDuringSave = args[0].Int == 1;
	}

	private void ToggleSaveBadData(CommandArg[] args)
	{
		this.SaveManager.forceSaveBadData = args[0].Int == 1;
	}

	private void ToggleLoadBadData(CommandArg[] args)
	{
		this.SaveManager.forceLoadBadData = args[0].Int == 1;
	}

	private void OnApplicationQuit()
	{
		Process.GetCurrentProcess().Kill();
	}

	public static GameManager Instance;

	[SerializeField]
	private GameConfigData _prodGameConfigData;

	[SerializeField]
	private GameConfigData _demoGameConfigData;

	[SerializeField]
	private AssetReference buildInfoReference;

	private BuildInfo _buildInfo;

	[SerializeField]
	private AssetReference managerAudioAssetRef;

	[SerializeField]
	private SaveManager _saveManager;

	[SerializeField]
	private VibrationManager _vibrationManager;

	[SerializeField]
	private ConsoleManager _consoleManager;

	[SerializeField]
	private SceneLoader _sceneLoader;

	[SerializeField]
	private LanguageManager _languageManager;

	[SerializeField]
	private ItemManager _itemManager;

	[SerializeField]
	private DataLoader _dataLoader;

	[SerializeField]
	private UpgradeManager _upgradeManager;

	[SerializeField]
	private DredgeInputManager _dredgeInputManager;

	[SerializeField]
	private CanvasScaler canvasScaler;

	[SerializeField]
	private ScreenSideSwitcher screenSideSwitcher;

	public float gameTime;

	[HideInInspector]
	public Action OnGameStarted;

	[HideInInspector]
	public Action OnGameEnded;

	private string bankedID;
}
