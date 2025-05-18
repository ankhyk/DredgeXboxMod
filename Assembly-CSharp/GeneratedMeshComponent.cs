using System;
using UnityEngine;

public class GeneratedMeshComponent : MonoBehaviour
{
	public float GetLODRadius()
	{
		LODGroup lodgroup = base.GetComponent<LODGroup>();
		if (lodgroup == null)
		{
			lodgroup = base.GetComponentInChildren<LODGroup>();
		}
		if (lodgroup)
		{
			return lodgroup.size * 0.5f;
		}
		return 1f;
	}

	public Renderer GetRenderer()
	{
		LODGroup lodgroup = base.GetComponent<LODGroup>();
		if (lodgroup == null)
		{
			lodgroup = base.GetComponentInChildren<LODGroup>();
		}
		Renderer renderer;
		if (lodgroup)
		{
			renderer = lodgroup.GetLODs()[lodgroup.lodCount - 1].renderers[0];
			if (renderer != null)
			{
				return renderer;
			}
		}
		renderer = base.GetComponent<MeshRenderer>();
		if (renderer == null)
		{
			renderer = base.GetComponentInChildren<MeshRenderer>();
		}
		return renderer;
	}
}
