using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapStampContainer : MonoBehaviour
{
	public int CurrentStampIndex
	{
		get
		{
			return this.currentStampIndex;
		}
	}

	private void Awake()
	{
		for (int i = 0; i < this.config.stampSprites.Count; i++)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.mapStampPrefab, this.stampContainer);
			Image component = gameObject.GetComponent<Image>();
			component.sprite = this.config.stampSprites[i];
			component.color = GameManager.Instance.LanguageManager.GetColor(this.config.stampColors[i]);
			this.stamps.Add(gameObject);
			int capturedIndex = i;
			BasicButtonWrapper component2 = gameObject.GetComponent<BasicButtonWrapper>();
			component2.OnPointerDownAction = (Action)Delegate.Combine(component2.OnPointerDownAction, new Action(delegate
			{
				this.OnStampClicked(capturedIndex);
			}));
		}
		this.prevActionPress = new DredgePlayerActionPress("Tab Left", GameManager.Instance.Input.Controls.TabLeft);
		this.prevActionPress.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.prevActionPress;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPrevStampPressed));
		this.nextActionPress = new DredgePlayerActionPress("Tab Right", GameManager.Instance.Input.Controls.TabRight);
		this.nextActionPress.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.nextActionPress;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnNextStampPressed));
		this.prevControlPrompt.Init(this.prevActionPress);
		this.nextControlPrompt.Init(this.nextActionPress);
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.DelayedSelectStamp());
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.prevActionPress, this.nextActionPress }, ActionLayer.POPUP_WINDOW);
	}

	private IEnumerator DelayedSelectStamp()
	{
		yield return new WaitForEndOfFrame();
		this.DoSelectStamp();
		yield break;
	}

	private void OnDisable()
	{
		EventSystem.current.SetSelectedGameObject(null);
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.prevActionPress, this.nextActionPress }, ActionLayer.POPUP_WINDOW);
	}

	private void OnPrevStampPressed()
	{
		this.currentStampIndex--;
		if (this.currentStampIndex < 0)
		{
			this.currentStampIndex = this.stamps.Count - 1;
		}
		this.DoSelectStamp();
	}

	private void OnNextStampPressed()
	{
		this.currentStampIndex++;
		if (this.currentStampIndex > this.stamps.Count - 1)
		{
			this.currentStampIndex = 0;
		}
		this.DoSelectStamp();
	}

	private void DoSelectStamp()
	{
		Selectable component = this.stamps[this.currentStampIndex].GetComponent<Selectable>();
		EventSystem.current.SetSelectedGameObject(component.gameObject);
		component.Select();
	}

	private void OnStampClicked(int i)
	{
		this.currentStampIndex = i;
		this.DoSelectStamp();
		Action stampClickedAction = this.StampClickedAction;
		if (stampClickedAction == null)
		{
			return;
		}
		stampClickedAction();
	}

	[SerializeField]
	private Transform stampContainer;

	[SerializeField]
	private GameObject mapStampPrefab;

	[SerializeField]
	private MapStampConfig config;

	[SerializeField]
	private ControlPromptIcon prevControlPrompt;

	[SerializeField]
	private ControlPromptIcon nextControlPrompt;

	public Action StampClickedAction;

	private List<GameObject> stamps = new List<GameObject>();

	private DredgePlayerActionPress prevActionPress;

	private DredgePlayerActionPress nextActionPress;

	private int currentStampIndex = 2;
}
