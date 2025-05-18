using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostRockManager : MonoBehaviour
{
	private void FindRocks()
	{
		this.allGhostRocks = global::UnityEngine.Object.FindObjectsOfType<GhostRock>().ToList<GhostRock>();
	}

	private void Update()
	{
		if (!this.playerRef)
		{
			this.playerRef = GameManager.Instance.Player;
		}
		if (!this.playerRef)
		{
			return;
		}
		if (Time.time > this.timeOfLastSanityAssignment + this.timeBetweenSanityAssignments)
		{
			this.timeOfLastSanityAssignment = Time.time;
			for (int i = 0; i < this.allGhostRocks.Count; i++)
			{
				this.allGhostRocks[i].SanityThreshold = global::UnityEngine.Random.Range(this.ghostRockConfig.sanityThresholdMin, this.ghostRockConfig.sanityThresholdMax);
			}
		}
		float currentSanity = this.playerRef.Sanity.CurrentSanity;
		int num = this.rockCheckIndex;
		while (num < this.rockCheckIndex + this.rocksToCheckPerFrame && num < this.allGhostRocks.Count)
		{
			this.UpdateRock(this.allGhostRocks[num], currentSanity);
			this.rockCheckIndex = num;
			num++;
		}
		if (this.rockCheckIndex >= this.allGhostRocks.Count - 1)
		{
			this.rockCheckIndex = 0;
		}
	}

	private void UpdateRock(GhostRock ghostRock, float currentSanity)
	{
		this.canBeSeenHelperVar = true;
		if (this.canBeSeenHelperVar && currentSanity > ghostRock.SanityThreshold)
		{
			this.canBeSeenHelperVar = false;
		}
		if (this.canBeSeenHelperVar && GameManager.Instance.Time.Time < this.ghostRockConfig.spawnStartTime && GameManager.Instance.Time.Time > this.ghostRockConfig.spawnEndTime)
		{
			this.canBeSeenHelperVar = false;
		}
		if (this.canBeSeenHelperVar)
		{
			this.distanceHelperVar = Vector3.Distance(ghostRock.transform.position, this.playerRef.transform.position);
			if (!ghostRock.IsShowing && this.distanceHelperVar < this.ghostRockConfig.minDistanceThreshold)
			{
				this.canBeSeenHelperVar = false;
			}
			if (this.distanceHelperVar > this.ghostRockConfig.maxDistanceThreshold)
			{
				this.canBeSeenHelperVar = false;
			}
		}
		if (!ghostRock.IsShowing && this.canBeSeenHelperVar)
		{
			ghostRock.RockMeshObject.SetActive(true);
			ghostRock.IsShowing = true;
		}
		if (ghostRock.IsShowing && !this.canBeSeenHelperVar && !ghostRock.RockRenderer.isVisible && ghostRock.IsShowing && !this.canBeSeenHelperVar)
		{
			ghostRock.RockMeshObject.SetActive(false);
			ghostRock.IsShowing = false;
		}
	}

	[SerializeField]
	private List<GhostRock> allGhostRocks;

	[SerializeField]
	private GhostRockConfig ghostRockConfig;

	[SerializeField]
	private float timeBetweenSanityAssignments;

	[SerializeField]
	private int rocksToCheckPerFrame;

	private int rockCheckIndex;

	private Player playerRef;

	private float distanceHelperVar;

	private bool canBeSeenHelperVar;

	private float timeOfLastSanityAssignment = float.NegativeInfinity;
}
