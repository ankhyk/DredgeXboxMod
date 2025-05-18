using System;
using UnityEngine;

public class SteeringAnimator : MonoBehaviour
{
	private void OnEnable()
	{
		this.playerController = GameManager.Instance.Player.Controller;
	}

	private void Update()
	{
		if (this.moveAction == null)
		{
			this.moveAction = GameManager.Instance.Player.Controller.MoveAction;
			return;
		}
		this.moveValX = 0f;
		this.moveValY = 0f;
		if (this.playerController.IsMovementAllowed && this.playerController.AutoMoveTarget == null)
		{
			this.moveValX = this.moveAction.Value.x;
			this.moveValY = this.moveAction.Value.y;
		}
		this.lerpedMoveValY = Mathf.Lerp(this.lerpedMoveValY, this.moveValY, Time.deltaTime * 4f);
		if (this.moveValY < 0f)
		{
			this.moveValX *= -1f;
		}
		for (int i = 0; i < this.rudders.Length; i++)
		{
			Vector3 vector = new Vector3(0f, this.rudderMaxTurnDegrees * -this.moveValX, 0f);
			this.rudders[i].localRotation = Quaternion.Lerp(this.rudders[i].localRotation, Quaternion.Euler(vector), Time.deltaTime * 10f);
		}
		for (int j = 0; j < this.propellers.Length; j++)
		{
			this.propellers[j].transform.Rotate(this.propellorMaxTurnSpeed * this.lerpedMoveValY * Time.deltaTime, Space.Self);
		}
	}

	[SerializeField]
	private Transform[] rudders;

	[SerializeField]
	private Transform[] propellers;

	[SerializeField]
	private float rudderMaxTurnDegrees = 50f;

	[SerializeField]
	private Vector3 propellorMaxTurnSpeed = new Vector3(0f, 0f, 50f);

	private DredgePlayerActionTwoAxis moveAction;

	private PlayerController playerController;

	private float moveValX;

	private float moveValY;

	private float lerpedMoveValY;
}
