using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStateResponder : MonoBehaviour
{
	private void Awake()
	{
		this.RefreshState();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnBuildingConstructed += this.OnBuildingConstructed;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnBuildingConstructed -= this.OnBuildingConstructed;
	}

	private void OnBuildingConstructed(BuildingTierId constructedTierId)
	{
		if (this.tierId == constructedTierId)
		{
			this.RefreshState();
		}
	}

	private void RefreshState()
	{
		if (GameManager.Instance.ConstructableBuildingManager == null)
		{
			return;
		}
		bool isBuilt = GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(this.tierId);
		this.mainModels.ForEach(delegate(GameObject m)
		{
			m.SetActive(isBuilt);
		});
	}

	[SerializeField]
	private BuildingTierId tierId;

	[SerializeField]
	private List<GameObject> mainModels;
}
