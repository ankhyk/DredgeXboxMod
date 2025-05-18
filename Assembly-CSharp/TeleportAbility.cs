using System;
using Cinemachine;
using UnityEngine;

public class TeleportAbility : Ability
{
	public override void Init()
	{
		base.Init();
		this.teleportVCam.enabled = false;
		this.teleportDestinationObject = GameObject.FindGameObjectWithTag("TeleportDestination");
	}

	public override bool Activate()
	{
		this.isActive = true;
		bool flag = true;
		GameManager.Instance.Player.PlayerTeleport.Teleport(this.teleportDestinationObject.transform.position, this.sanityChange, this.abilityData);
		if (Vector3.Distance(this.teleportDestinationObject.transform.position, GameManager.Instance.Player.transform.position) > this.achievementDistance)
		{
			GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.ABILITY_MANIFEST, true);
		}
		base.Activate();
		this.Deactivate();
		return flag;
	}

	[SerializeField]
	private CinemachineVirtualCamera teleportVCam;

	[SerializeField]
	private float preHoldTimeSec;

	[SerializeField]
	private float holdTimeSec;

	[SerializeField]
	private float sanityChange;

	[SerializeField]
	private float achievementDistance;

	[SerializeField]
	private GameObject effect;

	private GameObject teleportDestinationObject;
}
