using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InspectPOIHandler : MonoBehaviour
{
	public bool IsHandlerActive { get; set; }

	public IConversationStarter ConversationStarter
	{
		get
		{
			return this.conversationStarter;
		}
		set
		{
			this.conversationStarter = value;
		}
	}

	private void Awake()
	{
		this.inspectAction = new DredgePlayerActionPress("prompt.inspect", GameManager.Instance.Input.Controls.Interact);
		DredgePlayerActionPress dredgePlayerActionPress = this.inspectAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
		this.inspectAction.showInControlArea = true;
		this.inspectAction.allowPreholding = true;
	}

	private void OnDestroy()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.inspectAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
	}

	public void Activate(ConversationPOI conversationPOI)
	{
		if (conversationPOI.RefreshStatus())
		{
			this.IsHandlerActive = true;
			this.conversationStarter = conversationPOI;
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.inspectAction };
			input.AddActionListener(array, ActionLayer.BASE);
		}
	}

	public void Deactivate()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.inspectAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		this.IsHandlerActive = false;
	}

	private void OnPressComplete()
	{
		GameEvents.Instance.TriggerPlayerInteractedWithPOI();
		this.inspectAction.Disable(true);
		this.StartDialogue();
	}

	private void OnTriggerEnter(Collider other)
	{
		ConversationTrigger component = other.GetComponent<ConversationTrigger>();
		if (component)
		{
			this.conversationStarter = component;
			this.StartDialogue();
		}
	}

	private void StartDialogue()
	{
		if (!GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.DIALOGUE) && this.conversationStarter != null && (!this.conversationStarter.IsOneTimeOnly || !GameManager.Instance.DialogueRunner.GetHasVisitedNode(this.conversationStarter.ConversationNodeName)))
		{
			GameManager.Instance.DialogueRunner.StartDialogue(this.conversationStarter.ConversationNodeName);
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
			GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnDialogueComplete));
			if (this.conversationStarter.VCam)
			{
				this.conversationStarter.VCam.gameObject.SetActive(true);
			}
			this.conversationStarter.OnConversationStarted();
		}
	}

	private void OnDialogueComplete()
	{
		GameManager.Instance.DialogueRunner.onDialogueComplete.RemoveListener(new UnityAction(this.OnDialogueComplete));
		GameManager.Instance.UI.HideDialogueView();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
		if (this.conversationStarter != null)
		{
			if (this.conversationStarter.VCam && this.conversationStarter.ReleaseCameraOnComplete)
			{
				this.conversationStarter.VCam.gameObject.SetActive(false);
			}
			this.conversationStarter.OnConversationCompleted();
		}
		base.StartCoroutine(this.DelayedInputReenable());
	}

	private IEnumerator DelayedInputReenable()
	{
		yield return new WaitForSeconds(0.25f);
		this.inspectAction.Enable();
		yield break;
	}

	private IConversationStarter conversationStarter;

	private DredgePlayerActionPress inspectAction;
}
