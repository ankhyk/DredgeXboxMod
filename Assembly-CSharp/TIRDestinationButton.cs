using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class TIRDestinationButton : DestinationButton
{
	public SelectableWithNavigationPriority GetSelectableWithNavigationPriority()
	{
		return base.BasicButtonWrapper.Button as SelectableWithNavigationPriority;
	}

	public override void Init(BaseDestination destination)
	{
		if (destination.useFixedScreenPosition)
		{
			(base.transform as RectTransform).anchoredPosition = destination.screenPosition;
			this.uiLineRenderer.Points[0] = this.point0;
			this.uiLineRenderer.Points[1] = this.point1;
			if (destination.screenPosition.x > 0f)
			{
				Vector2[] points = this.uiLineRenderer.Points;
				int num = 1;
				points[num].x = points[num].x * -1f;
			}
			this.uiLineRenderer.SetVerticesDirty();
		}
		if (destination is ConstructableDestination)
		{
			ConstructableDestinationData constructableDestinationData = (destination as ConstructableDestination).constructableDestinationData;
			if (GameManager.Instance.ConstructableBuildingManager)
			{
				int num2 = 0;
				bool flag = false;
				for (int i = 0; i < constructableDestinationData.tiers.Count; i++)
				{
					BaseDestinationTier baseDestinationTier = constructableDestinationData.tiers[i];
					if (GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(baseDestinationTier.tierId))
					{
						num2 = i + 1;
					}
					if (!flag && GameManager.Instance.ConstructableBuildingManager.GetCanBuildingBeConstructed(baseDestinationTier.tierId))
					{
						flag = true;
					}
				}
				this.upgradeAvailableMarker.SetActive(flag);
				string text = "";
				if (num2 == 0)
				{
					text = "-";
				}
				else if (num2 == 4)
				{
					text = "IV";
				}
				else
				{
					for (int j = 0; j < num2; j++)
					{
						text += "I";
					}
				}
				this.upgradeLevelText.text = text;
			}
		}
		else
		{
			this.upgradeAvailableMarker.SetActive(false);
			this.upgradeLevelContainer.SetActive(false);
		}
		this.uiLineRenderer.gameObject.SetActive(destination.useFixedScreenPosition);
		this.lineEndCap.gameObject.SetActive(destination.useFixedScreenPosition);
		base.Init(destination);
	}

	private void UpdateLine(Transform transformToPointTo)
	{
		this.endPos = Camera.main.WorldToScreenPoint(transformToPointTo.position);
		this.endPos.x = this.endPos.x - this.uiLineRenderer.rectTransform.position.x;
		this.endPos.y = this.endPos.y - this.uiLineRenderer.rectTransform.position.y;
		this.endPos /= GameManager.Instance.ScaleFactor;
		this.uiLineRenderer.Points[2] = this.endPos;
		this.uiLineRenderer.SetVerticesDirty();
		this.lineEndCap.transform.position = Camera.main.WorldToScreenPoint(transformToPointTo.position);
	}

	private void LateUpdate()
	{
		this.UpdateLine(this.destination.transformToPointTo);
	}

	[SerializeField]
	private UILineRenderer uiLineRenderer;

	[SerializeField]
	private GameObject lineEndCap;

	[SerializeField]
	private Vector2 point0;

	[SerializeField]
	private Vector2 point1;

	[SerializeField]
	private GameObject upgradeAvailableMarker;

	[SerializeField]
	private GameObject upgradeLevelContainer;

	[SerializeField]
	private TextMeshProUGUI upgradeLevelText;

	private Vector2 endPos;
}
