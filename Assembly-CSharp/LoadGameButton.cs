using System;
using UnityEngine;

public class LoadGameButton : MonoBehaviour
{
	private void Awake()
	{
		if (!GameManager.Instance.SaveManager.HasAnySaveFiles())
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveDelete = (Action)Delegate.Combine(saveManager.OnSaveDelete, new Action(this.RefreshUI));
		this.RefreshUI();
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	private void OnDestroy()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveDelete = (Action)Delegate.Remove(saveManager.OnSaveDelete, new Action(this.RefreshUI));
	}

	private void RefreshUI()
	{
		this.hasSaveFiles = GameManager.Instance.SaveManager.HasAnySaveFiles();
	}

	public void OnClick()
	{
		if (!this.saveSlotWindow.IsShowing)
		{
			this.saveSlotWindow.Show();
		}
	}

	[SerializeField]
	private SaveSlotWindow saveSlotWindow;

	[SerializeField]
	private BasicButtonWrapper button;

	private bool hasBeenClicked;

	private bool hasSaveFiles;
}
