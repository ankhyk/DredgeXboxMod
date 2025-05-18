using System;
using UnityEngine;

public class SaveSlotWindow : PopupWindow
{
	protected override void Awake()
	{
		GameManager.Instance.SaveManager.LoadAllIntoMemory();
		base.Awake();
	}

	public override void Show()
	{
		for (int i = 0; i < this.saveSlots.Length; i++)
		{
			SaveSlotUI saveSlotUI = this.saveSlots[i];
			saveSlotUI.SaveSlotSelected = (Action)Delegate.Combine(saveSlotUI.SaveSlotSelected, new Action(this.OnSlotSelected));
			SaveSlotUI saveSlotUI2 = this.saveSlots[i];
			saveSlotUI2.SaveSlotDeleteRequested = (Action)Delegate.Combine(saveSlotUI2.SaveSlotDeleteRequested, new Action(this.RemoveActionListener));
			SaveSlotUI saveSlotUI3 = this.saveSlots[i];
			saveSlotUI3.SaveSlotDeleteResolved = (Action)Delegate.Combine(saveSlotUI3.SaveSlotDeleteResolved, new Action(this.OnDeleteResolved));
		}
		base.Show();
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		for (int i = 0; i < this.saveSlots.Length; i++)
		{
			SaveSlotUI saveSlotUI = this.saveSlots[i];
			saveSlotUI.SaveSlotSelected = (Action)Delegate.Remove(saveSlotUI.SaveSlotSelected, new Action(this.OnSlotSelected));
			SaveSlotUI saveSlotUI2 = this.saveSlots[i];
			saveSlotUI2.SaveSlotDeleteRequested = (Action)Delegate.Remove(saveSlotUI2.SaveSlotDeleteRequested, new Action(this.RemoveActionListener));
			SaveSlotUI saveSlotUI3 = this.saveSlots[i];
			saveSlotUI3.SaveSlotDeleteResolved = (Action)Delegate.Remove(saveSlotUI3.SaveSlotDeleteResolved, new Action(this.OnDeleteResolved));
		}
		base.Hide(windowHideMode);
	}

	private void OnSlotSelected()
	{
		this.RemoveActionListener();
		SaveSlotUI[] array = this.saveSlots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DisableButtons();
		}
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.SYSTEM);
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
	}

	private void OnDeleteResolved()
	{
		GameManager.Instance.SaveManager.LoadAllIntoMemory();
		this.AddActionListener();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.POPUP_WINDOW);
	}

	private void AddActionListener()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.AddActionListener(array, ActionLayer.POPUP_WINDOW);
	}

	private void RemoveActionListener()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.RemoveActionListener(array, ActionLayer.POPUP_WINDOW);
	}

	[SerializeField]
	private SaveSlotUI[] saveSlots;
}
