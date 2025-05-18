using System;
using UnityEngine;

public class TimelineVibrationTriggerer : MonoBehaviour
{
	public void DoVibration1()
	{
		this.OnVibrationSignalReceived(this.vibrationType1);
	}

	public void DoVibration2()
	{
		this.OnVibrationSignalReceived(this.vibrationType2);
	}

	public void DoVibration3()
	{
		this.OnVibrationSignalReceived(this.vibrationType3);
	}

	public void DoVibration4()
	{
		this.OnVibrationSignalReceived(this.vibrationType4);
	}

	public void DoVibration5()
	{
		this.OnVibrationSignalReceived(this.vibrationType5);
	}

	private void OnVibrationSignalReceived(VibrationData vibrationData)
	{
		GameManager.Instance.VibrationManager.Vibrate(vibrationData, VibrationRegion.WholeBody, true);
	}

	[SerializeField]
	private VibrationData vibrationType1;

	[SerializeField]
	private VibrationData vibrationType2;

	[SerializeField]
	private VibrationData vibrationType3;

	[SerializeField]
	private VibrationData vibrationType4;

	[SerializeField]
	private VibrationData vibrationType5;
}
