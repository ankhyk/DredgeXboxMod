using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityEngine;
using XGamingRuntime;
using XGamingRuntime.Interop;

public class GDKPCEntitlementStrategy : IEntitlementStrategy
{
	public bool GetHasEntitlement(EntitlementData entitlementData)
	{
		foreach (XPackageDetails xpackageDetails in this.OwnedEntitlements)
		{
			Debug.Log(string.Concat(new string[] { xpackageDetails.Description, ", ", xpackageDetails.DisplayName, ", ", xpackageDetails.StoreId, ", ", xpackageDetails.PackageIdentifier }));
		}
		Debug.Log("Testing DLC: " + entitlementData.gameCoreStoreID);
		return this.OwnedEntitlements.Find((XPackageDetails x) => x.StoreId == entitlementData.gameCoreStoreID) != null;
	}

	public void OpenEntitlementList()
	{
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		if (gdkpcconsoleStrategy != null)
		{
			XStoreContext xstoreContext;
			if (global::XGamingRuntime.Interop.HR.FAILED(SDK.XStoreCreateContext(gdkpcconsoleStrategy.xboxUser.UserHandle, out xstoreContext)))
			{
				Debug.Log("Failed to create store context");
				return;
			}
			XStoreProductKind xstoreProductKind = XStoreProductKind.Consumable | XStoreProductKind.Durable | XStoreProductKind.Game;
			SDK.XStoreShowAssociatedProductsUIAsync(xstoreContext, "9MSVVM5NS9L6", xstoreProductKind, delegate(int hresult)
			{
				if (global::XGamingRuntime.Interop.HR.FAILED(hresult))
				{
					Debug.LogWarning(string.Format("Failed to open associated store page page for product 0x{0:X8}", hresult));
					return;
				}
				Debug.Log(string.Format("Opened associated store page page for product 0x{0:X8}", hresult));
			});
		}
	}

	public async Task<bool> HasLicense(XPackageDetails details, XStoreContext storeContext)
	{
		bool done = false;
		XStoreLicense outLicense = null;
		SDK.XStoreAcquireLicenseForPackageAsync(storeContext, details.PackageIdentifier, delegate(int hresult, XStoreLicense license)
		{
			outLicense = license;
			if (hresult == 0)
			{
				Debug.Log("Got store license for PackageIdentifier: " + details.PackageIdentifier);
			}
			else
			{
				Debug.LogWarning(string.Format("Failed to get store License for PackageIdentifier: {0} HR:{1}", details.PackageIdentifier, hresult));
			}
			done = true;
		});
		while (!done)
		{
			await Awaiters.NextFrame;
		}
		bool flag = SDK.XStoreIsLicenseValid(outLicense);
		Debug.Log(string.Format("PackageIdentifier:{0} HasLicense={1}", details.PackageIdentifier, flag));
		return flag;
	}

	public async Task Init(List<EntitlementData> entitlements)
	{
		Debug.Log("GDKPCEntitlementStrategy Init");
		this.EntitlementRefs = entitlements;
		this.OwnedEntitlements = new List<XPackageDetails>();
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		this.StoreIDs = new List<string>();
		foreach (EntitlementData entitlementData in entitlements)
		{
			Debug.Log("Setup DLC storeID: " + entitlementData.gameCoreStoreID + " - " + entitlementData.entitlement.ToString());
			this.StoreIDs.Add(entitlementData.gameCoreStoreID);
		}
		XPackageDetails[] array;
		int num = SDK.XPackageEnumeratePackages(XPackageKind.Content, XPackageEnumerationScope.ThisAndRelated, out array);
		if (num == 0 || array != null)
		{
			Debug.Log("Found content from XPackageEnumeratePackages");
			XStoreContext xstoreContext;
			int num2 = SDK.XStoreCreateContext(gdkpcconsoleStrategy.xboxUser.UserHandle, out xstoreContext);
			if (num2 != 0)
			{
				Debug.LogWarning(string.Format("Failed to create store context HR:{0}", num2));
				return;
			}
			Debug.Log(string.Format("Got List of DLCS: Count={0}", array.Length));
			foreach (XPackageDetails xpackageDetails in array)
			{
				Debug.Log(string.Concat(new string[] { "Found DLC: ", xpackageDetails.Description, ", ", xpackageDetails.DisplayName, ", ", xpackageDetails.StoreId, ", ", xpackageDetails.PackageIdentifier }));
				this.OwnedEntitlements.Add(xpackageDetails);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Failed XPackageEnumeratePackages HR:{0} XPackageDetails DLC IsValid: {1}", num, array != null));
		}
		Debug.Log("GDKPCEntitlementStrategy Init completed.");
		Debug.Log(string.Format("{0} DLCs owned", this.OwnedEntitlements.Count));
		Debug.Log("GDKPCEntitlementStrategy Init - Start XPackageRegisterPackageInstalled");
		XRegistrationToken xregistrationToken;
		SDK.XPackageRegisterPackageInstalled(new XPackageInstalledCallback(this.OnInstallDLC), out xregistrationToken);
		Debug.Log("GDKPCEntitlementStrategy Init - End XPackageRegisterPackageInstalled");
	}

	public void OnInstallDLC(XPackageDetails details)
	{
		Debug.Log("Callback event - DLC packaged installed StoreID:" + details.StoreId);
		if (!this.StoreIDs.Contains(details.StoreId))
		{
			Debug.Log("Installed packaged is not on of this games DLCS");
			return;
		}
		if (this.OwnedEntitlements.Find((XPackageDetails x) => x.StoreId == details.StoreId) == null)
		{
			this.OwnedEntitlements.Add(details);
		}
		EntitlementData entitlementData = this.EntitlementRefs.Find((EntitlementData x) => x.gameCoreStoreID == details.StoreId);
		if (entitlementData == null)
		{
			Debug.LogWarning("Failed to find entitlement for store ID " + details.StoreId);
			return;
		}
		GameManager.Instance.EntitlementManager.OnDLCRuntimeInstall.Invoke(entitlementData.entitlement);
	}

	private List<XPackageDetails> OwnedEntitlements;

	private List<string> StoreIDs;

	private List<EntitlementData> EntitlementRefs;
}
