using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
	public bool _HasSaveFile
	{
		get
		{
			return this.hasSaveFile;
		}
	}

	public void DisableButtons()
	{
		BasicButtonWrapper basicButtonWrapper = this.selectSlotButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.deleteSlotButton;
		basicButtonWrapper2.OnClick = (Action)Delegate.Remove(basicButtonWrapper2.OnClick, new Action(this.OnDeleteClicked));
	}

	private void Awake()
	{
		BasicButtonWrapper basicButtonWrapper = this.selectSlotButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.deleteSlotButton;
		basicButtonWrapper2.OnClick = (Action)Delegate.Combine(basicButtonWrapper2.OnClick, new Action(this.OnDeleteClicked));
	}

	private void OnEnable()
	{
		this.SetupUI();
	}

	private void SetupUI()
	{
		this.hasSaveFile = GameManager.Instance.SaveManager.HasSaveFile(this.slotNum);
		this.saveData = (this.hasSaveFile ? GameManager.Instance.SaveManager.GetInMemorySaveDataForSlot(this.slotNum) : null);
		this.deleteSlotButton.gameObject.SetActive(this.hasSaveFile);
		this.loadImage.sprite = (this.hasSaveFile ? this.loadSprite : this.newSprite);
		this.selectSlotButton.LocalizedString.StringReference = (this.hasSaveFile ? this.loadString : this.newString);
		this.deleteSlotButton.gameObject.SetActive(this.hasSaveFile);
		if (this.hasSaveFile)
		{
			this.localizedSaveSlotInfo.enabled = false;
			this.localizedSaveSlotInfo.StringReference = this.infoString;
			string dockId = this.saveData.dockId;
			string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.STRING_TABLE, dockId, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
			decimal num = this.saveData.Time;
			if (num == 0m)
			{
				num = (decimal)this.saveData.GetFloatVariable("time", 0f);
			}
			int num2 = (int)Math.Ceiling(num);
			int intVariable = this.saveData.GetIntVariable("relics-handed-in", 0);
			this.localizedSaveSlotInfo.StringReference.Arguments = new string[]
			{
				localizedString,
				num2.ToString(),
				intVariable.ToString()
			};
			this.localizedSaveSlotInfo.enabled = true;
			this.dlc1Image.gameObject.SetActive(this.saveData.GetSaveUsesEntitlement(Entitlement.DLC_1));
			this.dlc2Image.gameObject.SetActive(this.saveData.GetSaveUsesEntitlement(Entitlement.DLC_2));
			return;
		}
		this.localizedSaveSlotInfo.StringReference = this.emptyString;
		this.dlc1Image.gameObject.SetActive(false);
		this.dlc2Image.gameObject.SetActive(false);
	}

	public void OnClicked()
	{
		if (!this.hasBeenClicked && !this.hasDeleteBeenClicked)
		{
			this.hasBeenClicked = true;
			this.saveData = GameManager.Instance.SaveManager.LoadIntoMemory(this.slotNum);
			if (this.hasSaveFile)
			{
				base.StartCoroutine(this.titleController.CheckIsSaveAllowedToBeLoaded(this.saveData, this.selectable, delegate(bool result)
				{
					if (result)
					{
						this.DoContinueOrNew(false);
						return;
					}
					this.hasBeenClicked = false;
					GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.POPUP_WINDOW);
				}));
				return;
			}
			this.DoContinueOrNew(true);
		}
	}

	private void DoContinueOrNew(bool canCreateNew)
	{
		Action saveSlotSelected = this.SaveSlotSelected;
		if (saveSlotSelected != null)
		{
			saveSlotSelected();
		}
		BasicButtonWrapper basicButtonWrapper = this.selectSlotButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnClicked));
		GameManager.Instance.SettingsSaveData.lastSaveSlot = this.slotNum;
		GameManager.Instance.Loader.LoadGameFromTitle(canCreateNew);
	}

	public void OnDeleteClicked()
	{
		if (this.hasBeenClicked)
		{
			return;
		}
		if (this.hasBeenClicked || this.hasDeleteBeenClicked)
		{
			return;
		}
		this.hasDeleteBeenClicked = true;
		Action saveSlotDeleteRequested = this.SaveSlotDeleteRequested;
		if (saveSlotDeleteRequested != null)
		{
			saveSlotDeleteRequested();
		}
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "popup.confirm-delete-save";
		DialogButtonOptions dialogButtonOptions = default(DialogButtonOptions);
		dialogButtonOptions.buttonString = "prompt.cancel";
		dialogButtonOptions.id = 0;
		dialogButtonOptions.hideOnButtonPress = true;
		dialogButtonOptions.isBackOption = true;
		DialogButtonOptions dialogButtonOptions2 = new DialogButtonOptions
		{
			buttonString = "prompt.confirm",
			id = 1,
			hideOnButtonPress = true
		};
		dialogOptions.buttonOptions = new DialogButtonOptions[] { dialogButtonOptions, dialogButtonOptions2 };
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnConfirmationResult));
	}

	private void OnConfirmationResult(DialogButtonOptions options)
	{
		if (options.id == 1)
		{
			GameManager.Instance.SaveManager.DeleteSaveFile(this.slotNum);
			if (GameManager.Instance.Input.IsUsingController)
			{
				EventSystem.current.SetSelectedGameObject(this.selectSlotButton.gameObject);
			}
			this.SetupUI();
		}
		else if (GameManager.Instance.Input.IsUsingController)
		{
			EventSystem.current.SetSelectedGameObject(this.deleteSlotButton.gameObject);
		}
		this.hasDeleteBeenClicked = false;
		Action saveSlotDeleteResolved = this.SaveSlotDeleteResolved;
		if (saveSlotDeleteResolved == null)
		{
			return;
		}
		saveSlotDeleteResolved();
	}

	[SerializeField]
	private TitleController titleController;

	[SerializeField]
	private LocalizeStringEvent localizedSaveSlotInfo;

	[SerializeField]
	private BasicButtonWrapper selectSlotButton;

	[SerializeField]
	private BasicButtonWrapper deleteSlotButton;

	[SerializeField]
	private Selectable selectable;

	[SerializeField]
	private LocalizedString loadString;

	[SerializeField]
	private LocalizedString newString;

	[SerializeField]
	private LocalizedString infoString;

	[SerializeField]
	private LocalizedString emptyString;

	[SerializeField]
	private Image loadImage;

	[SerializeField]
	private Image dlc1Image;

	[SerializeField]
	private Image dlc2Image;

	[SerializeField]
	private Sprite loadSprite;

	[SerializeField]
	private Sprite newSprite;

	[SerializeField]
	private int slotNum;

	public Action SaveSlotSelected;

	public Action SaveSlotDeleteRequested;

	public Action SaveSlotDeleteResolved;

	private bool hasSaveFile;

	private bool hasBeenClicked;

	private bool hasDeleteBeenClicked;

	private SaveData saveData;
}
