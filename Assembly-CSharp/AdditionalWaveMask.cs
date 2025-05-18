using System;
using UnityEngine;

[ExecuteInEditMode]
public class AdditionalWaveMask : MonoBehaviour
{
	public float Bounds
	{
		get
		{
			return this.bounds;
		}
	}

	public Texture2D WaveHeightMask
	{
		get
		{
			return this.waveHeightMask;
		}
	}

	private void Start()
	{
		this.UpdateShaderVariables();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 1f);
		Gizmos.DrawWireCube(base.transform.position, new Vector3(this.bounds, 100f, this.bounds));
		this.UpdateShaderVariables();
	}

	private void UpdateShaderVariables()
	{
		Shader.SetGlobalVector(this.shaderPositionVariableName, base.transform.position);
		Shader.SetGlobalFloat(this.shaderBoundsVariableName, this.bounds);
	}

	[SerializeField]
	private float bounds = 100f;

	[SerializeField]
	private Texture2D waveHeightMask;

	[SerializeField]
	private string shaderPositionVariableName = "_DLC1BiomeCenter";

	[SerializeField]
	private string shaderBoundsVariableName = "_DLC1BiomeScale";
}
