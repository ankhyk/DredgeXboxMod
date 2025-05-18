using System;
using System.Collections.Generic;
using UnityEngine;

public class BannersUI : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnItemSeen += this.OnItemSeen;
		GameEvents.Instance.OnTrophyFishCaught += this.OnTrophyFishCaught;
		GameEvents.Instance.OnResearchCompleted += this.OnResearchCompleted;
		GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		GameEvents.Instance.OnBookReadCompleted += this.OnBookReadCompleted;
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		BannerUI bannerUI = this.bannerUI;
		bannerUI.OnHideComplete = (Action)Delegate.Combine(bannerUI.OnHideComplete, new Action(this.ProcessListing));
		this.ProcessListing();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemSeen -= this.OnItemSeen;
		GameEvents.Instance.OnTrophyFishCaught -= this.OnTrophyFishCaught;
		GameEvents.Instance.OnResearchCompleted -= this.OnResearchCompleted;
		GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
		GameEvents.Instance.OnBookReadCompleted -= this.OnBookReadCompleted;
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
		BannerUI bannerUI = this.bannerUI;
		bannerUI.OnHideComplete = (Action)Delegate.Remove(bannerUI.OnHideComplete, new Action(this.ProcessListing));
	}

	private void AddNewListing(BannersUI.BannerListing bannerListing)
	{
		this.queue.Enqueue(bannerListing);
		if (this.queue.Count == 1 && !this.bannerUI.isShowing)
		{
			this.ProcessListing();
		}
	}

	private void ProcessListing()
	{
		if (this.queue.Count > 0 && GameManager.Instance.Player.IsAlive)
		{
			BannersUI.BannerListing bannerListing = this.queue.Dequeue();
			if (bannerListing != null)
			{
				RectTransform rectTransform = this.bannerUI.transform as RectTransform;
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, (GameManager.Instance.UI.ShowingWindowTypes.Count == 0) ? this.topYPos : this.bottomYPos);
				this.timeUntilHideBanner = bannerListing.holdTimeSec;
				bannerListing.ProcessListing(this.bannerUI);
			}
		}
	}

	private void Update()
	{
		if (this.bannerUI.isShowing)
		{
			this.timeUntilHideBanner -= Time.unscaledDeltaTime;
			if (this.timeUntilHideBanner <= 0f && !this.bannerUI.isHiding)
			{
				this.bannerUI.RequestHide();
			}
		}
	}

	private void OnItemSeen(SpatialItemInstance spatialItemInstance)
	{
		if (spatialItemInstance is FishItemInstance)
		{
			if (GameManager.Instance.SaveData.GetCaughtCountById(spatialItemInstance.id) == 0)
			{
				GameManager.Instance.SaveData.LastUnseenCaughtSpecies = spatialItemInstance.id;
				GameEvents.Instance.TriggerHasUnseenItemsChanged();
				this.AddNewListing(new BannersUI.FishDiscoveredBannerListing
				{
					fishItemData = spatialItemInstance.GetItemData<FishItemData>()
				});
				return;
			}
		}
		else if (spatialItemInstance.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.RELIC)
		{
			this.AddNewListing(new BannersUI.RelicBannerListing
			{
				spatialItemData = spatialItemInstance.GetItemData<SpatialItemData>()
			});
		}
	}

	private void OnTrophyFishCaught(FishItemInstance fishItemInstance)
	{
		this.AddNewListing(new BannersUI.FishTrophyBannerListing
		{
			fishItemData = fishItemInstance.GetItemData<FishItemData>(),
			size = fishItemInstance.size
		});
	}

	private void OnResearchCompleted(SpatialItemData spatialItemData)
	{
		if (spatialItemData)
		{
			this.AddNewListing(new BannersUI.ResearchBannerListing
			{
				spatialItemData = spatialItemData
			});
		}
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		this.AddNewListing(new BannersUI.AbilityBannerListing
		{
			abilityData = abilityData
		});
	}

	private void OnBookReadCompleted(ResearchableItemInstance bookInstance)
	{
		this.AddNewListing(new BannersUI.BookBannerListing
		{
			researchableItemData = bookInstance.GetItemData<ResearchableItemData>()
		});
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		this.AddNewListing(new BannersUI.UpgradeBannerListing
		{
			upgradeData = upgradeData
		});
	}

	[SerializeField]
	private BannerUI bannerUI;

	[SerializeField]
	private float topYPos;

	[SerializeField]
	private float bottomYPos;

	private Queue<BannersUI.BannerListing> queue = new Queue<BannersUI.BannerListing>();

	private float timeUntilHideBanner;

	private abstract class BannerListing
	{
		public abstract void ProcessListing(BannerUI bannerUI);

		public float holdTimeSec = 5f;
	}

	private class FishDiscoveredBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowFishDiscovered(this.fishItemData);
		}

		public FishItemData fishItemData;
	}

	private class FishTrophyBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowFishTrophy(this.fishItemData, this.size);
		}

		public FishItemData fishItemData;

		public float size;
	}

	private class AbilityBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowAbility(this.abilityData);
		}

		public AbilityData abilityData;
	}

	private class ResearchBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowResearch(this.spatialItemData);
		}

		public SpatialItemData spatialItemData;
	}

	private class RelicBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowRelic(this.spatialItemData);
		}

		public SpatialItemData spatialItemData;
	}

	private class BookBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowBook(this.researchableItemData);
		}

		public ResearchableItemData researchableItemData;
	}

	private class UpgradeBannerListing : BannersUI.BannerListing
	{
		public override void ProcessListing(BannerUI bannerUI)
		{
			bannerUI.ShowUpgrade(this.upgradeData);
		}

		public UpgradeData upgradeData;
	}
}
