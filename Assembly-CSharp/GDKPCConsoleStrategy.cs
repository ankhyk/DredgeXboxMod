using System;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityEngine;
using XGamingRuntime;
using XGamingRuntime.Interop;

public class GDKPCConsoleStrategy : IConsoleManagementStrategy
{
	public Task InitConsole()
	{
		Debug.Log("Starting Init GameCore");
		int num = SDK.XGameRuntimeInitialize();
		if (num == 0)
		{
			this.RunXTaskQueue = true;
			num = SDK.XBL.XblInitialize(GDKPCConsoleStrategy.SCID);
			SDK.XUserRegisterForChangeEvent(new XUserChangeEventCallback(this.OnSystemUserChange), out this.userRegisterForChangeEventToken);
		}
		if (num != 0)
		{
			Debug.LogError("Error initialising the gaming runtime, hresult: " + num.ToString());
		}
		return Task.CompletedTask;
	}

	public async Task LoginUser()
	{
		await Awaiters.NextFrame;
		await Awaiters.MainThread;
		Debug.Log("Starting Login User GameCore");
		do
		{
			this.xboxUser = await GDKPCConsoleStrategy.LoginXboxUser();
		}
		while (this.xboxUser == null);
		Debug.Log("Starting XUserGetId");
		ulong num2;
		int num = SDK.XUserGetId(this.xboxUser.UserHandle, out num2);
		if (num == 0)
		{
			this.xboxUser.UserXUID = num2;
			Debug.Log(string.Format("Got UserID - {0}", num2));
		}
		else
		{
			Debug.LogError(string.Format("XUserGetID Failed: {0}", num));
		}
		Debug.Log("Started XblContextCreateHandle");
		if (SDK.XBL.XblContextCreateHandle(this.xboxUser.UserHandle, out this.xboxUser.XboxLiveContext) == 0)
		{
			Debug.Log(string.Format("Got XblContextCreateHandle. Handle = {0}", this.xboxUser.XboxLiveContext));
		}
		else
		{
			Debug.LogError("Failed to get XblContextCreateHandle");
			this.xboxUser.XboxLiveContext = null;
		}
		if (SDK.XUserGetGamertag(this.xboxUser.UserHandle, XUserGamertagComponent.Classic, out this.xboxUser.UserGamertag) == 0)
		{
			Debug.Log("Got Gamer Tag. Tag=" + this.xboxUser.UserGamertag);
		}
		else
		{
			Debug.LogError("No Gamer Tag");
		}
		Debug.LogError("Start SetupSave");
		await this.SetupSave();
	}

	public async Task SetupSave()
	{
		Debug.LogError("Start SetupSave 1");
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		int hr = 0;
		bool Done = false;
		SDK.XGameSaveInitializeProviderAsync(gdkpcconsoleStrategy.xboxUser.UserHandle, GDKPCConsoleStrategy.SCID, false, delegate(int hresult, XGameSaveProviderHandle gameSaveProviderHandle)
		{
			Debug.LogError("Start SetupSave Done");
			hr = hresult;
			this.providerHandle = gameSaveProviderHandle;
			Done = true;
		});
		while (!Done)
		{
			await Awaiters.NextFrame;
		}
		Debug.LogError("Start SetupSave Done After");
		if (hr == 0)
		{
			Debug.Log("XGameSaveInitializeProvider with result 0. Success");
		}
		else
		{
			if (-2147024809 == hr)
			{
				Debug.LogError("XGameSaveInitializeProvider invalid Args");
			}
			SDK.XGameUiShowErrorDialogAsync(hr, "XGameSaveInitializeProvider", delegate(int hresult)
			{
			});
			Debug.LogError(string.Format("XGameSaveInitializeProvider Error with result {0}. Fail", hr));
		}
	}

	public void OpenStore(StoreSKUData data)
	{
		XStoreContext xstoreContext;
		if (global::XGamingRuntime.Interop.HR.FAILED(SDK.XStoreCreateContext(this.xboxUser.UserHandle, out xstoreContext)))
		{
			Debug.Log("Failed to create store context");
			return;
		}
		SDK.XStoreShowProductPageUIAsync(xstoreContext, data.xboxStoreID, delegate(int hresult)
		{
			if (global::XGamingRuntime.Interop.HR.FAILED(hresult))
			{
				Debug.LogWarning(string.Format("Failed to open Store page for product {0}: 0x{1:X8}", data.xboxStoreID, hresult));
				return;
			}
			Debug.Log("Store page opened for product " + data.xboxStoreID);
		});
	}

	public async Task<bool> SwitchUser()
	{
		ulong currentUID = this.xboxUser.UserXUID;
		await this.LoginUser();
		return currentUID != this.xboxUser.UserXUID;
	}

	public string GetUserName()
	{
		return this.xboxUser.UserGamertag;
	}

	public void UpdateConsole()
	{
		if (!this.RunXTaskQueue)
		{
			return;
		}
		SDK.XTaskQueueDispatch(0U);
	}

	public static async Task<GDCPCUser> LoginXboxUser()
	{
		bool completed = false;
		bool gotUser = false;
		GDCPCUser user = new GDCPCUser();
		Debug.Log("Started Account Picker");
		SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserAllowingUI, delegate(int hresult, global::XGamingRuntime.XUserHandle userHandle)
		{
			Debug.Log("XUser Add Callback");
			if (hresult == 0 && userHandle != null)
			{
				Debug.Log("SDK.XUserAddAsync Completed");
				user.UserHandle = userHandle;
				gotUser = true;
			}
			else
			{
				string text = ((userHandle == null) ? "Null" : string.Format("{0}", userHandle));
				Debug.Log(string.Format("Failed to add User Error Code:{0} userHandle:{1}", hresult, text));
			}
			completed = true;
		});
		while (!completed)
		{
			await Awaiters.NextFrame;
		}
		GDCPCUser gdcpcuser;
		if (gotUser)
		{
			ulong num;
			if (SDK.XUserGetId(user.UserHandle, out num) == 0)
			{
				Debug.Log(string.Format("Got UserID for Test - {0}", num));
				gdcpcuser = user;
			}
			else
			{
				Debug.LogError("Failed to get UserID");
				gdcpcuser = user;
			}
		}
		else
		{
			gdcpcuser = null;
		}
		return gdcpcuser;
	}

	private void OnSystemUserChange(XUserLocalId userLocalId, XUserChangeEvent eventType)
	{
		Debug.Log("On OnSystemUserChange");
		if (eventType == XUserChangeEvent.SignedOut || eventType == XUserChangeEvent.SigningOut)
		{
			Debug.Log(string.Format("User SignedOut or SigningOut out with localID = {0} EventType= {1}", userLocalId.value, eventType));
			ConsoleManager.OnActiveUserSignedOut(userLocalId.value.ToString());
		}
	}

	public GDCPCUser xboxUser;

	private bool RunXTaskQueue;

	private XRegistrationToken userRegisterForChangeEventToken;

	public static string SCID = "00000000-0000-0000-0000-000071b6a9d4";

	public XGameSaveProviderHandle providerHandle;
}
