using System;
using UnityEngine;

public class SimpleBuoyantObject : MonoBehaviour
{
	private void Awake()
	{
		this.transformHelper = base.transform.position;
		this._waveController = GameManager.Instance.WaveController;
		this.UpdateWaveSteepness();
	}

	private void UpdateWaveSteepness()
	{
		this.waveSteepnessAtPos = Mathf.Clamp01(GameManager.Instance.WaveController.SampleWaveSteepnessAtPosition(base.transform.position) * 10f);
		this.targetYPos = WaveDisplacement.GetWaveDisplacement(base.transform.position, this._waveController.Steepness * this.waveSteepnessAtPos, this._waveController.Wavelength, this._waveController.Speed, this._waveController.Directions).y + this.objectDepth;
	}

	private void Update()
	{
		this.timeSinceWaveSteepnessUpdated += Time.deltaTime;
		if (this.timeSinceWaveSteepnessUpdated > this.timeBetweenUpdatingWaveSteepnessSec)
		{
			this.UpdateWaveSteepness();
			this.timeSinceWaveSteepnessUpdated = 0f;
		}
		this.transformHelper.y = Mathf.Lerp(this.transformHelper.y, this.targetYPos, Time.deltaTime);
		base.transform.position = this.transformHelper;
	}

	[SerializeField]
	private float timeBetweenUpdatingWaveSteepnessSec = 0.5f;

	[SerializeField]
	public float objectDepth;

	private WaveController _waveController;

	private Vector3 transformHelper;

	private float waveSteepnessAtPos;

	private float timeSinceWaveSteepnessUpdated;

	private float targetYPos;
}
