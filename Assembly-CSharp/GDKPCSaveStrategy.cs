using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GDKPCSaveStrategy : ISaveStrategy
{
	private ConnectedStorageContainer GetConnectedStorage()
	{
		if (this.connectedStorageInternal != null)
		{
			return this.connectedStorageInternal;
		}
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		this.connectedStorageInternal = ConnectedStorageHandler.CreateConnectedStorage(gdkpcconsoleStrategy.xboxUser, "DredgeContainer", gdkpcconsoleStrategy.providerHandle);
		return this.connectedStorageInternal;
	}

	public void Delete(int slot)
	{
		string filePath = this.GetFilePath(slot);
		if (this.HasFile(filePath))
		{
			this.GetConnectedStorage().DeleteBlob(filePath);
		}
	}

	public bool HasFile(string filePath)
	{
		return this.GetConnectedStorage().DoesBlobExist(filePath);
	}

	public void Init()
	{
	}

	public T GetData<T>(string filePath)
	{
		T t = default(T);
		byte[] array = this.GetConnectedStorage().ReadFromConnectedStorage(filePath);
		if (array == null)
		{
			Debug.LogWarning("Failed to loaded from file path " + filePath + " Returning default");
			return t;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			memoryStream.Write(array, 0, array.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			t = (T)((object)binaryFormatter.Deserialize(memoryStream));
		}
		return t;
	}

	public void OnDestroy()
	{
	}

	public void SaveData<T>(byte[] data, string filePath, bool useBackupHistory)
	{
		this.GetConnectedStorage().WriteToConnectedStorage(filePath, data);
	}

	public string GetFilePath(int slot)
	{
		return string.Format("{0}_post_120_{1}", "DredgeSave", slot);
	}

	public string GetMetaFilePath()
	{
		return "DredgeSaveMeta";
	}

	private const string containerName = "DredgeContainer";

	private const string saveName = "DredgeSave";

	private const string metaName = "DredgeSaveMeta";

	private ConnectedStorageContainer connectedStorageInternal;
}
