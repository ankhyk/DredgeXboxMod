using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ControlEntryUI : MonoBehaviour
{
	public PlayerAction PlayerAction { get; set; }

	public bool Rebindable { get; set; }

	public bool Unbindable { get; set; }

	public List<ControlBindingEntryUI> ControlBindingEntryUIs
	{
		get
		{
			return this.controlBindingEntryUIs;
		}
	}

	public ResetControlEntryUI ResetEntryUI
	{
		get
		{
			return this.resetEntryUI;
		}
	}

	public void Init(PlayerAction playerAction, bool rebindable, bool unbindable)
	{
		this.PlayerAction = playerAction;
		this.localizedPromptNameField.StringReference.SetReference(LanguageManager.STRING_TABLE, playerAction.Name);
		this.Rebindable = rebindable;
		this.Unbindable = unbindable;
		this.ControlBindingEntryUIs.ForEach(delegate(ControlBindingEntryUI c)
		{
			if (c != null)
			{
				c.Init(playerAction, rebindable, unbindable);
			}
		});
	}

	public void Refresh()
	{
		this.Init(this.PlayerAction, this.Rebindable, this.Unbindable);
	}

	[SerializeField]
	private LocalizeStringEvent localizedPromptNameField;

	[SerializeField]
	private List<ControlBindingEntryUI> controlBindingEntryUIs;

	[SerializeField]
	private ResetControlEntryUI resetEntryUI;
}
