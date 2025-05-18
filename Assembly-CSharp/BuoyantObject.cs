using System;
using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
	public float SmoothedWaveSteepness
	{
		get
		{
			return this.smoothedWaveSteepness;
		}
	}

	public float CurrentWaveSteepness
	{
		get
		{
			return this.currentWaveSteepness;
		}
	}

	private void Awake()
	{
		this._waveController = GameManager.Instance.WaveController;
		this.effectorProjections = new Vector3[this.effectors.Length];
		for (int i = 0; i < this.effectors.Length; i++)
		{
			this.effectorProjections[i] = this.effectors[i].position;
		}
	}

	private void OnDisable()
	{
		this.rb.useGravity = true;
	}

	private void Start()
	{
		if (this.isStatic)
		{
			this.UpdateWaveSteepness();
			return;
		}
		base.InvokeRepeating("UpdateWaveSteepness", 0.5f, 0.5f);
	}

	private void UpdateWaveSteepness()
	{
		this.waveSteepness = Mathf.Clamp01(GameManager.Instance.WaveController.SampleWaveSteepnessAtPosition(base.transform.position) * 10f);
	}

	private void FixedUpdate()
	{
		this.smoothedWaveSteepness = Mathf.Lerp(this.smoothedWaveSteepness, this.waveSteepness, Time.fixedDeltaTime);
		this.currentWaveSteepness = this._waveController.Steepness * this.smoothedWaveSteepness;
		int num = this.effectors.Length;
		for (int i = 0; i < num; i++)
		{
			Vector3 position = this.effectors[i].position;
			this.effectorProjections[i] = WaveDisplacement.GetWaveDisplacement(position, this.currentWaveSteepness, this._waveController.Wavelength, this._waveController.Speed, this._waveController.Directions);
			float y = this.effectorProjections[i].y;
			float y2 = position.y;
			float num2 = y - y2;
			this.rb.AddForceAtPosition(Physics.gravity / (float)num * this.gravityModifier, position, ForceMode.Acceleration);
			if (num2 > 0f)
			{
				float num3 = Mathf.Clamp01(num2) / this.objectDepth;
				float num4 = Mathf.Abs(Physics.gravity.y) * num3 * this.strength;
				this.rb.AddForceAtPosition(Vector3.up * num4, position, ForceMode.Acceleration);
				this.rb.AddForce(-this.rb.velocity * this.velocityDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
				this.rb.AddTorque(-this.rb.angularVelocity * this.angularDrag * Time.fixedDeltaTime, ForceMode.Impulse);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (this.effectors == null)
		{
			return;
		}
		for (int i = 0; i < this.effectors.Length; i++)
		{
			if (!Application.isPlaying && this.effectors[i] != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(this.effectors[i].position, 0.06f);
			}
			else
			{
				if (this.effectors[i] == null)
				{
					return;
				}
				if (this.effectors[i].position.y < this.effectorProjections[i].y)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.green;
				}
				Gizmos.DrawSphere(this.effectors[i].position, 0.06f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(this.effectorProjections[i], 0.16f);
			}
		}
	}

	[SerializeField]
	private LayerMask waterLayer;

	[SerializeField]
	private bool isStatic;

	private WaveController _waveController;

	[Header("Buoyancy")]
	public float strength = 1f;

	[Range(0.1f, 5f)]
	public float objectDepth = 1f;

	public float gravityModifier = 1f;

	public float velocityDrag = 0.99f;

	public float angularDrag = 0.5f;

	[Header("Effectors")]
	public Transform[] effectors;

	[SerializeField]
	private Rigidbody rb;

	private Vector3[] effectorProjections;

	private float waveSteepness;

	private float smoothedWaveSteepness;

	private float currentWaveSteepness;
}
