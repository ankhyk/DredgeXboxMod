using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "HullUpgradeData", menuName = "Dredge/UpgradeData/HullUpgradeData", order = 0)]
public class HullUpgradeData : UpgradeData
{
	public override int GetNewCellCount()
	{
		return this.newCellCount;
	}

	public GridConfiguration hullGridConfiguration;

	public AssetReference engineAudioClip;

	public int newCellCount;

	public Sprite hullSprite;
}
