using System;

public interface ISaveStrategy
{
	void Init();

	void SaveData<T>(byte[] data, string filePath, bool useBackupHistory);

	bool HasFile(string filePath);

	void Delete(int slot);

	void OnDestroy();

	string GetFilePath(int slot);

	string GetFilePathPre1_2(int slot)
	{
		return this.GetFilePath(slot);
	}

	string GetMetaFilePath();

	T GetData<T>(string filePath);
}
