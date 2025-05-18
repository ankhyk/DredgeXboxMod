using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static bool BetweenScenes { get; private set; }

	public LoadingScreen LoadingScreen
	{
		get
		{
			return this.loadingScreen;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (SceneManager.sceneCount == 1)
		{
			this.WaitForConsoleManager();
			return;
		}
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == "Game")
			{
				SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
			}
		}
	}

	public async Task WaitForConsoleManager()
	{
		Debug.Log("WaitForConsoleManager - Start");
		while (!GameManager.Instance.HasLoadedAsyncManagers)
		{
			await Task.Delay(30);
		}
		Debug.Log("WaitForConsoleManager - End");
		Debug.Log("[GDK PC Resolution] Start");
		if (GameManager.Instance.SettingsSaveData == null)
		{
			Debug.Log("[GDK PC Resolution] Failed to find loaded SettingsSaveData");
		}
		if (GameManager.Instance.SettingsSaveData == null || GameManager.Instance.SettingsSaveData.resx == 0 || GameManager.Instance.SettingsSaveData.resy == 0 || !ResolutionHelper.IsSupportedAspectRatio((float)GameManager.Instance.SettingsSaveData.resx, (float)GameManager.Instance.SettingsSaveData.resy))
		{
			Debug.Log("[GDK PC Resolution] No Supported setting default");
			Resolution bestSupportedResolution = ResolutionHelper.GetBestSupportedResolution();
			if (bestSupportedResolution.width != 0 && bestSupportedResolution.height != 0)
			{
				Screen.SetResolution(bestSupportedResolution.width, bestSupportedResolution.height, (FullScreenMode)GameManager.Instance.SettingsSaveData.FullScreenMode);
			}
			Debug.Log(string.Format("[GDK PC Resolution] Set {0}X{1} FullScreen:{2}", bestSupportedResolution.width, bestSupportedResolution.height, GameManager.Instance.SettingsSaveData.FullScreenMode.ToString()));
			GameManager.Instance.SettingsSaveData.resx = bestSupportedResolution.width;
			GameManager.Instance.SettingsSaveData.resx = bestSupportedResolution.height;
			GameManager.Instance.SaveManager.SaveSettings();
		}
		else
		{
			Debug.Log(string.Format("[GDK PC Resolution] Supported Set {0}X{1} FullScreen:{2}", GameManager.Instance.SettingsSaveData.resx, GameManager.Instance.SettingsSaveData.resy, GameManager.Instance.SettingsSaveData.FullScreenMode));
			Screen.SetResolution(GameManager.Instance.SettingsSaveData.resx, GameManager.Instance.SettingsSaveData.resy, (FullScreenMode)GameManager.Instance.SettingsSaveData.FullScreenMode);
		}
		base.StartCoroutine(this.MoveToSavedDisplay());
		Debug.Log("Start Splash Load");
		this.LoadSplash();
	}

	private IEnumerator MoveToSavedDisplay()
	{
		Debug.Log("MoveToSavedDisplay");
		List<DisplayInfo> list = new List<DisplayInfo>();
		Screen.GetDisplayLayout(list);
		DisplayInfo mainWindowDisplayInfo = Screen.mainWindowDisplayInfo;
		if (list.Count - 1 >= GameManager.Instance.SettingsSaveData.DisplayIndex)
		{
			DisplayInfo displayInfo = list[GameManager.Instance.SettingsSaveData.DisplayIndex];
			Debug.Log("[GDK PC Resolution] Found valid window to move to");
			if (Screen.fullScreenMode == FullScreenMode.Windowed)
			{
				Debug.Log("[GDK PC Resolution] We are in windowed mode so force size max size to screen");
				displayInfo.width = Math.Min(displayInfo.width, GameManager.Instance.SettingsSaveData.resx);
				displayInfo.height = Math.Min(displayInfo.width, GameManager.Instance.SettingsSaveData.resy);
			}
			Vector2Int vector2Int = new Vector2Int(0, 0);
			AsyncOperation asyncOperation = Screen.MoveMainWindowTo(in displayInfo, vector2Int);
			yield return asyncOperation;
		}
		else
		{
			Debug.Log("[GDK PC Resolution] Saved window is no longer valid. Not moving");
		}
		yield break;
	}

	private bool ShouldShowSplashScreen()
	{
		return !GameManager.Instance.SaveManager.HasAnySaveFiles();
	}

	private IEnumerator DoSwitchSceneRequest(SceneLoader.SceneSwitchRequest request)
	{
		Application.backgroundLoadingPriority = ThreadPriority.High;
		if (request.doUnload)
		{
			if (request.unloadMixerSnapshot != SnapshotType.NONE && GameManager.Instance.AudioPlayer)
			{
				GameManager.Instance.AudioPlayer.TransitionToSnapshot(request.unloadMixerSnapshot, this.snapshotTransitionDurationSec);
			}
			if (request.showLoadingScreenOnUnload)
			{
				AutoSplitterData.isLoading = true;
				SceneLoader.BetweenScenes = true;
				this.loadingScreen.Fade(true, true);
				yield return new WaitUntil(new Func<bool>(this.loadingScreen.HasAnimatedLoadingScreen));
			}
			if (request.unloadHandle.IsValid())
			{
				AsyncOperationHandle<SceneInstance> unloadScene = Addressables.UnloadSceneAsync(request.unloadHandle, true);
				while (!unloadScene.IsDone)
				{
					yield return null;
				}
				Action<AsyncOperationHandle<SceneInstance>> unloadCallback = request.unloadCallback;
				if (unloadCallback != null)
				{
					unloadCallback(request.unloadHandle);
				}
				unloadScene = default(AsyncOperationHandle<SceneInstance>);
			}
		}
		yield return Resources.UnloadUnusedAssets();
		if (request.doLoad)
		{
			request.loadHandle = Addressables.LoadSceneAsync(request.loadSceneReference, LoadSceneMode.Additive, true, 100);
			request.loadHandle.Completed += delegate(AsyncOperationHandle<SceneInstance> scene)
			{
				SceneManager.SetActiveScene(scene.Result.Scene);
			};
			while (!request.loadHandle.IsDone)
			{
				yield return null;
			}
			Action<AsyncOperationHandle<SceneInstance>> loadCallback = request.loadCallback;
			if (loadCallback != null)
			{
				loadCallback(request.loadHandle);
			}
			if (request.loadMixerSnapshot != SnapshotType.NONE && GameManager.Instance.AudioPlayer)
			{
				GameManager.Instance.AudioPlayer.TransitionToSnapshot(request.loadMixerSnapshot, this.snapshotTransitionDurationSec);
			}
			if (request.hideLoadingScreenOnLoad)
			{
				SceneLoader.BetweenScenes = false;
				this.loadingScreen.Fade(false, true);
				yield return new WaitUntil(new Func<bool>(this.loadingScreen.HasAnimatedLoadingScreen));
				AutoSplitterData.isLoading = false;
			}
		}
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
		yield break;
	}

	public void LoadSplash()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doLoad = true,
			loadSceneReference = this.splashSceneReference,
			loadHandle = this.splashSceneHandle,
			loadCallback = delegate(AsyncOperationHandle<SceneInstance> loadHandle)
			{
				this.splashSceneHandle = loadHandle;
			},
			hideLoadingScreenOnLoad = false,
			unloadMixerSnapshot = SnapshotType.NONE,
			loadMixerSnapshot = SnapshotType.NONE
		}));
	}

	public void LoadStartupFromSplash()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.splashSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.splashSceneHandle = unloadHandle;
			},
			doLoad = true,
			loadSceneReference = this.startupSceneReference,
			loadHandle = this.startupSceneHandle,
			loadCallback = delegate(AsyncOperationHandle<SceneInstance> loadHandle)
			{
				this.startupSceneHandle = loadHandle;
			},
			showLoadingScreenOnUnload = true,
			hideLoadingScreenOnLoad = this.ShouldShowSplashScreen(),
			unloadMixerSnapshot = SnapshotType.NONE,
			loadMixerSnapshot = SnapshotType.MENU
		}));
	}

	public void LoadTitleFromStartup()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.startupSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.startupSceneHandle = unloadHandle;
			},
			doLoad = true,
			loadSceneReference = this.titleSceneReference,
			loadHandle = this.titleSceneHandle,
			loadCallback = delegate(AsyncOperationHandle<SceneInstance> loadHandle)
			{
				this.titleSceneHandle = loadHandle;
			},
			showLoadingScreenOnUnload = this.ShouldShowSplashScreen(),
			hideLoadingScreenOnLoad = true,
			unloadMixerSnapshot = SnapshotType.NONE,
			loadMixerSnapshot = SnapshotType.MENU
		}));
	}

	public void LoadGameFromStartup()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.startupSceneHandle,
			showLoadingScreenOnUnload = !GameManager.Instance.SaveManager.HasAnySaveFiles(),
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.startupSceneHandle = unloadHandle;
				GameManager.Instance.SaveManager.LoadLast(true);
				base.StartCoroutine(this.LoadGameScene());
			}
		}));
	}

	public void LoadTitleFromGame()
	{
		GameManager.Instance.EndGame();
		AutoSplitterData.isRunning = 0;
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.gameSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.gameSceneHandle = unloadHandle;
				ApplicationEvents.Instance.TriggerGameUnloaded();
			},
			doLoad = true,
			loadSceneReference = this.titleSceneReference,
			loadHandle = this.titleSceneHandle,
			loadCallback = delegate(AsyncOperationHandle<SceneInstance> loadHandle)
			{
				this.titleSceneHandle = loadHandle;
				GameManager.Instance.CanUnpause = true;
				GameManager.Instance.UnpauseAndDismissSettings();
			},
			showLoadingScreenOnUnload = true,
			hideLoadingScreenOnLoad = true,
			unloadMixerSnapshot = SnapshotType.LOADING,
			loadMixerSnapshot = SnapshotType.MENU
		}));
	}

	public void LoadGameFromTitle(bool canCreateNew = true)
	{
		ApplicationEvents.Instance.TriggerTitleClose();
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.titleSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.titleSceneHandle = unloadHandle;
				GameManager.Instance.SaveManager.LoadLast(canCreateNew);
				this.StartCoroutine(this.LoadGameScene());
			},
			showLoadingScreenOnUnload = true,
			unloadMixerSnapshot = SnapshotType.LOADING
		}));
	}

	public void LoadGameFromActivityCard()
	{
		if (this.titleSceneHandle.IsValid())
		{
			base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
			{
				doUnload = true,
				unloadHandle = this.titleSceneHandle,
				unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
				{
					this.titleSceneHandle = unloadHandle;
					GameManager.Instance.SaveManager.LoadLast(true);
					base.StartCoroutine(this.LoadGameScene());
				},
				showLoadingScreenOnUnload = true,
				unloadMixerSnapshot = SnapshotType.LOADING
			}));
			return;
		}
		GameManager.Instance.SaveManager.LoadLast(true);
		base.StartCoroutine(this.LoadGameScene());
	}

	public void LoadGameFromCutscene()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.cutsceneSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.cutsceneSceneHandle = unloadHandle;
				base.StartCoroutine(this.LoadGameScene());
			},
			showLoadingScreenOnUnload = true,
			unloadMixerSnapshot = SnapshotType.LOADING
		}));
	}

	public void ReloadGame()
	{
		base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
		{
			doUnload = true,
			unloadHandle = this.gameSceneHandle,
			unloadCallback = delegate(AsyncOperationHandle<SceneInstance> unloadHandle)
			{
				this.gameSceneHandle = unloadHandle;
				ApplicationEvents.Instance.TriggerGameUnloaded();
				GameManager.Instance.SaveManager.LoadLast(true);
				base.StartCoroutine(this.LoadGameScene());
			},
			showLoadingScreenOnUnload = true,
			unloadMixerSnapshot = SnapshotType.LOADING
		}));
	}

	private IEnumerator LoadGameScene()
	{
		AutoSplitterData.isRunning = 1;
		if (GameManager.Instance.SaveData.HasViewedIntroCutscene)
		{
			GameManager.Instance.ItemManager.Load();
			GameManager.Instance.DataLoader.Load();
			while (!GameManager.Instance.ItemManager.HasLoaded() || !GameManager.Instance.DataLoader.HasLoaded())
			{
				yield return null;
			}
			GameManager.Instance.SaveData.Init();
			this.gameSceneHandle = Addressables.LoadSceneAsync(this.gameSceneReference, LoadSceneMode.Additive, true, 100);
			this.gameSceneHandle.Completed += delegate(AsyncOperationHandle<SceneInstance> scene)
			{
				SceneManager.SetActiveScene(scene.Result.Scene);
			};
			while (!this.gameSceneHandle.IsDone || !(GameSceneInitializer.Instance != null) || !GameSceneInitializer.Instance.IsDone())
			{
				yield return null;
			}
			ApplicationEvents.Instance.TriggerGameLoaded();
			GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_OUTDOORS, this.snapshotTransitionDurationSec);
			yield return new WaitForSeconds(0.5f);
			SceneLoader.BetweenScenes = false;
			this.loadingScreen.Fade(false, true);
			yield return new WaitUntil(new Func<bool>(this.loadingScreen.HasAnimatedLoadingScreen));
			AutoSplitterData.isLoading = false;
			GameManager.Instance.CanUnpause = true;
			GameManager.Instance.UnpauseAndDismissSettings();
			GameManager.Instance.AudioPlayer.PlaySFX(this.sceneLoadChimeSFX, AudioLayer.SFX_UI, 1f, 1f);
			ApplicationEvents.Instance.TriggerGameStartable();
		}
		else
		{
			base.StartCoroutine(this.DoSwitchSceneRequest(new SceneLoader.SceneSwitchRequest
			{
				doLoad = true,
				loadSceneReference = this.cutsceneSceneReference,
				loadHandle = this.cutsceneSceneHandle,
				loadCallback = delegate(AsyncOperationHandle<SceneInstance> loadHandle)
				{
					this.cutsceneSceneHandle = loadHandle;
				},
				hideLoadingScreenOnLoad = true,
				unloadMixerSnapshot = SnapshotType.LOADING,
				loadMixerSnapshot = SnapshotType.MENU
			}));
		}
		yield break;
	}

	private AsyncOperationHandle<SceneInstance> splashSceneHandle;

	private AsyncOperationHandle<SceneInstance> startupSceneHandle;

	private AsyncOperationHandle<SceneInstance> titleSceneHandle;

	private AsyncOperationHandle<SceneInstance> gameSceneHandle;

	private AsyncOperationHandle<SceneInstance> cutsceneSceneHandle;

	[SerializeField]
	private float snapshotTransitionDurationSec;

	[SerializeField]
	private AssetReference splashSceneReference;

	[SerializeField]
	private AssetReference startupSceneReference;

	[SerializeField]
	private AssetReference titleSceneReference;

	[SerializeField]
	private AssetReference cutsceneSceneReference;

	[SerializeField]
	private AssetReference gameSceneReference;

	[SerializeField]
	private LoadingScreen loadingScreen;

	[SerializeField]
	private AssetReference sceneLoadChimeSFX;

	private struct SceneSwitchRequest
	{
		public bool doUnload;

		public bool doLoad;

		public AssetReference loadSceneReference;

		public AsyncOperationHandle<SceneInstance> loadHandle;

		public AsyncOperationHandle<SceneInstance> unloadHandle;

		public Action<AsyncOperationHandle<SceneInstance>> loadCallback;

		public Action<AsyncOperationHandle<SceneInstance>> unloadCallback;

		public SnapshotType loadMixerSnapshot;

		public SnapshotType unloadMixerSnapshot;

		public bool showLoadingScreenOnUnload;

		public bool hideLoadingScreenOnLoad;
	}
}
