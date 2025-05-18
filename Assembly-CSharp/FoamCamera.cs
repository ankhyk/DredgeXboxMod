using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FoamCamera : MonoBehaviour
{
	private void Awake()
	{
		Camera component = base.GetComponent<Camera>();
		component.fieldOfView = this.mainCam.fieldOfView;
		this.foamTexture = new RenderTexture(Screen.width / this.textureDivide, Screen.height / this.textureDivide, 0, RenderTextureFormat.R8);
		component.targetTexture = this.foamTexture;
	}

	private void Update()
	{
		Shader.SetGlobalTexture("_FoamTexture", this.foamTexture);
	}

	[Range(1f, 8f)]
	[SerializeField]
	private int textureDivide = 1;

	[SerializeField]
	private Camera mainCam;

	private RenderTexture foamTexture;
}
