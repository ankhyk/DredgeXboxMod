using System;
using System.Collections.Generic;
using CommandTerminal;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	public bool IsAlive { get; private set; }

	public bool IsDocked { get; private set; }

	public bool IsFishing { get; private set; }

	public Dock CurrentDock { get; private set; }

	public Dock PreviousDock { get; private set; }

	public bool CanMoveInstalledItems { get; set; }

	public BoatModelProxy BoatModelProxy
	{
		get
		{
			return this._boatModelProxy;
		}
		set
		{
			this._boatModelProxy = value;
		}
	}

	public PlayerController Controller
	{
		get
		{
			return this._controller;
		}
	}

	public PlayerSanity Sanity
	{
		get
		{
			return this._sanity;
		}
	}

	public SanityModifierDetector SanityModifierDetector
	{
		get
		{
			return this._sanityModifierDetector;
		}
	}

	public DockPOIHandler Docker
	{
		get
		{
			return this._docker;
		}
	}

	public Harvester Harvester
	{
		get
		{
			return this._harvester;
		}
	}

	public HarvestZoneDetector HarvestZoneDetector
	{
		get
		{
			return this._harvestZoneDetector;
		}
	}

	public PlayerZoneDetector PlayerZoneDetector
	{
		get
		{
			return this._playerZoneDetector;
		}
	}

	public Transform ColliderCenter
	{
		get
		{
			return this._colliderCenter;
		}
	}

	public Transform DevilsSpineMonsterAttachPoint
	{
		get
		{
			return this._devilsSpineMonsterAttachPoint;
		}
	}

	public PlayerCollisionAudio PlayerCollisionAudio
	{
		get
		{
			return this._playerCollisionAudio;
		}
	}

	public bool IsGodModeEnabled { get; private set; }

	public bool IsImmuneModeEnabled { get; set; }

	public DepthMonitor PlayerDepthMonitor
	{
		get
		{
			return this._playerDepthMonitor;
		}
	}

	public PlayerTeleport PlayerTeleport
	{
		get
		{
			return this._playerTeleport;
		}
	}

	public int RemainingHealth
	{
		get
		{
			return GameManager.Instance.PlayerStats.DamageThreshold + 1 - GameManager.Instance.SaveData.GetNumberOfDamagedSlots();
		}
	}

	public PlayerCollider Collider
	{
		get
		{
			return this._collider;
		}
	}

	public VibrationData SafeCollisionVibration
	{
		get
		{
			return this._safeCollisionVibration;
		}
	}

	public VibrationData SlowBoatVibration
	{
		get
		{
			return this._slowBoatVibration;
		}
	}

	public VibrationData MediumBoatVibration
	{
		get
		{
			return this._mediumBoatVibration;
		}
	}

	public VibrationData FastBoatVibration
	{
		get
		{
			return this._fastBoatVibration;
		}
	}

	public VibrationData SlowCollisionVibration
	{
		get
		{
			return this._slowCollisionVibration;
		}
	}

	public VibrationData MediumCollisionVibration
	{
		get
		{
			return this._mediumCollisionVibration;
		}
	}

	public VibrationData FastCollisionVibration
	{
		get
		{
			return this._fastCollisionVibration;
		}
	}

	public VibrationData EnemyAttackVibration
	{
		get
		{
			return this._enemyAttackVibration;
		}
	}

	public VibrationData FishingBlipVibration
	{
		get
		{
			return this._fishingBlipVibration;
		}
	}

	public VibrationData FishingSuccessVibration
	{
		get
		{
			return this._fishingSuccessVibration;
		}
	}

	public VibrationData FishingFailVibration
	{
		get
		{
			return this._fishingFailVibration;
		}
	}

	public VibrationData DredgingContinuousVibration
	{
		get
		{
			return this._dredgingContinuousVibration;
		}
	}

	public VibrationData DredgingSwitchLaneVibration
	{
		get
		{
			return this._dredgingSwitchLanesVibration;
		}
	}

	public VibrationData DredgingSuccessVibration
	{
		get
		{
			return this._dredgingSuccessVibration;
		}
	}

	public VibrationData DredgingFailVibration
	{
		get
		{
			return this._dredgingFailVibration;
		}
	}

	private void OnEnable()
	{
		this.AdjustHullToTier(GameManager.Instance.SaveData.HullTier);
	}

	private void Awake()
	{
		GameManager.Instance.Player = this;
		this.IsAlive = true;
		this.IsDocked = false;
		this.Controller.IsMovementAllowed = true;
		this.Sanity.PlayerRef = this;
		this.Controller.PlayerRef = this;
		this.Docker.PlayerRef = this;
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
		GameEvents.Instance.OnActivelyHarvestingChanged += this.OnHarvestMinigameToggled;
		this.Collider.OnCollisionEvent.AddListener(new UnityAction(this.OnCollision));
		PlayerCollider collider = this.Collider;
		collider.OnCollisionVibrationEvent = (Action<bool>)Delegate.Combine(collider.OnCollisionVibrationEvent, new Action<bool>(this.OnCollisionVibration));
		this.Collider.OnSafeCollisionEvent.AddListener(new UnityAction(this.OnSafeCollision));
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
		GameEvents.Instance.OnActivelyHarvestingChanged -= this.OnHarvestMinigameToggled;
		this.Collider.OnCollisionEvent.RemoveListener(new UnityAction(this.OnCollision));
		PlayerCollider collider = this.Collider;
		collider.OnCollisionVibrationEvent = (Action<bool>)Delegate.Remove(collider.OnCollisionVibrationEvent, new Action<bool>(this.OnCollisionVibration));
		this.Collider.OnSafeCollisionEvent.RemoveListener(new UnityAction(this.OnSafeCollision));
		GameManager.Instance.Player = null;
	}

	public void ToggleBoatModel(bool enabled)
	{
		this._boatModelProxy.gameObject.SetActive(enabled);
	}

	public void ToggleBoatEngineAudio(bool enabled)
	{
		this.playerEngineAudio.gameObject.SetActive(enabled);
	}

	public void IgnoreDamage()
	{
		this.Collider.OnCollisionEvent.RemoveListener(new UnityAction(this.OnCollision));
		PlayerCollider collider = this.Collider;
		collider.OnCollisionVibrationEvent = (Action<bool>)Delegate.Remove(collider.OnCollisionVibrationEvent, new Action<bool>(this.OnCollisionVibration));
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		if (upgradeData is HullUpgradeData)
		{
			this.AdjustHullToTier(upgradeData.tier);
		}
	}

	private void AdjustHullToTier(int tier)
	{
		int num = Mathf.Min(this._allBoatModelProxies.Count, tier) - 1;
		for (int i = 0; i < this._allBoatModelProxies.Count; i++)
		{
			this._allBoatModelProxies[i].gameObject.SetActive(i == num);
		}
		this.BoatModelProxy = this._allBoatModelProxies[num];
		GameEvents.Instance.TriggerBoatModelChangedEvent();
	}

	public bool IsBelowInsaneTooltipThreshold()
	{
		return this.Sanity.CurrentSanity <= GameManager.Instance.GameConfigData.InsaneTooltipThreshold;
	}

	private void Start()
	{
		this.AddTerminalCommands();
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	private void OnCollision()
	{
		if (this.IsAlive)
		{
			GameManager.Instance.GridManager.AddDamageToInventory(1, -1, -1);
		}
	}

	private void OnSafeCollision()
	{
		GameManager.Instance.VibrationManager.Vibrate(this.SafeCollisionVibration, VibrationRegion.WholeBody, false);
	}

	private void OnCollisionVibration(bool isMonster)
	{
		if (isMonster)
		{
			GameManager.Instance.VibrationManager.Vibrate(this.EnemyAttackVibration, VibrationRegion.WholeBody, true);
			return;
		}
		GameManager.Instance.VibrationManager.Vibrate(this.FastCollisionVibration, VibrationRegion.WholeBody, true);
	}

	private void OnPlayerDamageChanged()
	{
		if (this.IsGodModeEnabled || !this.IsAlive)
		{
			return;
		}
		if (GameManager.Instance.SaveData.GetNumberOfDamagedSlots() > GameManager.Instance.PlayerStats.DamageThreshold)
		{
			this.Die();
		}
	}

	private void OnHarvestMinigameToggled(bool enabled)
	{
		this.IsFishing = enabled;
	}

	public void Dock(Dock dock, int dockSlotIndex, bool updateSave)
	{
		this.IsDocked = true;
		this.CurrentDock = dock;
		this.Controller.IsMovementAllowed = false;
		GameManager.Instance.SaveData.SetBoolVariable("has-visited-dock-" + dock.Data.Id, true);
		if (updateSave)
		{
			GameManager.Instance.SaveData.dockId = dock.Data.Id;
			GameManager.Instance.SaveData.dockSlotIndex = dockSlotIndex;
			GameManager.Instance.Save();
		}
		GameEvents.Instance.TogglePlayerDocked(dock);
	}

	public void Undock()
	{
		if (!this.hasRequestedUndockSave)
		{
			this.hasRequestedUndockSave = true;
			SaveManager saveManager = GameManager.Instance.SaveManager;
			saveManager.OnSaveComplete = (Action)Delegate.Combine(saveManager.OnSaveComplete, new Action(this.OnUndockSaveComplete));
			GameManager.Instance.Save();
		}
	}

	private void OnUndockSaveComplete()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveComplete = (Action)Delegate.Remove(saveManager.OnSaveComplete, new Action(this.OnUndockSaveComplete));
		this.IsDocked = false;
		this.PreviousDock = this.CurrentDock;
		this.CurrentDock = null;
		this.Controller.IsMovementAllowed = true;
		GameEvents.Instance.TogglePlayerDocked(null);
		this.hasRequestedUndockSave = false;
	}

	public void Die()
	{
		if (this.IsGodModeEnabled || !this.IsAlive)
		{
			return;
		}
		this._collider.enabled = false;
		this.IsAlive = false;
		GameManager.Instance.GameOver(GameOverMode.DEATH);
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("player.die", new Action<CommandArg[]>(this.Die), 0, 0, "Kills the player");
		Terminal.Shell.AddCommand("player.god", new Action<CommandArg[]>(this.SetGodMode), 1, 1, "Sets godmode 0 = off | 1 = on");
		Terminal.Shell.AddCommand("player.immune", new Action<CommandArg[]>(this.SetImmuneMode), 1, 1, "Sets immune to equipment damage 0 = off | 1 = on");
		Terminal.Shell.AddCommand("dest.unlock", new Action<CommandArg[]>(this.UnlockDestination), 1, 1, "Unlocks a destination by id");
		Terminal.Shell.AddCommand("dest.list", new Action<CommandArg[]>(this.ListDestinations), 0, 0, "Lists all destination ids");
		Terminal.Shell.AddCommand("icebreaker", new Action<CommandArg[]>(this.ToggleIcebreaker), 1, 1, "Sets icebreaker 0 = off | 1 = on");
	}

	private void ListDestinations(CommandArg[] args)
	{
		Dock[] array = global::UnityEngine.Object.FindObjectsOfType<Dock>();
		for (int i = 0; i < array.Length; i++)
		{
		}
	}

	private void UnlockDestination(CommandArg[] args)
	{
		string @string = args[0].String;
		GameManager.Instance.SaveData.availableDestinations.Add(@string);
	}

	private void SetGodMode(CommandArg[] args)
	{
		this.IsGodModeEnabled = args[0].Int == 1;
	}

	private void ToggleIcebreaker(CommandArg[] args)
	{
		GameManager.Instance.SaveData.SetBoolVariable(BoatSubModelToggler.ICEBREAKER_EQUIP_STRING_KEY, args[0].Int == 1);
		GameEvents.Instance.TriggerIcebreakerEquipChanged();
	}

	private void SetImmuneMode(CommandArg[] args)
	{
		this.IsImmuneModeEnabled = args[0].Int == 1;
	}

	private void Die(CommandArg[] args)
	{
		this.Die();
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("player.die");
		Terminal.Shell.RemoveCommand("player.upgrade");
		Terminal.Shell.RemoveCommand("player.god");
		Terminal.Shell.RemoveCommand("player.immune");
		Terminal.Shell.RemoveCommand("dest.unlock");
		Terminal.Shell.RemoveCommand("dest.list");
		Terminal.Shell.RemoveCommand("icebreaker");
	}

	[SerializeField]
	private List<BoatModelProxy> _allBoatModelProxies;

	[SerializeField]
	private BoatModelProxy _boatModelProxy;

	[SerializeField]
	private PlayerController _controller;

	[SerializeField]
	private PlayerSanity _sanity;

	[SerializeField]
	private SanityModifierDetector _sanityModifierDetector;

	[SerializeField]
	private DockPOIHandler _docker;

	[SerializeField]
	private Harvester _harvester;

	[SerializeField]
	private HarvestZoneDetector _harvestZoneDetector;

	[SerializeField]
	private PlayerZoneDetector _playerZoneDetector;

	[SerializeField]
	private Transform _colliderCenter;

	[SerializeField]
	private Transform _devilsSpineMonsterAttachPoint;

	[SerializeField]
	private PlayerCollisionAudio _playerCollisionAudio;

	[SerializeField]
	private DepthMonitor _playerDepthMonitor;

	[SerializeField]
	private PlayerEngineAudio playerEngineAudio;

	[SerializeField]
	private PlayerTeleport _playerTeleport;

	[Header("Collisions")]
	[SerializeField]
	private PlayerCollider _collider;

	[Header("Vibrations")]
	[SerializeField]
	private VibrationData _safeCollisionVibration;

	[SerializeField]
	private VibrationData _slowBoatVibration;

	[SerializeField]
	private VibrationData _mediumBoatVibration;

	[SerializeField]
	private VibrationData _fastBoatVibration;

	[SerializeField]
	private VibrationData _slowCollisionVibration;

	[SerializeField]
	private VibrationData _mediumCollisionVibration;

	[SerializeField]
	private VibrationData _fastCollisionVibration;

	[SerializeField]
	private VibrationData _enemyAttackVibration;

	[SerializeField]
	private VibrationData _fishingBlipVibration;

	[SerializeField]
	private VibrationData _fishingSuccessVibration;

	[SerializeField]
	private VibrationData _fishingFailVibration;

	[SerializeField]
	private VibrationData _dredgingContinuousVibration;

	[SerializeField]
	private VibrationData _dredgingSwitchLanesVibration;

	[SerializeField]
	private VibrationData _dredgingSuccessVibration;

	[SerializeField]
	private VibrationData _dredgingFailVibration;

	private bool hasRequestedUndockSave;
}
