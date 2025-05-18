using System;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.enable)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.prefab);
		}
	}

	private void Awake()
	{
		if (this.awake)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.prefab);
		}
	}

	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool awake;

	[SerializeField]
	private bool enable;
}
