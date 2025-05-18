using System;
using UnityEngine;

public class FollowPlayerOverride : MonoBehaviour
{
	private void OnEnable()
	{
		this.followPlayer.SetOverride(base.transform);
	}

	private void OnDisable()
	{
		this.followPlayer.SetOverride(null);
	}

	[SerializeField]
	private FollowPlayerInWorld followPlayer;
}
