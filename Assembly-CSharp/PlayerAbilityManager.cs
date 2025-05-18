using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-950)]
public class PlayerAbilityManager : SerializedMonoBehaviour
{
	public AbilityData CurrentlySelectedAbilityData
	{
		get
		{
			return this.currentlySelectedAbilityData;
		}
	}

	private void Awake()
	{
		GameManager.Instance.PlayerAbilities = this;
		this.AddTerminalCommands();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilitySelected += this.OnPlayerAbilitySelected;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilitySelected -= this.OnPlayerAbilitySelected;
		GameManager.Instance.PlayerAbilities = null;
		this.RemoveTerminalCommands();
	}

	private void OnPlayerAbilitySelected(AbilityData abilityData)
	{
		this.currentlySelectedAbilityData = abilityData;
	}

	public AbilityData GetAbilityDataByName(string abilityName)
	{
		return this.abilityDatas.FirstOrDefault((AbilityData d) => d.name == abilityName);
	}

	public void RegisterAbility(AbilityData abilityData, Ability ability)
	{
		if (this.abilityMap.ContainsKey(abilityData.name.ToLower()))
		{
			return;
		}
		this.abilityMap.Add(abilityData.name.ToLower(), ability);
		GameEvents.Instance.TriggerPlayerAbilityRegistered(abilityData);
	}

	public bool ActivateAbility(AbilityData abilityData)
	{
		Ability ability;
		this.abilityMap.TryGetValue(abilityData.name.ToLower(), out ability);
		return ability != null && ability.Activate();
	}

	public void DeactivateAbility(AbilityData abilityData)
	{
		Ability ability;
		this.abilityMap.TryGetValue(abilityData.name.ToLower(), out ability);
		if (ability != null)
		{
			ability.Deactivate();
		}
	}

	public Ability GetAbilityForData(AbilityData abilityData)
	{
		Ability ability = null;
		this.abilityMap.TryGetValue(abilityData.name.ToLower(), out ability);
		return ability;
	}

	public bool GetIsAbilityActive(AbilityData ability)
	{
		if (ability == null)
		{
			return false;
		}
		string text = ability.name.ToLower();
		return this.abilityMap.ContainsKey(text) && this.abilityMap[text].IsActive;
	}

	public bool GetIsAbilityUnlocked(AbilityData ability)
	{
		return !(ability == null) && GameManager.Instance.SaveData.unlockedAbilities.Contains(ability.name.ToLower());
	}

	public bool GetHasDependantItems(AbilityData ability)
	{
		if (ability == null)
		{
			return false;
		}
		if (ability.linkedItems != null && ability.linkedItems.Length != 0)
		{
			return GameManager.Instance.SaveData.HasAnyOfTheseItemsInInventory(ability.linkedItems.Select((ItemData i) => i.id).ToArray<string>(), true);
		}
		return ability.linkedItemSubtype == ItemSubtype.NONE || GameManager.Instance.SaveData.HasItemsOfSubtypeInInventory(ability.linkedItemSubtype, ability.allowDamagedItems, ability.allowExhaustedItems);
	}

	public float GetTimeSinceLastCast(AbilityData abilityData)
	{
		float num = 0f;
		if (!GameManager.Instance.SaveData.abilityHistory.TryGetValue(abilityData.name.ToLowerInvariant(), out num))
		{
			num = float.NegativeInfinity;
		}
		return GameManager.Instance.Time.TimeAndDay - num;
	}

	public void UnlockAbility(string abilityName)
	{
		AbilityData abilityDataByName = this.GetAbilityDataByName(abilityName);
		if (abilityDataByName)
		{
			this.UnlockAbility(abilityDataByName);
		}
	}

	public void UnlockAbility(AbilityData abilityData)
	{
		if (abilityData && !GameManager.Instance.SaveData.unlockedAbilities.Contains(abilityData.name))
		{
			GameManager.Instance.SaveData.unlockedAbilities.Add(abilityData.name);
			if (abilityData.showUnseenNotification)
			{
				GameManager.Instance.SaveData.SetAbilityUnseen(abilityData, true);
			}
			GameEvents.Instance.TogglePlayerAbilitiesChanged(abilityData);
		}
	}

	public bool GetHasAnyUnseenAbilities()
	{
		return this.abilityDatas.Any((AbilityData abilityData) => (abilityData.showUnseenNotification && GameManager.Instance.SaveData.GetAbilityUnseen(abilityData)) || (abilityData.linkedAdvancedVersion != null && abilityData.linkedAdvancedVersion.showUnseenNotification && GameManager.Instance.SaveData.GetAbilityUnseen(abilityData.linkedAdvancedVersion)));
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("ability.list", new Action<CommandArg[]>(this.DebugListAbilities), 0, 0, "Lists all abilities.");
			Terminal.Shell.AddCommand("ability.add", new Action<CommandArg[]>(this.DebugUnlockAbility), 1, 1, "Unlocks an ability e.g. 'ability.add ability1'");
			Terminal.Shell.AddCommand("ability.remove", new Action<CommandArg[]>(this.DebugLockAbility), 1, 1, "Locks an ability e.g. 'ability.remove ability1'");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("ability.list");
			Terminal.Shell.RemoveCommand("ability.add");
			Terminal.Shell.RemoveCommand("ability.remove");
		}
	}

	private void DebugListAbilities(CommandArg[] args)
	{
		string abilities = "";
		this.abilityMap.Keys.ToList<string>().ForEach(delegate(string i)
		{
			abilities = abilities + i + ", ";
		});
	}

	private void DebugUnlockAbility(CommandArg[] args)
	{
		this.UnlockAbility(args[0].String.ToLower());
	}

	private void DebugLockAbility(CommandArg[] args)
	{
		GameManager.Instance.SaveData.unlockedAbilities.Remove(args[0].String.ToLower());
		GameEvents.Instance.TogglePlayerAbilitiesChanged(null);
	}

	[SerializeField]
	private List<AbilityData> abilityDatas = new List<AbilityData>();

	private Dictionary<string, Ability> abilityMap = new Dictionary<string, Ability>();

	private AbilityData currentlySelectedAbilityData;
}
