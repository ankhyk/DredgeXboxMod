using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTrees : MonoBehaviour
{
	private void DoPlace()
	{
		Vector3 size = base.GetComponent<Renderer>().bounds.size;
		for (int i = 0; i < this.NumberOfTries; i++)
		{
			Vector3 position = base.transform.position;
			position.y += 20f;
			position.x += global::UnityEngine.Random.Range(-size.x, size.x);
			position.z += global::UnityEngine.Random.Range(-size.z, size.z);
			RaycastHit raycastHit;
			if (Physics.Raycast(position, Vector3.down, out raycastHit, float.PositiveInfinity) && raycastHit.transform == base.transform && raycastHit.point.y > 0f && raycastHit.normal.y > 0.8f && Mathf.PerlinNoise(position.x * this.noiseScale + this.noiseOffset.x, position.z * this.noiseScale + this.noiseOffset.y) > 0.5f)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.trees[global::UnityEngine.Random.Range(0, this.trees.Length)], raycastHit.point + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
				gameObject.transform.localScale = Vector3.one * global::UnityEngine.Random.Range(this.treeSize - this.treeSizeVariation, this.treeSize + this.treeSizeVariation);
				gameObject.transform.LookAt(raycastHit.normal);
				Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
				localEulerAngles.y = 0f;
				gameObject.transform.localEulerAngles = localEulerAngles;
				gameObject.transform.parent = base.transform;
			}
		}
	}

	private void CombineMeshes(List<MeshFilter> meshFilters)
	{
		if (this.treesObject)
		{
			global::UnityEngine.Object.DestroyImmediate(this.treesObject);
		}
		this.treesObject = new GameObject();
		MeshFilter meshFilter = this.treesObject.AddComponent<MeshFilter>();
		this.treesObject.AddComponent<MeshRenderer>().material = this.treeMaterial;
		this.treesObject.name = "TreesMesh";
		CombineInstance[] array = new CombineInstance[meshFilters.Count];
		for (int i = 0; i < meshFilters.Count; i++)
		{
			array[i].mesh = meshFilters[i].sharedMesh;
			array[i].transform = meshFilters[i].transform.localToWorldMatrix;
			global::UnityEngine.Object.DestroyImmediate(meshFilters[i].gameObject);
		}
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.name = "CombinedMesh";
		meshFilter.sharedMesh.CombineMeshes(array);
		this.treesObject.transform.SetParent(base.transform);
	}

	private void RandomizeColours(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color[] colors = mesh.colors;
		Color[] array = new Color[vertices.Length];
		float num = global::UnityEngine.Random.Range(0f, 1f);
		for (int i = 0; i < vertices.Length; i++)
		{
			array[i] = new Color(colors[i].r, colors[i].g, num, 1f);
		}
		mesh.colors = array;
	}

	[SerializeField]
	private GameObject[] trees;

	[SerializeField]
	private int NumberOfTries;

	[Range(0.01f, 0.2f)]
	[SerializeField]
	private float noiseScale = 0.08f;

	[SerializeField]
	private Vector2 noiseOffset;

	[Range(0.5f, 2f)]
	[SerializeField]
	private float treeSize = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float treeSizeVariation = 0.5f;

	[SerializeField]
	private Material treeMaterial;

	[SerializeField]
	private GameObject treesObject;
}
