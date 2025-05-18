using System;
using UnityEngine;

public class NetTabShowQuery : TabShowQuery
{
	private void OnEnable()
	{
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
		this.cachedCanNavigate = this.GetCanNavigate();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
	}

	private void OnItemInventoryChanged(SpatialItemData itemData)
	{
		if (itemData && itemData.itemSubtype == ItemSubtype.NET)
		{
			this.testCanNavigate = this.GetCanNavigate();
			if (this.testCanNavigate != this.cachedCanNavigate)
			{
				this.cachedCanNavigate = this.testCanNavigate;
				Action<bool> canNavigateChanged = this.canNavigateChanged;
				if (canNavigateChanged == null)
				{
					return;
				}
				canNavigateChanged(this.cachedCanNavigate);
			}
		}
	}

	public override bool GetCanNavigate()
	{
		return GameManager.Instance.SaveData.EquippedTrawlNetInstance() != null;
	}

	public override bool GetCanShow()
	{
		return GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked(this.dependentAbility);
	}

	[SerializeField]
	private AbilityData dependentAbility;

	private bool cachedCanNavigate;

	private bool testCanNavigate;
}
