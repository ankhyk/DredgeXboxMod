using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public void Generate()
	{
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		List<GeneratedMeshComponent> list = base.GetComponentsInChildren<GeneratedMeshComponent>(true).ToList<GeneratedMeshComponent>();
		list.Prepend(base.GetComponent<GeneratedMeshComponent>());
		new List<CombineInstance>();
		for (int i = 0; i < list.Count; i++)
		{
			Renderer renderer = list[i].GetRenderer();
			if (renderer == null)
			{
				Debug.LogWarning("Renderer was null for " + list[i].name);
			}
			else
			{
				MeshFilter component = renderer.gameObject.GetComponent<MeshFilter>();
				if (component == null)
				{
					Debug.LogWarning("MeshFilter was null for " + list[i].name);
				}
				else if (renderer && component.sharedMesh && renderer.sharedMaterials.Length == component.sharedMesh.subMeshCount)
				{
					for (int j = 0; j < component.sharedMesh.subMeshCount; j++)
					{
						int num = this.Contains(arrayList, renderer.sharedMaterials[j].name);
						if (num == -1)
						{
							arrayList.Add(renderer.sharedMaterials[j]);
							num = arrayList.Count - 1;
						}
						arrayList2.Add(new ArrayList());
						CombineInstance combineInstance = default(CombineInstance);
						combineInstance.transform = worldToLocalMatrix * component.transform.localToWorldMatrix;
						combineInstance.subMeshIndex = j;
						combineInstance.mesh = component.sharedMesh;
						(arrayList2[num] as ArrayList).Add(combineInstance);
					}
				}
			}
		}
		Mesh[] array = new Mesh[arrayList.Count];
		CombineInstance[] array2 = new CombineInstance[arrayList.Count];
		for (int k = 0; k < arrayList.Count; k++)
		{
			CombineInstance[] array3 = (arrayList2[k] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
			array[k] = new Mesh();
			array[k].CombineMeshes(array3, true, true);
			array2[k] = default(CombineInstance);
			array2[k].mesh = array[k];
			array2[k].subMeshIndex = 0;
		}
		Mesh mesh = new Mesh();
		mesh.CombineMeshes(array2, true, false);
		foreach (Mesh mesh2 in array)
		{
			mesh2.Clear();
			global::UnityEngine.Object.DestroyImmediate(mesh2);
		}
		if (string.IsNullOrEmpty("Assets/Meshes/Generated/" + base.name + "_gen.asset"))
		{
			return;
		}
		if (this.replacerObject == null && this.createReplacerObject)
		{
			this.replacerObject = new GameObject(base.name + "_replacer");
			this.replacerObject.transform.parent = base.transform.parent;
			this.replacerObject.transform.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
			this.replacerObject.AddComponent<MeshFilter>();
			this.replacerObject.AddComponent<MeshRenderer>();
		}
		if (this.replacerObject)
		{
			MeshFilter component2 = this.replacerObject.GetComponent<MeshFilter>();
			if (component2)
			{
				component2.mesh = mesh;
			}
			this.replacerObject.transform.localPosition = base.transform.localPosition;
			this.replacerObject.transform.localScale = base.transform.localScale;
			this.replacerObject.transform.localRotation = base.transform.localRotation;
		}
		if (this.configureCullable)
		{
			Cullable cullable = base.GetComponent<Cullable>();
			if (cullable == null)
			{
				cullable = base.gameObject.AddComponent<Cullable>();
			}
			cullable.replacerObject = this.replacerObject;
			cullable.sphereRadius = list[0].GetLODRadius();
		}
	}

	private int Contains(ArrayList searchList, string searchName)
	{
		for (int i = 0; i < searchList.Count; i++)
		{
			if (((Material)searchList[i]).name == searchName)
			{
				return i;
			}
		}
		return -1;
	}

	[SerializeField]
	private bool configureCullable = true;

	[SerializeField]
	private bool createReplacerObject = true;

	[SerializeField]
	private GameObject replacerObject;
}
