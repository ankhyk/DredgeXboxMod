using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerColorCustomizer : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnBoatColorsChanged += this.OnBoatColorChanged;
		GameEvents.Instance.OnBoatBuntingChanged += this.OnBoatBuntingChanged;
		GameEvents.Instance.OnBoatFlagChanged += this.OnBoatFlagChanged;
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		this.RefreshBoatColors();
		this.RefreshBoatBunting();
		this.RefreshBoatFlag();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnBoatColorsChanged -= this.OnBoatColorChanged;
		GameEvents.Instance.OnBoatBuntingChanged -= this.OnBoatBuntingChanged;
		GameEvents.Instance.OnBoatFlagChanged -= this.OnBoatFlagChanged;
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		if (upgradeData is HullUpgradeData && !GameManager.Instance.SaveData.HasChangedBoatColors)
		{
			GameManager.Instance.SaveData.RoofColorIndex = this.defaultRoofColorIndices[upgradeData.tier];
			GameManager.Instance.SaveData.HullColorIndex = this.defaultHullColorIndices[upgradeData.tier];
			this.RefreshBoatColors();
		}
	}

	private void OnBoatColorChanged(int area, int index)
	{
		this.RefreshBoatColors();
	}

	private void OnBoatFlagChanged(int index)
	{
		this.RefreshBoatFlag();
	}

	private void OnBoatBuntingChanged(bool enabled)
	{
		this.RefreshBoatBunting();
	}

	private void RefreshBoatColors()
	{
		int hullTier = GameManager.Instance.SaveData.HullTier;
		int roofColorIndex = GameManager.Instance.SaveData.RoofColorIndex;
		int hullColorIndex = GameManager.Instance.SaveData.HullColorIndex;
		this.meshRenderers.ForEach(delegate(MeshRenderer meshRenderer)
		{
			meshRenderer.materials[0].SetColor("_Roof_Color", this.roofColors[roofColorIndex]);
			meshRenderer.materials[0].SetColor("_Hull_Color", this.hullColors[hullColorIndex]);
		});
		this.buntingMeshRenderers.ForEach(delegate(SkinnedMeshRenderer meshRenderer)
		{
			meshRenderer.materials[0].SetColor("_Roof_Color", this.roofColors[roofColorIndex]);
			meshRenderer.materials[0].SetColor("_Hull_Color", this.hullColors[hullColorIndex]);
		});
	}

	private void RefreshBoatBunting()
	{
		bool enabled = GameManager.Instance.SaveData.IsBoatBuntingEnabled;
		this.buntingObjects.ForEach(delegate(GameObject buntingObject)
		{
			buntingObject.SetActive(enabled);
		});
	}

	private void RefreshBoatFlag()
	{
		int style = GameManager.Instance.SaveData.BoatFlagStyle;
		bool isFlagEnabled = style != 0;
		this.flagObjects.ForEach(delegate(GameObject flagObject)
		{
			flagObject.gameObject.SetActive(isFlagEnabled);
		});
		this.flagMeshRenderers.ForEach(delegate(SkinnedMeshRenderer flagMeshRenderer)
		{
			if (isFlagEnabled)
			{
				flagMeshRenderer.material = this.flagMaterials[style - 1];
			}
		});
	}

	[SerializeField]
	private List<Color> roofColors = new List<Color>();

	[SerializeField]
	private List<Color> hullColors = new List<Color>();

	[SerializeField]
	private Dictionary<int, int> defaultRoofColorIndices = new Dictionary<int, int>();

	[SerializeField]
	private Dictionary<int, int> defaultHullColorIndices = new Dictionary<int, int>();

	[SerializeField]
	private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

	[SerializeField]
	private List<GameObject> buntingObjects = new List<GameObject>();

	[SerializeField]
	private List<SkinnedMeshRenderer> buntingMeshRenderers = new List<SkinnedMeshRenderer>();

	[SerializeField]
	private List<GameObject> flagObjects = new List<GameObject>();

	[SerializeField]
	private List<SkinnedMeshRenderer> flagMeshRenderers = new List<SkinnedMeshRenderer>();

	[SerializeField]
	private List<Material> flagMaterials = new List<Material>();
}
