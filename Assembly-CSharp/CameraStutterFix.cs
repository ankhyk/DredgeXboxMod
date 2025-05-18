using System;
using Cinemachine;
using UnityEngine;

public class CameraStutterFix : MonoBehaviour
{
	[SerializeField]
	private Rigidbody playerRigidbody;

	private CinemachineBrain brain;

	private bool updateHasRun;

	private int frameCounter;

	private int frameCounterTarget = 10;
}
