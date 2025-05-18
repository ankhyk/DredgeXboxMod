using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPromptUI : MonoBehaviour
{
	private void Awake()
	{
		this.RefreshControlPromptUI(GameManager.Instance.Input.GetActiveActionSet());
	}

	private void OnEnable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnPlayerActionSetChanged = (Action<DredgePlayerActionSet>)Delegate.Combine(input.OnPlayerActionSetChanged, new Action<DredgePlayerActionSet>(this.OnActivePlayerActionSetChanged));
		this.RefreshControlPromptUI(GameManager.Instance.Input.GetActiveActionSet());
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnPlayerActionSetChanged = (Action<DredgePlayerActionSet>)Delegate.Remove(input.OnPlayerActionSetChanged, new Action<DredgePlayerActionSet>(this.OnActivePlayerActionSetChanged));
	}

	private void OnActivePlayerActionSetChanged(DredgePlayerActionSet actionSet)
	{
		this.RefreshControlPromptUI(actionSet);
	}

	private void RefreshControlPromptUI(DredgePlayerActionSet actionSet)
	{
		if (actionSet == null)
		{
			for (int i = this.controlPrompts.Count - 1; i >= 0; i--)
			{
				this.controlPrompts[i].gameObject.Recycle();
			}
			this.controlPrompts.Clear();
		}
		else
		{
			for (int j = this.controlPrompts.Count - 1; j >= 0; j--)
			{
				this.controlPrompts[j].gameObject.Recycle();
			}
			this.controlPrompts.Clear();
			List<DredgePlayerActionBase> list = null;
			if (this.controlPromptMode == ControlPromptUI.ControlPromptMode.BOTTOM_RIGHT)
			{
				list = actionSet.PlayerActions().FindAll((DredgePlayerActionBase a) => a.showInControlArea);
			}
			else if (this.controlPromptMode == ControlPromptUI.ControlPromptMode.TOOLTIP)
			{
				list = actionSet.PlayerActions().FindAll((DredgePlayerActionBase a) => a.showInTooltip);
			}
			if (list != null)
			{
				for (int k = 0; k < list.Count; k++)
				{
					ControlPromptEntryUI component = this.controlPromptEntryPrefab.Spawn(this.controlPromptEntryContainer.transform).GetComponent<ControlPromptEntryUI>();
					if (component)
					{
						DredgePlayerActionBase dredgePlayerActionBase = list[k];
						component.Init(dredgePlayerActionBase, this.controlPromptMode);
						this.controlPrompts.Add(component);
					}
				}
			}
		}
		this._isDirty = true;
	}

	private void LateUpdate()
	{
		if (this._isDirty)
		{
			Canvas.ForceUpdateCanvases();
			this.verticalLayoutGroup.enabled = false;
			this.verticalLayoutGroup.enabled = true;
			this._isDirty = false;
		}
	}

	[SerializeField]
	private VerticalLayoutGroup verticalLayoutGroup;

	[SerializeField]
	private GameObject controlPromptEntryContainer;

	[SerializeField]
	private GameObject controlPromptEntryPrefab;

	[SerializeField]
	private ControlPromptUI.ControlPromptMode controlPromptMode;

	private List<ControlPromptEntryUI> controlPrompts = new List<ControlPromptEntryUI>();

	private bool _isDirty;

	public enum ControlPromptMode
	{
		NONE,
		BOTTOM_RIGHT,
		TOOLTIP,
		CUSTOM
	}
}
