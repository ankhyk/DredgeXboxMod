using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SubDestinationButton : MonoBehaviour
{
	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	private void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.BasicButtonWrapper;
		basicButtonWrapper2.OnSelectAction = (Action)Delegate.Combine(basicButtonWrapper2.OnSelectAction, new Action(this.OnButtonSelected));
		BasicButtonWrapper basicButtonWrapper3 = this.BasicButtonWrapper;
		basicButtonWrapper3.OnDeselectAction = (Action)Delegate.Combine(basicButtonWrapper3.OnDeselectAction, new Action(this.OnButtonDeselected));
	}

	private void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.BasicButtonWrapper;
		basicButtonWrapper2.OnSelectAction = (Action)Delegate.Remove(basicButtonWrapper2.OnSelectAction, new Action(this.OnButtonSelected));
		BasicButtonWrapper basicButtonWrapper3 = this.BasicButtonWrapper;
		basicButtonWrapper3.OnDeselectAction = (Action)Delegate.Remove(basicButtonWrapper3.OnDeselectAction, new Action(this.OnButtonDeselected));
	}

	private void OnButtonSelected()
	{
		Action<BaseDestination> onButtonSelectAction = this.OnButtonSelectAction;
		if (onButtonSelectAction == null)
		{
			return;
		}
		onButtonSelectAction(this.destination);
	}

	private void OnButtonDeselected()
	{
		Action<BaseDestination> onButtonDeselectAction = this.OnButtonDeselectAction;
		if (onButtonDeselectAction == null)
		{
			return;
		}
		onButtonDeselectAction(this.destination);
	}

	public void Init(BaseDestination destination)
	{
		this.destination = destination;
		if (destination.Icon != null)
		{
			this.icon.sprite = destination.Icon;
		}
		bool flag = destination.HighlightConditions.Any((HighlightCondition h) => h.ShouldHighlight());
		this.questAttentionCallout.SetActive(flag);
	}

	private void OnButtonClicked()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
		GameManager.Instance.UI.ShowDestination(this.destination);
	}

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private GameObject questAttentionCallout;

	public Action<BaseDestination> OnButtonSelectAction;

	public Action<BaseDestination> OnButtonDeselectAction;

	private BaseDestination destination;
}
