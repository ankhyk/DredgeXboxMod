using System;
using UnityEngine;

public class NarwhalIceWallAnimator : MonoBehaviour
{
	public void NarwhalIceWallBreakStart()
	{
		this.iceWallAnimator.SetTrigger("break");
		IceMonsterSpawner.SetDelayForIceMonsterSpawn(0.5f);
	}

	[SerializeField]
	private Animator iceWallAnimator;
}
