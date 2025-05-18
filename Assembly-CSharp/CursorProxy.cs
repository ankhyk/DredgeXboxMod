using System;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorProxy : MonoBehaviour
{
	public RectTransform CursorSquare
	{
		get
		{
			return this.cursorSquare;
		}
	}

	public Vector3 GetPosition()
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			return this.cursorPos;
		}
		return Input.mousePosition;
	}

	private void Awake()
	{
		this.cursorPos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		this.UpdateCursorBorder();
	}

	private void OnEnable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnItemPickedUp += this.UpdateCursorBorder;
		GameEvents.Instance.OnItemPlaceComplete += this.UpdateCursorBorder;
		GameEvents.Instance.OnItemRemovedFromCursor += this.UpdateCursorBorder;
		GameEvents.Instance.OnItemRotated += this.UpdateCursorBorder;
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnItemPlaceComplete -= this.UpdateCursorBorder;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.UpdateCursorBorder;
		GameEvents.Instance.OnItemRotated -= this.UpdateCursorBorder;
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.UpdateCursorBorder();
	}

	public void MoveTo(Vector3 pos)
	{
		this.cursorPos = pos;
		this.cursorSquare.position = this.cursorPos;
		this.UpdateCursorBorder();
	}

	public void Show()
	{
		this.cursorBorderImage.gameObject.SetActive(true);
	}

	public void Hide()
	{
		if (this.cursorBorderImage && this.cursorBorderImage.gameObject && this.cursorBorderImage.gameObject.activeSelf)
		{
			this.cursorBorderImage.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (!GameManager.Instance.IsPaused)
		{
			if (GameManager.Instance.Input.CurrentBindingSource != BindingSourceType.DeviceBindingSource)
			{
				this.cursorPos = Input.mousePosition;
			}
			this.cursorSquare.position = this.cursorPos;
			if (this.cursorBorderImage.gameObject.activeSelf && !GameManager.Instance.GridManager.IsShowingGrid)
			{
				this.Hide();
			}
		}
	}

	public void UpdateCursorBorder(GridObject go, bool result)
	{
		this.UpdateCursorBorder();
	}

	public void UpdateCursorBorder(GridObject go)
	{
		this.UpdateCursorBorder();
	}

	public void UpdateCursorBorder()
	{
		GridObject currentlyHeldObject = GameManager.Instance.GridManager.CurrentlyHeldObject;
		if (currentlyHeldObject)
		{
			RectTransform rectTransform = currentlyHeldObject.transform as RectTransform;
			if (currentlyHeldObject.CurrentRotation == 90 || currentlyHeldObject.CurrentRotation == 270)
			{
				this.cursorBorderImage.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y, rectTransform.sizeDelta.x);
			}
			else
			{
				this.cursorBorderImage.rectTransform.sizeDelta = rectTransform.sizeDelta;
			}
		}
		else if (EventSystem.current.currentSelectedGameObject != null)
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			this.cursorBorderImage.rectTransform.sizeDelta = (currentSelectedGameObject.transform as RectTransform).sizeDelta;
		}
		this.UpdateShouldShowCursor();
		base.transform.SetAsLastSibling();
	}

	public void UpdateShouldShowCursor()
	{
		if (GameManager.Instance.GridManager.IsShowingGrid && (GameManager.Instance.Input.IsUsingController || GameManager.Instance.GridManager.CurrentlyHeldObject))
		{
			this.Show();
			return;
		}
		this.Hide();
	}

	private Vector3 cursorPos;

	[SerializeField]
	private Image cursorBorderImage;

	[SerializeField]
	private RectTransform cursorSquare;
}
