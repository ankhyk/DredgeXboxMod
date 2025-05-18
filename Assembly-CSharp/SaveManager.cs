using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class SaveManager : MonoBehaviour
{
	public SaveData ActiveSaveData
	{
		get
		{
			return this.activeSaveData;
		}
	}

	public SettingsSaveData ActiveSettingsData
	{
		get
		{
			return this.activeSettingsData;
		}
	}

	public ISaveStrategy SaveStrategy
	{
		get
		{
			return this.saveStrategy;
		}
	}

	public bool DidDeleteCorruptData
	{
		get
		{
			return this.didDeleteCorruptData;
		}
		set
		{
			this.didDeleteCorruptData = value;
		}
	}

	public void Init()
	{
		this.allSaveData = new SaveData[this.numSlots];
		this.useAsyncSave = false;
		this.saveStrategy = new GDKPCSaveStrategy();
		this.saveStrategy.Init();
		bool flag = false;
		if (this.saveStrategy.HasFile(this.saveStrategy.GetMetaFilePath()))
		{
			this.activeSettingsData = this.saveStrategy.GetData<SettingsSaveData>(this.saveStrategy.GetMetaFilePath());
			flag = this.activeSettingsData != null;
		}
		if (!flag)
		{
			this.CreateMetaData();
		}
		this.ApplyMetaFileFixes();
		this.CheckForCorruptData();
		this.useAsyncSave = true;
		ApplicationEvents.Instance.TriggerSaveManagerInitialized();
	}

	private void CheckForCorruptData()
	{
		for (int i = 0; i < this.numSlots; i++)
		{
			bool flag = false;
			try
			{
				if (this.HasSaveFile(i) && this.LoadIntoMemory(i) == null)
				{
					throw new Exception(string.Format("[SaveManager] CheckForCorruptData() failed to load data in slot {0}.", i));
				}
			}
			catch (Exception ex)
			{
				CustomDebug.EditorLogError(ex.ToString());
				flag = true;
			}
			if (flag)
			{
				this.DeleteSaveFile(i);
				this.didDeleteCorruptData = true;
			}
		}
	}

	private void ApplyMetaFileFixes()
	{
		if (this.activeSettingsData.voiceVolume == 0f)
		{
			this.activeSettingsData.voiceVolume = GameManager.Instance.GameConfigData.SettingsSaveDataTemplate.voiceVolume;
		}
		if (this.activeSettingsData.spyglassCameraSensitivityX == 0f)
		{
			this.activeSettingsData.spyglassCameraSensitivityX = GameManager.Instance.GameConfigData.SettingsSaveDataTemplate.spyglassCameraSensitivityX;
		}
		if (this.activeSettingsData.spyglassCameraSensitivityY == 0f)
		{
			this.activeSettingsData.spyglassCameraSensitivityY = GameManager.Instance.GameConfigData.SettingsSaveDataTemplate.spyglassCameraSensitivityY;
		}
	}

	private void OnDestroy()
	{
		this.saveStrategy.OnDestroy();
	}

	public bool HasAnySaveFiles()
	{
		for (int i = 0; i < this.numSlots; i++)
		{
			if (this.HasSaveFile(i))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasSaveFile(int slot)
	{
		return this.saveStrategy.HasFile(this.saveStrategy.GetFilePath(slot));
	}

	public void DeleteSaveFile(int slot)
	{
		this.saveStrategy.Delete(slot);
		Action onSaveDelete = this.OnSaveDelete;
		if (onSaveDelete != null)
		{
			onSaveDelete();
		}
		this.RemoveInMemorySaveDataForSlot(slot);
	}

	public void CreateSaveData(int slot)
	{
		this.activeSaveData = new SaveData(GameManager.Instance.GameConfigData.SaveDataTemplate);
		this.TrySwapBasicRodForPreorderRod(this.activeSaveData);
		this.activeSaveSlot = slot;
		this.SaveGameFile(false);
	}

	private void TrySwapBasicRodForPreorderRod(SaveData saveData)
	{
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.PREORDER))
		{
			SpatialItemInstance spatialItemInstance = saveData.grids[GridKey.INVENTORY].spatialItems.Find((SpatialItemInstance itemInstance) => itemInstance.id == "rod1");
			if (spatialItemInstance != null)
			{
				spatialItemInstance.id = "rod21";
			}
		}
	}

	public void CreateMetaData()
	{
		int num = 0;
		if (this.activeSettingsData != null)
		{
			num = this.activeSettingsData.lastSaveSlot;
		}
		this.activeSettingsData = GameManager.Instance.GameConfigData.SettingsSaveDataTemplate.GetData();
		this.activeSettingsData.lastSaveSlot = num;
		this.SaveData<SettingsSaveData>(this.ObjectToByteArray(this.activeSettingsData), this.saveStrategy.GetMetaFilePath(), false);
	}

	public void Save()
	{
		if (!this.activeSaveData.ForbidSave)
		{
			this.SaveGameFile(true);
			this.SaveData<SettingsSaveData>(this.ObjectToByteArray(this.activeSettingsData), this.saveStrategy.GetMetaFilePath(), false);
			return;
		}
		Action onSaveComplete = this.OnSaveComplete;
		if (onSaveComplete == null)
		{
			return;
		}
		onSaveComplete();
	}

	private void SaveGameFile(bool useBackupHistory)
	{
		this.activeSaveData.StampEntitlements();
		this.activeSaveData.StampSaveTime();
		this.SaveData<SaveData>(this.ObjectToByteArray(this.activeSaveData), this.saveStrategy.GetFilePath(this.activeSaveSlot), useBackupHistory);
	}

	public void SaveSettings()
	{
		this.SaveData<SettingsSaveData>(this.ObjectToByteArray(this.activeSettingsData), this.saveStrategy.GetMetaFilePath(), false);
	}

	private void SaveData<T>(byte[] data, string filePath, bool useBackupHistory = false)
	{
		Action onSaveStart = this.OnSaveStart;
		if (onSaveStart != null)
		{
			onSaveStart();
		}
		this.saveStrategy.SaveData<T>(data, filePath, useBackupHistory);
		Action onSaveComplete = this.OnSaveComplete;
		if (onSaveComplete == null)
		{
			return;
		}
		onSaveComplete();
	}

	public byte[] ObjectToByteArray(object obj)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, obj);
			array = memoryStream.ToArray();
		}
		return array;
	}

	public SaveData GetInMemorySaveDataForSlot(int slot)
	{
		return this.allSaveData[slot];
	}

	public void RemoveInMemorySaveDataForSlot(int slot)
	{
		this.allSaveData[slot] = null;
	}

	public void LoadAllIntoMemory()
	{
		this.allSaveData = new SaveData[this.numSlots];
		for (int i = 0; i < this.numSlots; i++)
		{
			if (this.HasSaveFile(i))
			{
				this.allSaveData[i] = this.LoadIntoMemory(i);
			}
		}
	}

	public SaveData LoadIntoMemory(int slot)
	{
		string filePath = this.saveStrategy.GetFilePath(slot);
		return this.saveStrategy.GetData<SaveData>(filePath);
	}

	public bool Load(int slot)
	{
		bool flag = false;
		string filePath = this.saveStrategy.GetFilePath(slot);
		SaveData saveData = this.saveStrategy.GetData<SaveData>(filePath);
		if (saveData != null)
		{
			saveData = this.TryReplaceUnownedDLCItems(saveData);
			this.activeSaveData = saveData;
			this.activeSaveSlot = slot;
			if (saveData.version == GameManager.Instance.GameConfigData.SaveDataTemplate.version)
			{
				flag = true;
			}
			else
			{
				CustomDebug.EditorLogError(string.Format("[SaveManager] Load({0}) succeeded, but version mismatch detected.", slot));
				CustomDebug.EditorLogError(string.Format("[SaveManager] Load({0}) TODO: PERFORM SAVE MIGRATION HERE.", slot));
			}
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[SaveManager] Load({0}) failed", slot));
		}
		return flag;
	}

	private SaveData TryReplaceUnownedDLCItems(SaveData loadedData)
	{
		SerializableGrid inventoryGrid = loadedData.GetGridByKey(GridKey.INVENTORY);
		SerializableGrid storageGrid = loadedData.GetGridByKey(GridKey.STORAGE);
		bool hasEntitlement = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.PREORDER);
		bool hasEntitlement2 = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DELUXE);
		if (!hasEntitlement)
		{
			if (inventoryGrid != null)
			{
				SpatialItemInstance spatialItemInstance = inventoryGrid.spatialItems.Find((SpatialItemInstance itemInstance) => itemInstance.id == "rod21");
				if (spatialItemInstance != null)
				{
					spatialItemInstance.id = "rod1";
				}
			}
			if (storageGrid != null)
			{
				SpatialItemInstance spatialItemInstance = storageGrid.spatialItems.Find((SpatialItemInstance itemInstance) => itemInstance.id == "rod21");
				if (spatialItemInstance != null)
				{
					spatialItemInstance.id = "rod1";
				}
			}
		}
		if (!hasEntitlement2)
		{
			new List<string> { "rod20", "engine10" }.ForEach(delegate(string deluxeItemID)
			{
				if (inventoryGrid != null)
				{
					SpatialItemInstance spatialItemInstance2 = inventoryGrid.spatialItems.Find((SpatialItemInstance itemInstance) => itemInstance.id == deluxeItemID);
					if (spatialItemInstance2 != null)
					{
						inventoryGrid.spatialItems.Remove(spatialItemInstance2);
					}
				}
				if (storageGrid != null)
				{
					SpatialItemInstance spatialItemInstance2 = storageGrid.spatialItems.Find((SpatialItemInstance itemInstance) => itemInstance.id == deluxeItemID);
					if (spatialItemInstance2 != null)
					{
						storageGrid.spatialItems.Remove(spatialItemInstance2);
					}
				}
			});
		}
		return loadedData;
	}

	public bool CanLoadLast()
	{
		int lastSaveSlot = this.activeSettingsData.lastSaveSlot;
		return lastSaveSlot >= 0 && lastSaveSlot < this.numSlots && this.HasSaveFile(lastSaveSlot);
	}

	public void LoadLast(bool canCreateNew = true)
	{
		int lastSaveSlot = this.activeSettingsData.lastSaveSlot;
		bool flag = false;
		if (this.CanLoadLast())
		{
			this.isLoadingForReal = true;
			flag = this.Load(lastSaveSlot);
		}
		if (!flag && canCreateNew)
		{
			this.CreateSaveData(lastSaveSlot);
		}
	}

	private int numSlots = 4;

	private int activeSaveSlot;

	private SaveData[] allSaveData;

	private SaveData activeSaveData;

	private SettingsSaveData activeSettingsData;

	public Action OnSaveDelete;

	public Action OnSaveStart;

	public Action OnSaveComplete;

	private ISaveStrategy saveStrategy;

	public bool forceCrashDuringSave;

	public bool forceSaveBadData;

	public bool forceLoadBadData;

	public bool isLoadingForReal;

	private bool didDeleteCorruptData;

	[HideInInspector]
	public bool useAsyncSave = true;
}
