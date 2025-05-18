using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpeakerData", menuName = "Dredge/SpeakerData", order = 0)]
public class SpeakerData : SerializedScriptableObject
{
	public AssetReference GetParalinguisticByType(ParalinguisticType paralinguisticType)
	{
		Dictionary<ParalinguisticType, List<AssetReference>> dictionary = this.paralinguistics;
		if (this.paralinguisticOverrideConditions != null && this.paralinguisticOverrideConditions.Count > 0)
		{
			for (int i = 0; i < this.paralinguisticOverrideConditions.Count; i++)
			{
				if (this.paralinguisticOverrideConditions[i].nodesVisited.All((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n)))
				{
					dictionary = this.paralinguisticOverrideConditions[i].config.paralinguistics;
					break;
				}
			}
		}
		if (dictionary.ContainsKey(paralinguisticType))
		{
			return dictionary[paralinguisticType].PickRandom<AssetReference>();
		}
		return null;
	}

	public string speakerNameKey;

	public List<NameKeyOverride> speakerNameKeyOverrides = new List<NameKeyOverride>();

	public string yarnRootNode;

	public GameObject portraitPrefab;

	public List<PortraitOverride> portraitOverrideConditions;

	public Sprite smallPortraitSprite;

	public AssetReference visitSFX;

	public AudioClip loopSFX;

	public bool isIndoors;

	public bool availableInDemo;

	public bool alwaysAvailable;

	public bool hideNameplate;

	public List<HighlightCondition> highlightConditions;

	public Dictionary<ParalinguisticType, List<AssetReference>> paralinguistics = new Dictionary<ParalinguisticType, List<AssetReference>>();

	public List<ParalinguisticOverride> paralinguisticOverrideConditions;
}
