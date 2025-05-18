using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldStateChangerConfig : SerializedMonoBehaviour
{
	public bool Done
	{
		get
		{
			return this.done;
		}
	}

	public void CheckPersistence()
	{
		if (this.persists)
		{
			this.done = GameManager.Instance.SaveData.GetBoolVariable(this.persistenceBoolKey, false);
			if (this.done)
			{
				this.Do();
			}
		}
	}

	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(WorldStateAction));
	}

	public bool EvaluateCondition()
	{
		if (GameManager.Instance.Player == null)
		{
			return false;
		}
		if (GameManager.Instance.Player.CurrentDock && !this.triggerOnDock)
		{
			return false;
		}
		if (GameManager.Instance.Player.CurrentDock == null && !this.triggerOnUndock)
		{
			return false;
		}
		bool flag = true;
		if (this.dialogueConditionMode == ConditionMode.ALL)
		{
			flag = this.visitedTheseDialogueNodes.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
		}
		else if (this.dialogueConditionMode == ConditionMode.ANY)
		{
			flag = this.visitedTheseDialogueNodes.Any((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
		}
		if (this.unvisitedDialogueConditionMode == ConditionMode.ALL)
		{
			bool flag2;
			if (flag)
			{
				flag2 = this.unvisitedTheseDialogueNodes.All((string s) => !GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		else if (this.unvisitedDialogueConditionMode == ConditionMode.ANY)
		{
			bool flag3;
			if (flag)
			{
				flag3 = !this.unvisitedTheseDialogueNodes.Any((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
			}
			else
			{
				flag3 = false;
			}
			flag = flag3;
		}
		bool flag4 = true;
		if (this.zoneConditionMode == ConditionMode.WHITELIST)
		{
			flag4 = this.zones.Contains(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone());
		}
		else if (this.zoneConditionMode == ConditionMode.BLACKLIST)
		{
			flag4 = !this.zones.Contains(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone());
		}
		bool flag5 = true;
		DockData dockData = null;
		if (GameManager.Instance.Player.CurrentDock != null)
		{
			dockData = GameManager.Instance.Player.CurrentDock.Data;
		}
		else if (GameManager.Instance.Player.PreviousDock.Data != null)
		{
			dockData = GameManager.Instance.Player.PreviousDock.Data;
		}
		if (dockData != null)
		{
			if (this.dockConditionMode == ConditionMode.WHITELIST)
			{
				flag5 = this.docks.Contains(dockData);
			}
			else if (this.dockConditionMode == ConditionMode.BLACKLIST)
			{
				flag5 = !this.docks.Contains(dockData);
			}
		}
		return flag && flag4 && flag5;
	}

	public void Do()
	{
		this.actions.ForEach(delegate(WorldStateAction a)
		{
			a.Do();
		});
		this.done = true;
		if (this.persists)
		{
			GameManager.Instance.SaveData.SetBoolVariable(this.persistenceBoolKey, true);
		}
	}

	[SerializeField]
	private bool done;

	[SerializeField]
	private bool persists;

	[SerializeField]
	private string persistenceBoolKey;

	[SerializeField]
	private bool triggerOnDock = true;

	[SerializeField]
	private bool triggerOnUndock = true;

	[SerializeField]
	private List<WorldStateAction> actions;

	[Header("Dialogue Conditions")]
	[SerializeField]
	private List<string> visitedTheseDialogueNodes;

	[SerializeField]
	private ConditionMode dialogueConditionMode;

	[SerializeField]
	private List<string> unvisitedTheseDialogueNodes;

	[SerializeField]
	private ConditionMode unvisitedDialogueConditionMode;

	[Header("Zone Conditions")]
	[SerializeField]
	private List<ZoneEnum> zones;

	[SerializeField]
	private ConditionMode zoneConditionMode;

	[Header("Dock Conditions")]
	[SerializeField]
	private List<DockData> docks;

	[SerializeField]
	private ConditionMode dockConditionMode;
}
