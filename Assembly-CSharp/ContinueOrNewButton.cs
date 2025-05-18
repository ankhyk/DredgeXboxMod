using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ContinueOrNewButton : MonoBehaviour
{
	public bool SuppressFocusGain
	{
		get
		{
			return this.suppressFocusGain;
		}
		set
		{
			this.suppressFocusGain = value;
		}
	}

	private void Start()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveDelete = (Action)Delegate.Combine(saveManager.OnSaveDelete, new Action(this.RefreshUI));
		this.RefreshUI();
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
		if (GameManager.Instance.Input.IsUsingController && !this.suppressFocusGain)
		{
			base.StartCoroutine(this.DoFocus());
		}
	}

	private IEnumerator DoFocus()
	{
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(this.button.gameObject);
		this.button.ManualOnSelect();
		yield break;
	}

	private void RefreshUI()
	{
		this.currentMode = (GameManager.Instance.SaveManager.CanLoadLast() ? ContinueOrNewButton.StartButtonMode.CONTINUE : ContinueOrNewButton.StartButtonMode.NEW);
		if (this.currentMode == ContinueOrNewButton.StartButtonMode.CONTINUE)
		{
			this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, "menu.continue");
			return;
		}
		if (this.currentMode == ContinueOrNewButton.StartButtonMode.NEW)
		{
			this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, "menu.new");
		}
	}

	private void OnDestroy()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveDelete = (Action)Delegate.Remove(saveManager.OnSaveDelete, new Action(this.RefreshUI));
	}

	public void OnClick()
	{
		if (!this.hasBeenClicked)
		{
			this.hasBeenClicked = true;
			if (this.currentMode == ContinueOrNewButton.StartButtonMode.CONTINUE)
			{
				SaveData saveData = GameManager.Instance.SaveManager.LoadIntoMemory(GameManager.Instance.SaveManager.ActiveSettingsData.lastSaveSlot);
				base.StartCoroutine(this.titleController.CheckIsSaveAllowedToBeLoaded(saveData, this.selectable, delegate(bool result)
				{
					if (result)
					{
						GameManager.Instance.Loader.LoadGameFromTitle(false);
						return;
					}
					this.hasBeenClicked = false;
				}));
				return;
			}
			if (this.currentMode == ContinueOrNewButton.StartButtonMode.NEW)
			{
				GameManager.Instance.Loader.LoadGameFromTitle(true);
			}
		}
	}

	[SerializeField]
	private TitleController titleController;

	[SerializeField]
	private BasicButtonWrapper button;

	[SerializeField]
	private Selectable selectable;

	[SerializeField]
	private LocalizeStringEvent localizedString;

	private bool canLoadLast;

	private bool hasBeenClicked;

	private bool suppressFocusGain;

	private ContinueOrNewButton.StartButtonMode currentMode;

	private enum StartButtonMode
	{
		NEW,
		CONTINUE
	}
}
