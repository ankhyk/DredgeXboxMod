using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandTerminal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class EntitlementManager : SerializedMonoBehaviour
{
	private void Awake()
	{
		this.entitlementBypasses = new bool[4];
		GameManager.Instance.EntitlementManager = this;
	}

	public async Task Init()
	{
		this.entitlementStrategy = new GDKPCEntitlementStrategy();
		await this.entitlementStrategy.Init(this.entitlements.Values.ToList<EntitlementData>());
	}

	private void OnEnable()
	{
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
	}

	public void OpenEntitlementStoreList()
	{
		this.entitlementStrategy.OpenEntitlementList();
	}

	public bool GetHasEntitlement(Entitlement entitlement)
	{
		EntitlementData entitlementData = null;
		this.entitlements.TryGetValue(entitlement, out entitlementData);
		bool flag = false;
		if (entitlementData != null)
		{
			flag = this.entitlementStrategy.GetHasEntitlement(entitlementData);
		}
		return flag;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("dlc.own", new Action<CommandArg[]>(this.SetEntitlementBypass), 2, 2, "Toggles the ownership of the specified dlc. dlc.own <dlc_id> <own_state>. Preorder = 0. Deluxe = 1.");
		Terminal.Shell.AddCommand("dlc.test", new Action<CommandArg[]>(this.TestDLCOwned), 0, 0, "Prints which DLCs are owned");
		Terminal.Shell.AddCommand("dlc.callback", new Action<CommandArg[]>(this.CallbackTest), 0, 0, "Test runtime callback for installed entitlements");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("dlc.own");
		Terminal.Shell.RemoveCommand("dlc.test");
		Terminal.Shell.RemoveCommand("dlc.callback");
	}

	private void CallbackTest(CommandArg[] args)
	{
		this.entitlementBypasses[2] = true;
		this.OnDLCRuntimeInstall.Invoke(Entitlement.DLC_1);
	}

	private void TestDLCOwned(CommandArg[] args)
	{
		this.entitlements.Keys.ToList<Entitlement>().ForEach(delegate(Entitlement e)
		{
		});
	}

	private void SetEntitlementBypass(CommandArg[] args)
	{
		int @int = args[0].Int;
		bool flag = args[1].Int == 1;
		this.entitlementBypasses[@int] = flag;
		UnityEvent<Entitlement> onDLCRuntimeInstall = GameManager.Instance.EntitlementManager.OnDLCRuntimeInstall;
		if (onDLCRuntimeInstall == null)
		{
			return;
		}
		onDLCRuntimeInstall.Invoke(@int + Entitlement.PREORDER);
	}

	public void GrantEntitlementBypass(Entitlement entitlement)
	{
		this.entitlementBypasses[this.entitlements[entitlement].bypassId] = true;
	}

	[SerializeField]
	private Dictionary<Entitlement, EntitlementData> entitlements = new Dictionary<Entitlement, EntitlementData>();

	public IEntitlementStrategy entitlementStrategy;

	private bool[] entitlementBypasses;

	public UnityEvent<Entitlement> OnDLCRuntimeInstall;
}
