using System;
using System.Collections;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class BasicButtonWrapper : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IPointerClickHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public Button Button
	{
		get
		{
			return this.button;
		}
	}

	public LocalizeStringEvent LocalizedString
	{
		get
		{
			return this.localizedString;
		}
	}

	public float TimeUntilTransition
	{
		get
		{
			return this.timeUntilTransition;
		}
		set
		{
			this.timeUntilTransition = value;
		}
	}

	public bool Interactable
	{
		get
		{
			return this.button.interactable;
		}
		set
		{
			this.button.interactable = value;
		}
	}

	public Navigation Navigation
	{
		get
		{
			return this.button.navigation;
		}
		set
		{
			this.button.navigation = value;
		}
	}

	private void Update()
	{
		if (this.awaitingTransition)
		{
			this.timeUntilTransition -= Time.deltaTime;
			if (this.timeUntilTransition <= 0f)
			{
				this.DoTransitionIn();
			}
		}
	}

	private void Awake()
	{
		this.defaultTransition = this.button.transition;
		if (this.transitionEffect)
		{
			this.transitionEffect.duration = UIController.TRANSITION_IN_DURATION_SEC;
		}
	}

	public void PlaySubmitSFX()
	{
		if (this.button.interactable)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.submitSFX, AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	public void PlaySelectSFX()
	{
		if (this.button.interactable)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.selectSFX, AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	private void OnEnable()
	{
		if (this.randomizeTransitionInDelay)
		{
			this.transitionInDelaySec = global::UnityEngine.Random.Range(this.randomTransitionInDelaySecMin, this.randomTransitionInDelaySecMax);
		}
		if (this.automaticallyAnimateTransition)
		{
			this.transitionEffect.effectFactor = 0f;
			this.needsAnimatingWhenReEnabling = true;
		}
		if (this.needsAnimatingWhenReEnabling)
		{
			this.needsAnimatingWhenReEnabling = false;
			this.awaitingTransition = true;
			this.SetCanBeClicked(false);
			this.timeUntilTransition = this.transitionInDelaySec;
		}
		this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
	}

	private void OnDisable()
	{
		this.button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
		if (this.automaticallyAnimateTransition)
		{
			this.needsAnimatingWhenReEnabling = true;
		}
	}

	private void OnButtonClicked()
	{
		if (this.canBeClicked)
		{
			Action onClick = this.OnClick;
			if (onClick == null)
			{
				return;
			}
			onClick();
		}
	}

	private void Start()
	{
		if (this.needsAnimatingWhenReEnabling)
		{
			this.needsAnimatingWhenReEnabling = false;
			this.awaitingTransition = true;
			this.SetCanBeClicked(false);
			this.timeUntilTransition = this.transitionInDelaySec;
		}
		if (this.transitionOutOnClick)
		{
			this.OnClick = (Action)Delegate.Combine(this.OnClick, new Action(this.TransitionOut));
		}
	}

	public void SetCanBeClicked(bool canBeClicked)
	{
		this.canBeClicked = canBeClicked;
		this.uiSelectable.ClearFocusOnSubmit = canBeClicked;
	}

	public void SetSelectable(ControllerFocusGrabber controllerFocusGrabber)
	{
		controllerFocusGrabber.SetSelectable(this.button);
	}

	public void ManualOnSelect()
	{
		this.button.OnSelect(null);
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action onSelectAction = this.OnSelectAction;
		if (onSelectAction != null)
		{
			onSelectAction();
		}
		this.PlaySelectSFX();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action onSubmitAction = this.OnSubmitAction;
		if (onSubmitAction != null)
		{
			onSubmitAction();
		}
		this.PlaySubmitSFX();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Action onDeselectAction = this.OnDeselectAction;
		if (onDeselectAction == null)
		{
			return;
		}
		onDeselectAction();
	}

	private void DoTransitionIn()
	{
		this.awaitingTransition = false;
		this.transitionEffect.Show(true);
		if (!this.canBeClicked)
		{
			base.StartCoroutine(this.DoMakeClickable());
		}
	}

	private IEnumerator DoMakeClickable()
	{
		yield return new WaitForSecondsRealtime(this.transitionEffect.duration);
		this.SetCanBeClicked(true);
		yield break;
	}

	private void TransitionOut()
	{
		this.DoTransitionOut(UIController.TRANSITION_OUT_DURATION_SEC);
	}

	private void DoTransitionOut(float durationSec)
	{
		this.transitionEffect.duration = durationSec;
		this.transitionEffect.Hide(true);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.OnSelect(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		this.OnDeselect(eventData);
		Action onPointerExitAction = this.OnPointerExitAction;
		if (onPointerExitAction == null)
		{
			return;
		}
		onPointerExitAction();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Action onPointerDownAction = this.OnPointerDownAction;
		if (onPointerDownAction == null)
		{
			return;
		}
		onPointerDownAction();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Action onPointerUpAction = this.OnPointerUpAction;
		if (onPointerUpAction == null)
		{
			return;
		}
		onPointerUpAction();
	}

	[HideInInspector]
	public Action OnClick;

	[HideInInspector]
	public Action OnSelectAction;

	[HideInInspector]
	public Action OnDeselectAction;

	[HideInInspector]
	public Action OnSubmitAction;

	[HideInInspector]
	public Action OnPointerDownAction;

	[HideInInspector]
	public Action OnPointerUpAction;

	[HideInInspector]
	public Action OnPointerExitAction;

	[SerializeField]
	private Button button;

	[SerializeField]
	private LocalizeStringEvent localizedString;

	[SerializeField]
	private bool canBeClicked = true;

	[SerializeField]
	private UISelectable uiSelectable;

	[Header("Transition FX")]
	[SerializeField]
	private bool automaticallyAnimateTransition;

	[SerializeField]
	private bool randomizeTransitionInDelay;

	[SerializeField]
	private bool transitionOutOnClick;

	[SerializeField]
	public float randomTransitionInDelaySecMin;

	[SerializeField]
	public float randomTransitionInDelaySecMax;

	[SerializeField]
	public float transitionInDelaySec;

	[SerializeField]
	private UITransitionEffect transitionEffect;

	[Header("SFX")]
	[SerializeField]
	private AudioClip submitSFX;

	[SerializeField]
	private AudioClip selectSFX;

	private bool needsAnimatingWhenReEnabling;

	private Coroutine transitionInCoroutine;

	private bool awaitingTransition;

	private float timeUntilTransition;

	private Selectable.Transition defaultTransition;
}
