using System;
using UnityEngine;

public class GhostRock : MonoBehaviour
{
	public GameObject RockMeshObject
	{
		get
		{
			return this.rockMeshObject;
		}
	}

	public Renderer RockRenderer
	{
		get
		{
			return this.rockRenderer;
		}
	}

	public bool IsShowing { get; set; }

	public float DistanceFromPlayer { get; set; }

	public float SanityThreshold { get; set; }

	[SerializeField]
	private GameObject rockMeshObject;

	[SerializeField]
	private Renderer rockRenderer;
}
