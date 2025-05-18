using System;
using UnityEngine;

public class UIFocusObject : MonoBehaviour
{
	public RectTransform Container
	{
		get
		{
			return this.container;
		}
	}

	private void OnEnable()
	{
		if (GameManager.Instance.UIFocusObject == null)
		{
			GameManager.Instance.UIFocusObject = this;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance.UIFocusObject == this)
		{
			GameManager.Instance.UIFocusObject = null;
		}
	}

	private void Update()
	{
		if (this.hasTargetObject && this.targetObject == null)
		{
			this.ClearFocusObject(null, true);
			return;
		}
		if (!this.hasTargetObject)
		{
			return;
		}
		if ((!this.hasSizedFocusedObject || this.doesCurrentFocusObjectMove) && this.targetObject)
		{
			RectTransform component = this.targetObject.GetComponent<RectTransform>();
			if (component != null)
			{
				this.containerPosition.x = component.position.x + component.rect.center.x * GameManager.Instance.ScaleFactor;
				this.containerPosition.y = component.position.y + component.rect.center.y * GameManager.Instance.ScaleFactor;
				this.container.position = this.containerPosition;
				this.containerSize.x = component.rect.width;
				this.containerSize.y = component.rect.height;
				this.container.sizeDelta = this.containerSize;
				if (!this.delayForOneFrame)
				{
					this.container.gameObject.SetActive(true);
				}
				this.hasSizedFocusedObject = true;
			}
		}
		if (this.delayForOneFrame)
		{
			this.delayForOneFrame = false;
		}
	}

	public void SetFocusObject(GameObject target, bool doesObjectMove, bool delayForOneFrame)
	{
		this.targetObject = target;
		this.doesCurrentFocusObjectMove = doesObjectMove;
		this.hasSizedFocusedObject = false;
		this.hasTargetObject = true;
		this.delayForOneFrame = delayForOneFrame;
		if (GameManager.Instance.GridManager && GameManager.Instance.GridManager.CursorProxy)
		{
			GameManager.Instance.GridManager.CursorProxy.Hide();
		}
	}

	public void ClearFocusObject(GameObject target, bool force = false)
	{
		if (force || target == this.targetObject)
		{
			this.container.gameObject.SetActive(false);
			this.targetObject = null;
			this.doesCurrentFocusObjectMove = false;
			this.hasTargetObject = false;
		}
	}

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private GameObject targetObject;

	private bool hasSizedFocusedObject;

	private bool doesCurrentFocusObjectMove;

	private bool delayForOneFrame;

	private bool hasTargetObject;

	private Vector2 containerPosition = Vector2.zero;

	private Vector2 containerSize = Vector2.zero;
}
