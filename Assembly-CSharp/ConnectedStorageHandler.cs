using System;
using UnityEngine;
using XGamingRuntime;

public class ConnectedStorageHandler
{
	public static ConnectedStorageContainer CreateConnectedStorage(GDCPCUser user, string containerName, XGameSaveProviderHandle providerHandle)
	{
		Debug.Log("Starting Creating Connected Storage");
		ConnectedStorageContainer connectedStorageContainer = new ConnectedStorageContainer();
		Debug.Log("Starting XGameSaveCreateContainer");
		XGameSaveContainerHandle xgameSaveContainerHandle;
		int num = SDK.XGameSaveCreateContainer(providerHandle, containerName, out xgameSaveContainerHandle);
		if (num == 0)
		{
			connectedStorageContainer.ContainerHandle = xgameSaveContainerHandle;
			connectedStorageContainer.ProviderHandle = providerHandle;
			connectedStorageContainer.ContainerName = containerName;
			connectedStorageContainer.ForUser = user;
			Debug.Log("Got XGameSaveCreateContainer");
		}
		else
		{
			Debug.LogWarning(string.Format("Failed to get XGameSaveCreateContainer Error Code {0}", num));
		}
		return connectedStorageContainer;
	}
}
