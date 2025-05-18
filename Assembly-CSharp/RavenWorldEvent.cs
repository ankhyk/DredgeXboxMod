using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RavenWorldEvent : WorldEvent
{
	public override void Activate()
	{
		this.timeOfLastRavenAttack = Time.time;
		base.transform.SetParent(GameManager.Instance.Player.transform, true);
	}

	private void Update()
	{
		if (!this.finishRequested && Time.time > this.timeOfLastRavenAttack + this.ravenAttackDelay)
		{
			this.numAttacksMade += 1f;
			this.timeOfLastRavenAttack = Time.time;
			this.ravenAnimator.SetTrigger("attack");
			if (this.numAttacksMade >= (float)this.numAttacksToMake)
			{
				this.RequestEventFinish();
			}
		}
	}

	public void OnRavenAttackBegin()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackSFX[global::UnityEngine.Random.Range(0, this.attackSFX.Length)], AudioLayer.SFX_WORLD, 1f, 1f);
	}

	public void OnRavenAttackComplete()
	{
		if (GameManager.Instance.Player.IsDocked)
		{
			return;
		}
		List<FishItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH);
		FishItemInstance chosenFish = null;
		allItemsOfType.ForEach(delegate(FishItemInstance fish)
		{
			if (chosenFish == null && !fish.IsAberrant() && fish.GetItemData<SpatialItemData>().dimensions.Count <= (int)this.stealableFishSizeThreshold)
			{
				chosenFish = fish;
			}
		});
		if (chosenFish == null)
		{
			this.RequestEventFinish();
			return;
		}
		GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(chosenFish, true);
		GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_REMOVED, "notification.raven-event-fish-lost", chosenFish.GetItemData<SpatialItemData>().itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
		GameManager.Instance.VibrationManager.Vibrate(this.stealFishVibration, VibrationRegion.WholeBody, true).Run();
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.swirlingRavenParticleSystem.Stop();
			this.audioSource.DOFade(0f, this.finishDelaySec);
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.finishDelaySec);
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private ParticleSystem swirlingRavenParticleSystem;

	[SerializeField]
	private float finishDelaySec;

	[SerializeField]
	private Animator ravenAnimator;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float ravenAttackDelay;

	[SerializeField]
	private int numAttacksToMake;

	[SerializeField]
	private AssetReference[] attackSFX;

	[SerializeField]
	private short stealableFishSizeThreshold;

	private float timeOfLastRavenAttack;

	private float numAttacksMade;

	[SerializeField]
	private VibrationData stealFishVibration;
}
