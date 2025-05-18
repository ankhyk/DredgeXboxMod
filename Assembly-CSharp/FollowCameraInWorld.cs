using System;
using UnityEngine;

public class FollowCameraInWorld : MonoBehaviour
{
	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (this.cameraRef == null)
		{
			this.cameraRef = Camera.main.transform;
		}
		if (this.cameraRef != null)
		{
			if (this.x)
			{
				this.followPosition.x = this.cameraRef.position.x;
			}
			if (this.y)
			{
				this.followPosition.y = this.cameraRef.position.y;
			}
			if (this.z)
			{
				this.followPosition.z = this.cameraRef.position.z;
			}
			this.transformToFollowCamera.position = this.followPosition;
			if (this.rotation)
			{
				this.transformToFollowCamera.rotation = this.cameraRef.rotation;
			}
		}
	}

	[SerializeField]
	private Transform transformToFollowCamera;

	[SerializeField]
	private bool x;

	[SerializeField]
	private bool y;

	[SerializeField]
	private bool z;

	[SerializeField]
	private bool rotation;

	private Transform cameraRef;

	private Vector3 followPosition = Vector3.zero;
}
