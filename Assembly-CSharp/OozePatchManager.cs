using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class OozePatchManager : SerializedMonoBehaviour
{
	public float TotalOozeCollected
	{
		get
		{
			return this.totalOozeCollected;
		}
	}

	private void Awake()
	{
		this.totalOozeCollected = GameManager.Instance.SaveData.GetFloatVariable(OozePatchManager.CURRENT_OOZE_FILL_SAVE_KEY, 0f);
		GameManager.Instance.OozePatchManager = this;
	}

	private void OnDestroy()
	{
		GameManager.Instance.OozePatchManager = null;
	}

	private void OnEnable()
	{
		this.Initialize();
		this.OnTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
		this.RemoveTerminalCommands();
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("ooze.clearall", new Action<CommandArg[]>(this.DebugClearAllOoze), 0, 0, "Completely empties all ooze patches.");
		Terminal.Shell.AddCommand("ooze.list", new Action<CommandArg[]>(this.DebugListAllOoze), 0, 0, "Lists the ids of all ooze patches.");
		Terminal.Shell.AddCommand("ooze.get", new Action<CommandArg[]>(this.DebugGetOoze), 1, 1, "Get ooze patch fullness by ooze patch id.");
		Terminal.Shell.AddCommand("ooze.set", new Action<CommandArg[]>(this.DebugSetOoze), 2, 2, "Get ooze patch fullness by ooze patch id <id> <0-1>");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("ooze.clearall");
		Terminal.Shell.RemoveCommand("ooze.list");
		Terminal.Shell.RemoveCommand("ooze.get");
		Terminal.Shell.RemoveCommand("ooze.set");
	}

	private void DebugListAllOoze(CommandArg[] args)
	{
		this.oozePatches.ForEach(delegate(List<OozePatch> l)
		{
			l.ForEach(delegate(OozePatch p)
			{
			});
		});
	}

	private void DebugGetOoze(CommandArg[] args)
	{
		Action<OozePatch> <>9__1;
		this.oozePatches.ForEach(delegate(List<OozePatch> l)
		{
			Action<OozePatch> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(OozePatch p)
				{
					p.OozePatchId == args[0].String;
				});
			}
			l.ForEach(action);
		});
	}

	private void DebugSetOoze(CommandArg[] args)
	{
		Action<OozePatch> <>9__1;
		this.oozePatches.ForEach(delegate(List<OozePatch> l)
		{
			Action<OozePatch> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(OozePatch p)
				{
					if (p.OozePatchId == args[0].String)
					{
						p.SetProportionFilled(args[1].Float);
						p.InitialiseOoze(true);
					}
				});
			}
			l.ForEach(action);
		});
	}

	private void DebugClearAllOoze(CommandArg[] args)
	{
		this.oozePatches.ForEach(delegate(List<OozePatch> l)
		{
			l.ForEach(delegate(OozePatch p)
			{
				GameManager.Instance.SaveData.SetOozePatchFillAmount(p.OozePatchId, 0f);
				p.SetProportionFilled(0f);
				p.InitialiseOoze(true);
			});
		});
	}

	private void Initialize()
	{
		this.allOozePatches = new List<OozePatch>();
		this.oozePatches.ForEach(delegate(List<OozePatch> l)
		{
			l.ForEach(delegate(OozePatch o)
			{
				this.allOozePatches.Add(o);
				o.gameObject.SetActive(false);
			});
		});
	}

	public List<OozePatch> GetAllActiveOozePatches()
	{
		float minPatchSize = GameManager.Instance.GameConfigData.OozePatchProportionMinimum;
		return this.allOozePatches.Where((OozePatch p) => GameManager.Instance.SaveData.GetOozePatchFillAmount(p.OozePatchId) > minPatchSize && p.gameObject.activeSelf).ToList<OozePatch>();
	}

	public int GetAreaIndexWithOozePresent()
	{
		int tirworldPhase = GameManager.Instance.SaveData.TIRWorldPhase;
		int num = Mathf.Min(4, tirworldPhase - 1);
		int num2 = -1;
		float minPatchSize = GameManager.Instance.GameConfigData.OozePatchProportionMinimum;
		Func<OozePatch, bool> <>9__0;
		for (int i = num; i >= 0; i--)
		{
			IEnumerable<OozePatch> enumerable = this.oozePatches[i];
			Func<OozePatch, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (OozePatch p) => p.GetProportionFilled() > minPatchSize);
			}
			if (enumerable.Any(func))
			{
				num2 = i;
				break;
			}
		}
		return num2;
	}

	public int RepopulateRandomAreaWithOoze()
	{
		int num;
		if (GameManager.Instance.SaveData.TIRWorldPhase >= 6)
		{
			num = global::UnityEngine.Random.Range(0, 5);
		}
		else
		{
			num = GameManager.Instance.SaveData.TIRWorldPhase - 1;
		}
		for (int i = 0; i < 3; i++)
		{
			this.RepopulateRandomDepletedOozePatchByAreaIndex(num);
		}
		return num;
	}

	public void RepopulateRandomDepletedOozePatchByAreaIndex(int areaIndex)
	{
		float minPatchSize = GameManager.Instance.GameConfigData.OozePatchProportionMinimum;
		List<OozePatch> list = this.oozePatches[areaIndex].Where((OozePatch p) => GameManager.Instance.SaveData.GetOozePatchFillAmount(p.OozePatchId) < minPatchSize).ToList<OozePatch>();
		if (list.Count == 0)
		{
			return;
		}
		OozePatch oozePatch = list.PickRandom<OozePatch>();
		GameManager.Instance.SaveData.SetOozePatchFillAmount(oozePatch.OozePatchId, 1f);
		oozePatch.SetProportionFilled(1f);
		oozePatch.InitialiseOoze(true);
	}

	public bool GetIsCurrentlyGatheringOoze()
	{
		return Time.time < this.timeOfLastOozeGathering + this.oozeGatheringCooldownTimeSec;
	}

	private void OnTIRWorldPhaseChanged(int tirWorldPhase)
	{
		int num = 0;
		while (num < tirWorldPhase && num < this.oozePatches.Count)
		{
			this.oozePatches[num].ForEach(delegate(OozePatch o)
			{
				o.SetProportionFilled(GameManager.Instance.SaveData.GetOozePatchFillAmount(o.OozePatchId));
				o.InitialiseOoze(false);
				o.gameObject.SetActive(true);
			});
			num++;
		}
		for (int i = tirWorldPhase; i < this.oozePatches.Count; i++)
		{
			this.oozePatches[i].ForEach(delegate(OozePatch o)
			{
				o.gameObject.SetActive(false);
			});
		}
	}

	public bool IsPositionInOoze(Vector3 worldPosition)
	{
		for (int i = 0; i < this.allOozePatches.Count; i++)
		{
			if (this.allOozePatches[i].OozeAmountAtPosition(worldPosition) > 0.5f)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionInOozePatchBounds(Vector3 worldPosition)
	{
		for (int i = 0; i < this.allOozePatches.Count; i++)
		{
			if (this.allOozePatches[i].PositionIsWithinBounds(worldPosition))
			{
				return true;
			}
		}
		return false;
	}

	public float SampleOozeAtPosition(Vector3 worldPosition)
	{
		float num = 0f;
		for (int i = 0; i < this.allOozePatches.Count; i++)
		{
			num += this.allOozePatches[i].OozeAmountAtPosition(worldPosition);
		}
		return num;
	}

	public bool TryGetOozePatchAtPosition(Vector3 worldPosition, out OozePatch oozeOut)
	{
		for (int i = 0; i < this.allOozePatches.Count; i++)
		{
			if (this.allOozePatches[i].PositionIsWithinBounds(worldPosition))
			{
				oozeOut = this.allOozePatches[i];
				return true;
			}
		}
		oozeOut = null;
		return false;
	}

	public void NotifyOozeGathered(float amount)
	{
		amount *= GameManager.Instance.GameConfigData.OozeCollectionCoefficient;
		if (amount > 0f)
		{
			this.timeOfLastOozeGathering = Time.time;
			this.totalOozeCollected += amount;
			if (this.totalOozeCollected > 1f)
			{
				this.totalOozeCollected = 0f;
				this.TryAddOozeCanister();
			}
			GameManager.Instance.SaveData.SetFloatVariable(OozePatchManager.CURRENT_OOZE_FILL_SAVE_KEY, this.totalOozeCollected);
		}
	}

	private bool TryAddOozeCanister()
	{
		Vector3Int vector3Int = default(Vector3Int);
		bool flag = GameManager.Instance.SaveData.TrawlNet.FindPositionForObject(this.oozeCanisterItemData, out vector3Int, 0, false);
		if (flag)
		{
			SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(this.oozeCanisterItemData);
			GameManager.Instance.SaveData.TrawlNet.AddObjectToGridData(spatialItemInstance, vector3Int, true, null);
			GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance);
			GameManager.Instance.SaveData.AdjustIntVariable("canisters-caught", 1);
			GameManager.Instance.AchievementManager.EvaluateAchievement(DredgeAchievementId.DLC_4_7);
			GameManager.Instance.AudioPlayer.PlaySFX(this.oozeCatchSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
		}
		return flag;
	}

	private static string CURRENT_OOZE_FILL_SAVE_KEY = "ooze-net-fill-amount";

	[SerializeField]
	private SpatialItemData oozeCanisterItemData;

	private const float oozeCheckSensitivity = 0.5f;

	[SerializeField]
	private List<List<OozePatch>> oozePatches = new List<List<OozePatch>>();

	[SerializeField]
	private AssetReference oozeCatchSFX;

	private List<OozePatch> allOozePatches;

	private float timeOfLastOozeGathering;

	private float oozeGatheringCooldownTimeSec = 1f;

	public bool isOozeNearToPlayer;

	public bool isOozeFarToPlayer;

	private float totalOozeCollected;
}
