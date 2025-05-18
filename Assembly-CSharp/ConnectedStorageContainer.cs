using System;
using UnityEngine;
using XGamingRuntime;

public class ConnectedStorageContainer
{
	~ConnectedStorageContainer()
	{
		SDK.XGameSaveCloseProvider(this.ProviderHandle);
		SDK.XGameSaveCloseContainer(this.ContainerHandle);
	}

	public XGameSaveContainerInfo GetContainerInfo()
	{
		XGameSaveContainerInfo xgameSaveContainerInfo;
		SDK.XGameSaveGetContainerInfo(this.ProviderHandle, this.ContainerName, out xgameSaveContainerInfo);
		return xgameSaveContainerInfo;
	}

	public void PrintContainerInfo(XGameSaveContainerInfo print)
	{
		try
		{
			Debug.Log(string.Format("BlobCount:{0}", print.BlobCount));
			Debug.Log("DisplayName:" + print.DisplayName);
			Debug.Log(string.Format("TotalSize:{0}", print.TotalSize));
			Debug.Log("Name:" + print.Name);
			Debug.Log("LastModifiedTime:" + print.LastModifiedTime.ToString("MM/dd/yyyy hh:mm tt"));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to print XGameSaveContainerInfo " + ex.Message);
		}
	}

	public void WriteToConnectedStorage(string BlobName, byte[] dataByteArray)
	{
		Debug.Log("XGameSaveCreateUpdate");
		XGameSaveUpdateHandle xgameSaveUpdateHandle = null;
		if (SDK.XGameSaveCreateUpdate(this.ContainerHandle, this.ContainerName, out xgameSaveUpdateHandle) != 0)
		{
			Debug.Log("Failed to create XGameSaveCreateUpdate");
		}
		Debug.Log("XGameSaveSubmitBlobWrite");
		if (SDK.XGameSaveSubmitBlobWrite(xgameSaveUpdateHandle, BlobName, dataByteArray) != 0)
		{
			Debug.Log("Failed to XGameSaveSubmitBlobWrite");
		}
		Debug.Log("XGameSaveSubmitUpdateAsync");
		if (SDK.XGameSaveSubmitUpdate(xgameSaveUpdateHandle) == 0)
		{
			Debug.Log(string.Concat(new string[] { "Blob Data saved to container (", BlobName, " from ", this.ContainerName, ")" }));
		}
		else
		{
			Debug.Log(string.Concat(new string[] { "Failed to save blob data to container (", BlobName, " from ", this.ContainerName, ")" }));
		}
		SDK.XGameSaveCloseUpdateHandle(xgameSaveUpdateHandle);
		Debug.Log("Finished BlobSave");
	}

	public byte[] ReadFromConnectedStorage(string BlobName)
	{
		XGameSaveBlob xgameSaveBlob = null;
		XGameSaveBlobInfo[] array;
		int num = SDK.XGameSaveEnumerateBlobInfo(this.ContainerHandle, out array);
		if (num != 0)
		{
			Debug.LogWarning("Failed to EnumerateBlobInfo");
		}
		XGameSaveBlobInfo xgameSaveBlobInfo = null;
		foreach (XGameSaveBlobInfo xgameSaveBlobInfo2 in array)
		{
			if (xgameSaveBlobInfo2.Name == BlobName)
			{
				xgameSaveBlobInfo = xgameSaveBlobInfo2;
			}
		}
		if (xgameSaveBlobInfo == null)
		{
			Debug.LogWarning(string.Concat(new string[] { "Failed to find blob (", BlobName, " in ", this.ContainerName, ")" }));
			return null;
		}
		XGameSaveBlob[] array3;
		num = SDK.XGameSaveReadBlobData(this.ContainerHandle, new XGameSaveBlobInfo[] { xgameSaveBlobInfo }, out array3);
		if (num == 0)
		{
			Debug.LogWarning(string.Format("Blob Data retrieved from container ({0} from {1}) (blob count = {2})", BlobName, this.ContainerName, array3.Length));
			if (array3.Length != 0)
			{
				xgameSaveBlob = array3[0];
			}
			else
			{
				Debug.LogWarning("0 blobs found");
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Failed to get blob data from container ({0} from {1} HR:{2})", BlobName, this.ContainerName, num));
		}
		if (xgameSaveBlob == null || xgameSaveBlob.Data == null)
		{
			return null;
		}
		return xgameSaveBlob.Data;
	}

	public void DeleteBlob(string BlobName)
	{
		Debug.Log("XGameSaveCreateUpdate");
		XGameSaveUpdateHandle xgameSaveUpdateHandle;
		int num = SDK.XGameSaveCreateUpdate(this.ContainerHandle, this.ContainerName, out xgameSaveUpdateHandle);
		if (num != 0)
		{
			Debug.Log("Failed to create delete update handle");
		}
		num = SDK.XGameSaveSubmitBlobDelete(xgameSaveUpdateHandle, BlobName);
		if (num == 0)
		{
			Debug.Log("Submit Deleted blob " + BlobName);
		}
		else
		{
			Debug.Log(string.Format("Failed submit to delete blob {0} - Error: {1}", BlobName, num));
		}
		num = SDK.XGameSaveSubmitUpdate(xgameSaveUpdateHandle);
		if (num == 0)
		{
			Debug.Log("Deleted blob " + BlobName);
		}
		else
		{
			Debug.Log(string.Format("Failed to delete blob {0} - Error: {1}", BlobName, num));
		}
		SDK.XGameSaveCloseUpdateHandle(xgameSaveUpdateHandle);
		Debug.Log("Finished Blob delete");
	}

	public bool DoesBlobExist(string BlobName)
	{
		XGameSaveBlobInfo[] array;
		if (SDK.XGameSaveEnumerateBlobInfo(this.ContainerHandle, out array) != 0)
		{
			Debug.LogWarning("Failed to EnumerateBlobInfo");
			return false;
		}
		XGameSaveBlobInfo[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i].Name == BlobName)
			{
				return true;
			}
		}
		return false;
	}

	public string ContainerName = "";

	public XGameSaveContainerHandle ContainerHandle;

	public XGameSaveProviderHandle ProviderHandle;

	public GDCPCUser ForUser;
}
