using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class IntroIllustratedCutscene : MonoBehaviour
{
	private void Start()
	{
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.SYSTEM);
		this.isComplete = false;
		base.StartCoroutine(this.DelayedAddSkip());
	}

	private IEnumerator DelayedAddSkip()
	{
		yield return new WaitForSeconds(2f);
		this.skipAction = new DredgePlayerActionHold("prompt.skip", GameManager.Instance.Input.Controls.Skip, 2f);
		this.skipAction.showInControlArea = true;
		this.skipAction.allowPreholding = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.skipAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnCutsceneSkipped));
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.skipAction };
		input.AddActionListener(array, ActionLayer.SYSTEM);
		yield break;
	}

	public void OnCutsceneComplete()
	{
		this.DoCutsceneComplete();
	}

	public void OnCutsceneSkipped()
	{
		this.DoCutsceneComplete();
	}

	private void DoCutsceneComplete()
	{
		if (!this.isComplete)
		{
			this.isComplete = true;
			this.audioSource.DOFade(0f, this.audioFadeOutDuration);
			GameManager.Instance.ChromaManager.StopAllAnimations();
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.skipAction };
			input.RemoveActionListener(array, ActionLayer.SYSTEM);
			GameManager.Instance.SaveData.HasViewedIntroCutscene = true;
			GameManager.Instance.Loader.LoadGameFromCutscene();
		}
	}

	public void OnCutsceneStage1Begin()
	{
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.INTRO_1);
	}

	public void OnCutsceneStage2Begin()
	{
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.INTRO_2);
	}

	public void OnCutsceneStage3Begin()
	{
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.INTRO_3);
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float audioFadeOutDuration;

	private DredgePlayerActionHold skipAction;

	private bool isComplete;
}
