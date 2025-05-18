using System;
using UnityEngine;

public class BaitHarvestPOI : HarvestPOI
{
	private void Awake()
	{
		if (this.animator)
		{
			this.animator.SetTrigger("deploy");
		}
	}

	[SerializeField]
	private Animator animator;
}
