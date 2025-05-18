using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class InteractPointUI : MonoBehaviour
{
	private void Awake()
	{
		this.materialCopy = new Material(this.material);
		this.frameSpriteRenderer.material = this.materialCopy;
		this.iconSpriteRenderer.material = this.materialCopy;
		GameManager.Instance.UI.InteractPointUI = this;
		this.UpdateAnim();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerInteractedWithPOI += this.Hide;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerInteractedWithPOI -= this.Hide;
	}

	public void SetCurrentPOI(POI poi)
	{
		if (this.currentPOI == poi)
		{
			return;
		}
		this.currentPOI = poi;
		if (this.currentPOI == null)
		{
			this.Hide();
			return;
		}
		this.Show(this.currentPOI);
	}

	public void Hide()
	{
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = DOTween.To(() => this.prop, delegate(float x)
		{
			this.prop = x;
		}, 0f, this.fadeDurationSec).OnComplete(new TweenCallback(this.OnHideComplete)).OnUpdate(new TweenCallback(this.UpdateAnim));
		this.fadeTween.SetEase(Ease.OutExpo);
		this.currentPOI = null;
	}

	public void Show(POI poi)
	{
		if (!InteractPointUI.showInteractIcon)
		{
			return;
		}
		if (GameManager.Instance.Player.IsDocked)
		{
			return;
		}
		if (poi.InteractPointTargetTransform == null)
		{
			base.transform.position = poi.transform.position;
		}
		else
		{
			base.transform.position = poi.InteractPointTargetTransform.position;
		}
		if (poi is HarvestPOI)
		{
			HarvestPOI harvestPOI = poi as HarvestPOI;
			if (harvestPOI.IsCrabPotPOI)
			{
				this.iconSpriteRenderer.sprite = this.crabSprite;
			}
			else
			{
				HarvestableType harvestType = harvestPOI.HarvestPOIData.GetHarvestType();
				bool isAdvancedHarvestType = harvestPOI.HarvestPOIData.GetIsAdvancedHarvestType();
				if (GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(harvestType, isAdvancedHarvestType))
				{
					if (harvestType == HarvestableType.DREDGE)
					{
						this.iconSpriteRenderer.sprite = this.dredgeSprite;
					}
					else
					{
						this.iconSpriteRenderer.sprite = this.fishSprite;
					}
				}
				else
				{
					this.iconSpriteRenderer.sprite = this.unavailableSprite;
				}
			}
		}
		else if (poi is ItemPOI)
		{
			this.iconSpriteRenderer.sprite = this.itemSprite;
		}
		else if (poi is DockPOI)
		{
			this.iconSpriteRenderer.sprite = this.dockSprite;
		}
		else if (poi is TeleportAnchorPOI)
		{
			this.iconSpriteRenderer.sprite = this.teleportAnchorSprite;
		}
		else if (poi is ConversationPOI)
		{
			this.iconSpriteRenderer.sprite = this.inspectSprite;
		}
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = DOTween.To(() => this.prop, delegate(float x)
		{
			this.prop = x;
		}, 1f, this.fadeDurationSec).OnComplete(new TweenCallback(this.OnShowComplete)).OnUpdate(new TweenCallback(this.UpdateAnim));
		this.fadeTween.SetEase(Ease.OutExpo);
		this.container.SetActive(true);
	}

	private void UpdateAnim()
	{
		this.materialCopy.SetFloat("_Alpha", this.prop);
		float num = Mathf.Lerp(this.disappearY, this.appearY, this.prop) + base.transform.position.y;
		this.subContainer.transform.position = new Vector3(this.subContainer.transform.position.x, num, this.subContainer.transform.position.z);
	}

	private void OnHideComplete()
	{
		this.UpdateAnim();
		this.fadeTween = null;
		this.container.SetActive(false);
	}

	private void OnShowComplete()
	{
		this.UpdateAnim();
		this.fadeTween = null;
	}

	public static bool showInteractIcon = true;

	[SerializeField]
	private SpriteRenderer iconSpriteRenderer;

	[SerializeField]
	private SpriteRenderer frameSpriteRenderer;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject subContainer;

	[SerializeField]
	private Material material;

	[SerializeField]
	private float appearY;

	[SerializeField]
	private float disappearY;

	[SerializeField]
	private float fadeDurationSec;

	[SerializeField]
	private Sprite dockSprite;

	[SerializeField]
	private Sprite fishSprite;

	[SerializeField]
	private Sprite dredgeSprite;

	[SerializeField]
	private Sprite crabSprite;

	[SerializeField]
	private Sprite inspectSprite;

	[SerializeField]
	private Sprite itemSprite;

	[SerializeField]
	private Sprite unavailableSprite;

	[SerializeField]
	private Sprite teleportAnchorSprite;

	private Material materialCopy;

	private POI currentPOI;

	private Tweener fadeTween;

	private float prop;
}
