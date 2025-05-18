using System;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNotch : MonoBehaviour
{
	private void OnEnable()
	{
		this.fillImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
	}

	public void SetFilled(bool filled)
	{
		this.animator.Play(filled ? "IdleFull" : "IdleEmpty");
	}

	public void AnimateFill()
	{
		this.animator.SetTrigger("fill");
	}

	public void OnAnimationComplete()
	{
		Action onAnimationCompleteAction = this.OnAnimationCompleteAction;
		if (onAnimationCompleteAction == null)
		{
			return;
		}
		onAnimationCompleteAction();
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image fillImage;

	[SerializeField]
	private Animator animator;

	public Action OnAnimationCompleteAction;
}
