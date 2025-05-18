using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class WindowsSaveStrategy : ISaveStrategy
{
	public void Init()
	{
		WindowsSaveStrategy.dataPath = Application.persistentDataPath + "/" + WindowsSaveStrategy.saveFolder + "/";
		WindowsSaveStrategy.metaDataPath = Application.persistentDataPath + "/";
		if (!Directory.Exists(WindowsSaveStrategy.dataPath))
		{
			Directory.CreateDirectory(WindowsSaveStrategy.dataPath);
		}
		string text = WindowsSaveStrategy.dataPath.Replace(WindowsSaveStrategy.saveFolder, WindowsSaveStrategy.backupSavesFolder);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
	}

	public void Delete(int slot)
	{
		string filePath = this.GetFilePath(slot);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}

	public bool HasFile(string filePath)
	{
		return File.Exists(filePath);
	}

	public T GetData<T>(string filePath)
	{
		T t = default(T);
		FileStream fileStream = null;
		try
		{
			if (File.Exists(filePath))
			{
				fileStream = File.Open(filePath, FileMode.Open);
				t = (T)((object)new BinaryFormatter().Deserialize(fileStream));
				if (GameManager.Instance.SaveManager.forceLoadBadData && GameManager.Instance.SaveManager.isLoadingForReal)
				{
					t = default(T);
					throw new Exception("This is a fake exception because the forceLoadBadData flag is set.");
				}
			}
		}
		catch (Exception ex)
		{
			CustomDebug.EditorLogError(ex.ToString());
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}
		return t;
	}

	private T Deserialize<T>(byte[] param)
	{
		T t;
		using (MemoryStream memoryStream = new MemoryStream(param))
		{
			t = (T)((object)((IFormatter)new BinaryFormatter()).Deserialize(memoryStream));
		}
		return t;
	}

	public void OnDestroy()
	{
	}

	public void SaveData<T>(byte[] data, string filePath, bool useBackupHistory)
	{
		FileStream fileStream = null;
		if (GameManager.Instance.SaveManager.forceSaveBadData)
		{
			data = new byte[1];
		}
		try
		{
			if (this.Deserialize<T>(data) == null)
			{
				throw new Exception("[WindowsSaveStrategy] SaveData(" + filePath + ") failed to deserialize object into origin type. Will not write this data.");
			}
			string text = filePath + ".bak";
			fileStream = File.Open(text, FileMode.Create);
			fileStream.Write(data, 0, data.Length);
			fileStream.Flush();
			if (GameManager.Instance.SaveManager.forceCrashDuringSave)
			{
				throw new Exception("[WindowsSaveStrategy] SaveData(" + filePath + ") Forcing a save exception.");
			}
			fileStream.Close();
			T data2 = this.GetData<T>(text);
			if (EqualityComparer<T>.Default.Equals(data2, default(T)))
			{
				throw new Exception("[WindowsSaveStrategy] SaveData(" + filePath + ") could not re-read written data. Save must have written bad data.");
			}
			this.FileStreamCopy(text, filePath);
			float time = Time.time;
			if (useBackupHistory)
			{
				string text2 = text.Replace(WindowsSaveStrategy.saveFolder, WindowsSaveStrategy.backupSavesFolder);
				string text4;
				for (int i = 50 - 1; i >= 0; i--)
				{
					string text3 = string.Format("{0}_{1}", text2, i);
					text4 = string.Format("{0}_{1}", text2, i + 1);
					if (File.Exists(text3))
					{
						this.FileStreamCopy(text3, text4);
					}
				}
				text4 = string.Format("{0}_{1}", text2, 0);
				this.FileStreamCopy(text, text4);
			}
		}
		catch (Exception ex)
		{
			CustomDebug.EditorLogError(ex.ToString());
			if (GameManager.Instance.UI)
			{
				GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.save.failed");
			}
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}
	}

	public void FileStreamCopy(string inputFilePath, string outputFilePath)
	{
		int num = 1048576;
		using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, num, FileOptions.WriteThrough))
		{
			using (FileStream fileStream2 = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				fileStream.SetLength(fileStream2.Length);
				byte[] array = new byte[num];
				int num2;
				while ((num2 = fileStream2.Read(array, 0, num)) > 0)
				{
					fileStream.Write(array, 0, num2);
				}
			}
		}
	}

	public string GetFilePath(int slot)
	{
		return string.Format("{0}{1}{2}{3}", new object[]
		{
			WindowsSaveStrategy.dataPath,
			WindowsSaveStrategy.saveTitle,
			slot,
			WindowsSaveStrategy.fileExtension
		});
	}

	public string GetMetaFilePath()
	{
		return string.Format("{0}{1}{2}", WindowsSaveStrategy.metaDataPath, WindowsSaveStrategy.metaTitle, WindowsSaveStrategy.fileExtension);
	}

	private static string saveTitle = "dredge-save";

	private static string metaTitle = "dredge-settings";

	private static string dataPath;

	private static string metaDataPath;

	private static string fileExtension = ".bin";

	private static string saveFolder = "saves";

	private static string backupSavesFolder = "backup-saves";
}
