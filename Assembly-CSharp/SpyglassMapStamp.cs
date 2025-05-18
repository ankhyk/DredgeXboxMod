using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpyglassMapStamp : MonoBehaviour
{
	public string HarvestPOIID
	{
		get
		{
			return this.harvestPOIID;
		}
		set
		{
			this.harvestPOIID = value;
		}
	}

	public void Init(Sprite sprite, Color color, SerializedMapStamp stampData)
	{
		this.stampImage.sprite = sprite;
		this.stampImage.color = color;
		this.stampImage.material = null;
		this.outline.enabled = false;
		this.worldPosition = new Vector3(stampData.x, 0f, stampData.z);
		this.harvestPOIID = "";
	}

	public void Init(HarvestPOI harvestPOI)
	{
		HarvestableItemData firstHarvestableItem = harvestPOI.Harvestable.GetFirstHarvestableItem();
		this.stampImage.sprite = firstHarvestableItem.sprite;
		this.stampImage.material = this.silhouetteMaterial;
		this.harvestPOIID = harvestPOI.Harvestable.GetId();
		Color color;
		if (this.harvestTypeTagConfig.colorLookup.TryGetValue(firstHarvestableItem.harvestableType, out color))
		{
			this.outline.effectColor = color;
			this.outline.enabled = true;
		}
		this.worldPosition = new Vector3(harvestPOI.transform.position.x, 0f, harvestPOI.transform.position.z);
	}

	private void LateUpdate()
	{
		this.rectTransform.position = Camera.main.WorldToScreenPoint(this.worldPosition);
		this.isOnScreen = this.rectTransform.position.z > 0f;
		this.stampImage.enabled = this.isOnScreen;
		this.distanceTextField.enabled = this.isOnScreen;
		if (!this.isOnScreen)
		{
			return;
		}
		this.magnitude = this.rectTransform.anchoredPosition.magnitude;
		this.rectTransform.anchoredPosition = Vector3.ClampMagnitude(this.rectTransform.anchoredPosition, this.radius);
		this.canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0f, this.radius * 2f, this.magnitude - this.radius));
		this.timeUntilDistanceUpdate -= Time.deltaTime;
		if (this.timeUntilDistanceUpdate < 0f)
		{
			this.timeUntilDistanceUpdate = this.timeBetweenDistanceUpdates;
			this.UpdatePlayerDistance();
		}
	}

	private void UpdatePlayerDistance()
	{
		float num = Vector3.Distance(this.worldPosition, GameManager.Instance.Player.transform.position);
		this.distanceTextField.text = GameManager.Instance.LanguageManager.FormatLongDistanceString(num);
		float num2 = Mathf.Lerp(this.smallScale, this.regularScale, Mathf.InverseLerp(this.farDistance, this.closeDistance, num));
		this.rectTransform.localScale = new Vector3(num2, num2, num2);
	}

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float radius;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Image stampImage;

	[SerializeField]
	private TextMeshProUGUI distanceTextField;

	[SerializeField]
	private float smallScale;

	[SerializeField]
	private float regularScale;

	[SerializeField]
	private float closeDistance;

	[SerializeField]
	private float farDistance;

	[SerializeField]
	private Material silhouetteMaterial;

	[SerializeField]
	private Outline outline;

	[SerializeField]
	private HarvestTypeTagConfig harvestTypeTagConfig;

	private string harvestPOIID;

	private float magnitude;

	private float timeUntilDistanceUpdate = -1f;

	private float timeBetweenDistanceUpdates = 0.5f;

	private bool isOnScreen;

	private Vector3 worldPosition;
}
