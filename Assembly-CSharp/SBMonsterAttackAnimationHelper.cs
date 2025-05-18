using System;
using UnityEngine;

public class SBMonsterAttackAnimationHelper : MonoBehaviour
{
	public void PlayPreAttackSFX()
	{
		this.sbMonsterMain.PlayPreAttackSFX();
	}

	public void PlayAttackSFX()
	{
		this.sbMonsterMain.PlayAttackSFX();
	}

	public void OnAttackComplete()
	{
		this.sbMonsterMain.OnAttackComplete();
	}

	[SerializeField]
	private SBMonsterAnimationHelper sbMonsterMain;
}
