using System;
using System.Collections.Generic;
using UnityEngine;

public class IceShardStateController : MonoBehaviour
{
	private void OnEnable()
	{
		if (GameManager.Instance && GameManager.Instance.SaveData != null)
		{
			for (int i = 0; i < this.nodeNames.Count; i++)
			{
				bool flag = GameManager.Instance.SaveData.visitedNodes.Contains(this.nodeNames[i]);
				this.skinnedMeshRenderer.SetBlendShapeWeight(i, flag ? 0f : 100f);
				this.iceShardStumps[i].SetActive(flag);
				this.iceShardCrystals[i].SetActive(!flag);
			}
		}
	}

	[SerializeField]
	private List<string> nodeNames = new List<string>();

	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField]
	private List<GameObject> iceShardStumps = new List<GameObject>();

	[SerializeField]
	private List<GameObject> iceShardCrystals = new List<GameObject>();
}
