using System;
using CommandTerminal;
using UnityEngine;

[ExecuteInEditMode]
[DefaultExecutionOrder(-700)]
public class WaveController : MonoBehaviour
{
	public float Steepness
	{
		get
		{
			return this.steepness;
		}
		set
		{
			this.steepness = value;
		}
	}

	public float Wavelength
	{
		get
		{
			return this.wavelength;
		}
	}

	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	public float[] Directions
	{
		get
		{
			return this.waveDirections;
		}
	}

	public float WaterHeight
	{
		get
		{
			return this.waterHeight;
		}
	}

	private void OnEnable()
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.WaveController = this;
			this.cachedWorldSize = GameManager.Instance.GameConfigData.WorldSize;
			this.cachedHalfWorldSize = this.cachedWorldSize * 0.5f;
			if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1))
			{
				Shader.EnableKeyword("_DLC1_WAVEMASK");
				return;
			}
			Shader.DisableKeyword("_DLC1_WAVEMASK");
			this.DLC1BiomeWaveMask = null;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.WaveController = null;
		}
	}

	private void Awake()
	{
		Shader.SetGlobalFloat("_WaveSteepness", this.steepness);
		Shader.SetGlobalFloat("_WaveLength", this.wavelength);
		Shader.SetGlobalFloat("_WaveSpeed", this.speed);
		Shader.SetGlobalVector("_WaveDirections", new Vector4(this.waveDirections[0], this.waveDirections[1], this.waveDirections[2], this.waveDirections[3]));
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			Shader.SetGlobalFloat("_WaveSteepness", this.steepness);
			Shader.SetGlobalFloat("_WaveLength", this.wavelength);
			Shader.SetGlobalFloat("_WaveSpeed", this.speed);
			Shader.SetGlobalVector("_WaveDirections", new Vector4(this.waveDirections[0], this.waveDirections[1], this.waveDirections[2], this.waveDirections[3]));
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			this.AddTerminalCommands();
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			this.RemoveTerminalCommands();
		}
	}

	public float SampleWaterDepthAtPlayerPosition()
	{
		return this.SampleWaterDepthAtPosition(GameManager.Instance.Player.transform.position);
	}

	public float SampleWaveSteepnessAtPlayerPosition()
	{
		return this.SampleWaveSteepnessAtPosition(GameManager.Instance.Player.transform.position);
	}

	private Vector2 GetSamplePositionByWorldPosition(Vector3 position)
	{
		return new Vector2(position.x + this.cachedHalfWorldSize, position.z + this.cachedHalfWorldSize) / this.cachedWorldSize;
	}

	public float SampleWaveSteepnessAtPosition(Vector3 samplePos)
	{
		Vector2 samplePositionByWorldPosition = this.GetSamplePositionByWorldPosition(samplePos);
		Color pixelBilinear = this.waveHeightMask.GetPixelBilinear(samplePositionByWorldPosition.x, samplePositionByWorldPosition.y);
		if (this.DLC1BiomeWaveMask != null)
		{
			float num = samplePos.x - this.DLC1BiomeWaveMask.transform.position.x + this.DLC1BiomeWaveMask.Bounds / 2f;
			float num2 = samplePos.z - this.DLC1BiomeWaveMask.transform.position.z + this.DLC1BiomeWaveMask.Bounds / 2f;
			num /= this.DLC1BiomeWaveMask.Bounds;
			num2 /= this.DLC1BiomeWaveMask.Bounds;
			Color pixelBilinear2 = this.DLC1BiomeWaveMask.WaveHeightMask.GetPixelBilinear(num, num2);
			pixelBilinear.a = Mathf.Min(pixelBilinear.a, pixelBilinear2.a);
		}
		return pixelBilinear.a;
	}

	public float SampleWaterDepthAtPosition(Vector3 samplePos)
	{
		Vector2 samplePositionByWorldPosition = this.GetSamplePositionByWorldPosition(samplePos);
		Color pixelBilinear = this.waveHeightMask.GetPixelBilinear(samplePositionByWorldPosition.x, samplePositionByWorldPosition.y);
		if (this.DLC1BiomeWaveMask != null)
		{
			float num = samplePos.x - this.DLC1BiomeWaveMask.transform.position.x + this.DLC1BiomeWaveMask.Bounds / 2f;
			float num2 = samplePos.z - this.DLC1BiomeWaveMask.transform.position.z + this.DLC1BiomeWaveMask.Bounds / 2f;
			num /= this.DLC1BiomeWaveMask.Bounds;
			num2 /= this.DLC1BiomeWaveMask.Bounds;
			Color pixelBilinear2 = this.DLC1BiomeWaveMask.WaveHeightMask.GetPixelBilinear(num, num2);
			pixelBilinear.g = Mathf.Min(pixelBilinear.g, pixelBilinear2.g);
		}
		return pixelBilinear.g;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("wave.steep", new Action<CommandArg[]>(this.SetWaveSteepness), 1, 1, "Sets wave steepness");
		Terminal.Shell.AddCommand("wave.length", new Action<CommandArg[]>(this.SetWaveLength), 1, 1, "Sets wave length");
		Terminal.Shell.AddCommand("wave.speed", new Action<CommandArg[]>(this.SetWaveSpeed), 1, 1, "Sets wave speed");
		Terminal.Shell.AddCommand("wave.directions", new Action<CommandArg[]>(this.SetWaveDirections), 4, 4, "Sets wave directions");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("wave.steep");
		Terminal.Shell.RemoveCommand("wave.length");
		Terminal.Shell.RemoveCommand("wave.speed");
		Terminal.Shell.RemoveCommand("wave.directions");
	}

	private void SetWaveSteepness(CommandArg[] args)
	{
		this.steepness = args[0].Float;
		Shader.SetGlobalFloat("_WaveSteepness", this.steepness);
	}

	private void SetWaveLength(CommandArg[] args)
	{
		this.wavelength = args[0].Float;
		Shader.SetGlobalFloat("_WaveLength", this.wavelength);
	}

	private void SetWaveSpeed(CommandArg[] args)
	{
		this.speed = args[0].Float;
		Shader.SetGlobalFloat("_WaveSpeed", this.speed);
	}

	private void SetWaveDirections(CommandArg[] args)
	{
		this.waveDirections[0] = args[0].Float;
		this.waveDirections[1] = args[1].Float;
		this.waveDirections[2] = args[2].Float;
		this.waveDirections[3] = args[3].Float;
		Shader.SetGlobalVector("_WaveDirections", new Vector4(this.waveDirections[0], this.waveDirections[1], this.waveDirections[2], this.waveDirections[3]));
	}

	[SerializeField]
	private Texture2D waveHeightMask;

	[SerializeField]
	private float waterHeight;

	[SerializeField]
	public float steepness;

	[SerializeField]
	private float wavelength;

	private float speed = 0.1f;

	[SerializeField]
	private float[] waveDirections = new float[4];

	[SerializeField]
	private AdditionalWaveMask DLC1BiomeWaveMask;

	private float cachedWorldSize;

	private float cachedHalfWorldSize;
}
