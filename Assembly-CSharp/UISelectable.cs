using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectable : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerClickHandler
{
	public bool ClearFocusOnSubmit
	{
		get
		{
			return this.clearFocusOnSubmit;
		}
		set
		{
			this.clearFocusOnSubmit = value;
		}
	}

	public bool ClearFocusOnDestroy
	{
		get
		{
			return this.clearFocusOnDestroy;
		}
		set
		{
			this.clearFocusOnDestroy = value;
		}
	}

	public bool ClearFocusOnDisable
	{
		get
		{
			return this.clearFocusOnDisable;
		}
		set
		{
			this.clearFocusOnDisable = value;
		}
	}

	public bool DoesSelectableMove
	{
		get
		{
			return this.doesSelectableMove;
		}
		set
		{
			this.doesSelectableMove = value;
		}
	}

	public bool DelayForOneFrame
	{
		get
		{
			return this.delayForOneFrame;
		}
		set
		{
			this.delayForOneFrame = value;
		}
	}

	public virtual void OnSelect(BaseEventData eventData)
	{
		GameManager.Instance.UIFocusObject.SetFocusObject(base.gameObject, this.doesSelectableMove, this.delayForOneFrame);
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (this.strictPointerEnter && eventData.pointerEnter != base.gameObject)
		{
			return;
		}
		GameManager.Instance.UIFocusObject.SetFocusObject(base.gameObject, this.doesSelectableMove, this.delayForOneFrame);
	}

	public virtual void OnDeselect(BaseEventData eventData)
	{
		GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		if (this.clearFocusOnSubmit)
		{
			GameManager instance = GameManager.Instance;
			if ((instance != null) ? instance.UIFocusObject : null)
			{
				GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
			}
		}
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (this.clearFocusOnSubmit)
		{
			GameManager instance = GameManager.Instance;
			if ((instance != null) ? instance.UIFocusObject : null)
			{
				GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (this.clearFocusOnDestroy)
		{
			GameManager instance = GameManager.Instance;
			if ((instance != null) ? instance.UIFocusObject : null)
			{
				GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (this.clearFocusOnDisable)
		{
			GameManager instance = GameManager.Instance;
			if ((instance != null) ? instance.UIFocusObject : null)
			{
				GameManager.Instance.UIFocusObject.ClearFocusObject(base.gameObject, false);
			}
		}
	}

	[SerializeField]
	private bool clearFocusOnSubmit;

	[SerializeField]
	private bool clearFocusOnDestroy;

	[SerializeField]
	private bool clearFocusOnDisable;

	[SerializeField]
	private bool doesSelectableMove;

	[SerializeField]
	private bool delayForOneFrame;

	[SerializeField]
	private bool strictPointerEnter;
}
