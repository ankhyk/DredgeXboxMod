using System;
using UnityEngine;

public class SpiralComponent : MonoBehaviour
{
	public void SetPosition(float newPosition, SpiralMinigame.SpiralConfig spiralConfig)
	{
		float num = Mathf.Lerp(0f, spiralConfig.totalDegrees, newPosition) + spiralConfig.startAngleOffset;
		num *= 0.017453292f;
		num *= spiralConfig.direction;
		float num2 = Mathf.Lerp(spiralConfig.startRadius, spiralConfig.endRadius, newPosition);
		float num3 = num2 * Mathf.Cos(num);
		float num4 = num2 * Mathf.Sin(num);
		(base.transform as RectTransform).anchoredPosition = new Vector2(num3, num4);
	}

	public void SetRotation(float newPosition, SpiralMinigame.SpiralConfig spiralConfig)
	{
		float num = Mathf.Lerp(0f, spiralConfig.totalDegrees, newPosition) + spiralConfig.startAngleOffset;
		num *= spiralConfig.direction;
		base.transform.rotation = Quaternion.Euler(0f, 0f, num + spiralConfig.baseGameRotation);
	}
}
