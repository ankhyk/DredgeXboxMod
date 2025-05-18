using System;
using UnityEngine;

public class TrawlResetter : MonoBehaviour
{
	private void Start()
	{
		this.initialPosition = this.net.transform.localPosition;
		this.initialRotation = this.net.transform.localEulerAngles;
	}

	private void OnDisable()
	{
		this.net.velocity = Vector3.zero;
		this.net.transform.localEulerAngles = this.initialRotation;
		this.net.transform.localPosition = this.initialPosition;
	}

	[SerializeField]
	private Rigidbody net;

	private Vector3 initialPosition;

	private Vector3 initialRotation;
}
