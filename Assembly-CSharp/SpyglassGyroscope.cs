using System;
using UnityEngine;
using UnityEngine.UI;

public class SpyglassGyroscope : MonoBehaviour
{
	private void Awake()
	{
		this.spyglassGyroscopeXAxisMat = new Material(this.spyglassGyroscopeMaterial);
		this.spyglassGyroscopeYAxisMat = new Material(this.spyglassGyroscopeMaterial);
		this.XAxis.material = this.spyglassGyroscopeXAxisMat;
		this.YAxis.material = this.spyglassGyroscopeYAxisMat;
		this.cam = Camera.main.transform;
	}

	private void Update()
	{
		if (this.cam != null)
		{
			Vector3 zero = Vector3.zero;
			zero.x = this.cam.transform.eulerAngles.y * this.sensitivity;
			zero.y = this.cam.transform.eulerAngles.x * this.sensitivity;
			this.spyglassGyroscopeXAxisMat.SetTextureOffset("_MainTex", new Vector2(0f, zero.x));
			this.spyglassGyroscopeYAxisMat.SetTextureOffset("_MainTex", new Vector2(0f, zero.y));
		}
	}

	[SerializeField]
	private float sensitivity = -0.5f;

	[SerializeField]
	private Material spyglassGyroscopeMaterial;

	[SerializeField]
	private Image XAxis;

	[SerializeField]
	private Image YAxis;

	private Material spyglassGyroscopeXAxisMat;

	private Material spyglassGyroscopeYAxisMat;

	private Transform cam;
}
