using System;
using UnityEngine;

public class GlobalOozeVisibilityController : MonoBehaviour
{
	private void Update()
	{
		Shader.SetGlobalFloat("_GlobalOozeFade", this.globalOozeFadeAmount);
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float globalOozeFadeAmount;
}
