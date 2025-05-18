using System;
using UnityEngine;

public class OceanMover : MonoBehaviour
{
	private void Update()
	{
		if (this.playerRef == null && GameManager.Instance && GameManager.Instance.Player)
		{
			this.playerRef = GameManager.Instance.Player.transform;
		}
		if (this.cameraRef == null)
		{
			this.cameraRef = Camera.main.transform;
		}
		this.transformToFollow = (this.followCameraOverride ? this.cameraRef : this.playerRef);
		if (this.transformToFollow != null)
		{
			float num = this.transformToFollow.position.x - this.playerPositionLast.x;
			float num2 = this.transformToFollow.position.z - this.playerPositionLast.z;
			if (Mathf.Sqrt(num * num + num2 * num2) > this.distanceBetweenUpdates)
			{
				this.followPosition.x = this.RoundTo(this.transformToFollow.position.x, this.distanceBetweenUpdates);
				this.followPosition.z = this.RoundTo(this.transformToFollow.position.z, this.distanceBetweenUpdates);
				base.transform.position = this.followPosition;
				this.playerPositionLast = this.transformToFollow.position;
			}
		}
	}

	private float RoundTo(float value, float multipleOf)
	{
		return Mathf.Round(value / multipleOf) * multipleOf;
	}

	[SerializeField]
	private float distanceBetweenUpdates = 128f;

	public bool followCameraOverride;

	private Transform transformToFollow;

	private Transform playerRef;

	private Transform cameraRef;

	private Vector3 followPosition = Vector3.zero;

	private Vector3 playerPositionLast = Vector3.zero;
}
