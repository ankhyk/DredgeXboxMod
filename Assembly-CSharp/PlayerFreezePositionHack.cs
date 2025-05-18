using System;
using UnityEngine;

public class PlayerFreezePositionHack : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock)
		{
			this.isLocked = true;
			this.lockedPos = new Vector2(this.playerRef.transform.position.x, this.playerRef.transform.position.z);
			return;
		}
		this.isLocked = false;
	}

	private void LateUpdate()
	{
		if (this.isLocked)
		{
			this.playerRef.transform.position = new Vector3(this.lockedPos.x, this.playerRef.transform.position.y, this.lockedPos.y);
		}
	}

	[SerializeField]
	private Player playerRef;

	private bool isLocked;

	private Vector2 lockedPos;
}
