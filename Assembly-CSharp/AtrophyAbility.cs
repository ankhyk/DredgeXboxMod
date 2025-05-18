using System;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class AtrophyAbility : Ability
{
	public override void Init()
	{
		base.Init();
	}

	public override bool Activate()
	{
		bool flag = false;
		HarvestPOI harvestPOI = null;
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, this.hitLayerMask);
		if (array.Length != 0)
		{
			float num = float.PositiveInfinity;
			for (int i = 0; i < array.Length; i++)
			{
				HarvestPOI component = array[i].gameObject.GetComponent<HarvestPOI>();
				if (component != null)
				{
					float num2 = Vector3.Distance(base.transform.position, component.transform.position);
					if (num2 < num && !component.IsCrabPotPOI && component.HarvestPOIData.GetHarvestableItemSubType() == ItemSubtype.FISH && component.HarvestPOIData.GetActiveFirstHarvestableItem() != null && component.HarvestPOIData.GetStockCount(false) >= 1f)
					{
						num = num2;
						harvestPOI = component;
					}
				}
			}
		}
		if (harvestPOI != null)
		{
			this.isActive = true;
			flag = true;
			for (int j = this.vCamTargetGroup.m_Targets.Length - 1; j >= 0; j--)
			{
				this.vCamTargetGroup.RemoveMember(this.vCamTargetGroup.m_Targets[j].target);
			}
			this.vCamTargetGroup.AddMember(GameManager.Instance.Player.transform, 1f, 0f);
			this.vCamTargetGroup.AddMember(harvestPOI.transform, 1f, 0f);
			this.playerVfx = global::UnityEngine.Object.Instantiate<GameObject>(this.playerVfxPrefab, base.transform.position, Quaternion.identity);
			this.spotVfx = global::UnityEngine.Object.Instantiate<GameObject>(this.spotVfxPrefab, harvestPOI.transform.position, Quaternion.identity);
			GameManager.Instance.Player.Harvester.CurrentHarvestPOI = harvestPOI;
			GameManager.Instance.Player.Harvester.IsAtrophyMode = true;
			GameManager.Instance.Player.Harvester.enabled = true;
			GameEvents.Instance.OnHarvestModeToggled += this.OnHarvestModeToggled;
			GameManager.Instance.Player.Sanity.ChangeSanity(this.sanityLossOnActivate);
			this.ToggleLoopAudio(true);
			GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, true);
			if (Vector3.Distance(base.transform.position, harvestPOI.transform.position) >= this.achievementDistance)
			{
				GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.ABILITY_ATROPHY, true);
			}
		}
		else
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.atrophy-failed");
		}
		base.Activate();
		this.Deactivate();
		return flag;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	private void OnHarvestModeToggled(bool enabled)
	{
		if (!enabled)
		{
			GameEvents.Instance.OnHarvestModeToggled -= this.OnHarvestModeToggled;
			this.ToggleLoopAudio(false);
		}
		SafeParticleDestroyer component = this.playerVfx.GetComponent<SafeParticleDestroyer>();
		SafeParticleDestroyer component2 = this.spotVfx.GetComponent<SafeParticleDestroyer>();
		if (component)
		{
			component.Destroy();
		}
		if (component2)
		{
			component2.Destroy();
		}
	}

	private void ToggleLoopAudio(bool enable)
	{
		if (enable)
		{
			DOTween.Kill(this.loopAudioSource, false);
			this.loopAudioSource.Play();
			this.loopAudioSource.DOFade(this.loopAudioMaxVolume, this.loopAudioFadeDuration);
			return;
		}
		DOTween.Kill(this.loopAudioSource, false);
		this.loopAudioSource.DOFade(0f, this.loopAudioFadeDuration).OnComplete(delegate
		{
			this.loopAudioSource.Stop();
		});
	}

	[SerializeField]
	private CinemachineTargetGroup vCamTargetGroup;

	[SerializeField]
	private float radius;

	[SerializeField]
	private float achievementDistance;

	[SerializeField]
	private LayerMask hitLayerMask;

	[SerializeField]
	private GameObject playerVfxPrefab;

	[SerializeField]
	private GameObject spotVfxPrefab;

	[SerializeField]
	private float sanityLossOnActivate;

	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private float loopAudioMaxVolume;

	[SerializeField]
	private float loopAudioFadeDuration;

	private GameObject playerVfx;

	private GameObject spotVfx;
}
