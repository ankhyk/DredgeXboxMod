using System;
using UnityEngine;

public class FollowPlayerInWorld : MonoBehaviour
{
	private void Update()
	{
		if (this.playerRef == null && GameManager.Instance && GameManager.Instance.Player)
		{
			this.playerRef = GameManager.Instance.Player.transform;
		}
		if (this.playerRef != null)
		{
			if (this.x)
			{
				this.followPosition.x = this.playerRef.position.x;
			}
			if (this.y)
			{
				this.followPosition.y = this.playerRef.position.y;
			}
			if (this.z)
			{
				this.followPosition.z = this.playerRef.position.z;
			}
			this.transformToFollowPlayer.position = this.followPosition;
			if (this.rotation)
			{
				this.transformToFollowPlayer.rotation = this.playerRef.rotation;
			}
		}
	}

	public void SetOverride(Transform overrideTransform)
	{
		if (overrideTransform)
		{
			this.playerRef = overrideTransform;
			return;
		}
		this.playerRef = GameManager.Instance.Player.transform;
	}

	[SerializeField]
	private Transform transformToFollowPlayer;

	[SerializeField]
	private bool x;

	[SerializeField]
	private bool y;

	[SerializeField]
	private bool z;

	[SerializeField]
	private bool rotation;

	private Transform playerRef;

	private Vector3 followPosition = Vector3.zero;
}
