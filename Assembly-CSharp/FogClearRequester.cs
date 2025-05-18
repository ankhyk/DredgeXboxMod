using System;
using UnityEngine;

public class FogClearRequester : MonoBehaviour
{
	private void LateUpdate()
	{
		Shader.SetGlobalVector("_FogCenter", new Vector4(base.transform.position.x, base.transform.position.y, base.transform.position.z, 0f));
		Shader.SetGlobalFloat("_FogRemove", this.fogClearAmount);
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float fogClearAmount;
}
