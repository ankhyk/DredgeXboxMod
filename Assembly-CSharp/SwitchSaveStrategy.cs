using System;

public class SwitchSaveStrategy : ISaveStrategy
{
	public void Delete(int slot)
	{
		this.HasFile(this.GetFilePath(slot));
	}

	public bool HasFile(string filePath)
	{
		return false;
	}

	public void Init()
	{
	}

	public T GetData<T>(string filePath)
	{
		return default(T);
	}

	public void OnDestroy()
	{
	}

	public void SaveData<T>(byte[] data, string filePath, bool useBackupHistory)
	{
	}

	public string GetFilePath(int slot)
	{
		return string.Format("{0}:/{1}{2}", "DredgeMount", "DredgeSave", slot);
	}

	public string GetMetaFilePath()
	{
		return string.Format("{0}:/{1}", "DredgeMount", "DredgeSettings");
	}

	private const int saveDataSize = 262144;

	private const string mountName = "DredgeMount";

	private const string saveName = "DredgeSave";

	private const string metaName = "DredgeSettings";
}
