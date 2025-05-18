using System;
using UnityEngine;

public static class WaveDisplacement
{
	private static Vector3 GerstnerWave(Vector3 position, float steepness, float wavelength, float speed, float direction)
	{
		if (wavelength < 8f)
		{
			return Vector3.zero;
		}
		direction = direction * 2f - 1f;
		Vector2 normalized = new Vector2(Mathf.Cos(3.14f * direction), Mathf.Sin(3.14f * direction)).normalized;
		float num = 6.28f / wavelength;
		float num2 = steepness / num;
		float num3 = num * (Vector2.Dot(normalized, new Vector2(position.x, position.z)) - speed * (Time.time % (wavelength / speed)));
		return new Vector3(normalized.x * num2 * Mathf.Cos(num3), num2 * Mathf.Sin(num3), normalized.y * num2 * Mathf.Cos(num3));
	}

	private static Vector3 Wave(Vector3 position, float steepness, float wavelength, float speed, float direction)
	{
		if (wavelength < 8f)
		{
			return Vector3.zero;
		}
		direction = direction * 2f - 1f;
		Vector2 normalized = new Vector2(Mathf.Cos(3.1415927f * direction), Mathf.Sin(3.1415927f * direction)).normalized;
		float num = 6.2831855f / wavelength;
		float num2 = steepness / num;
		float num3 = num * (Vector2.Dot(normalized, new Vector2(position.x, position.z)) - GameManager.Instance.gameTime * speed * wavelength);
		return new Vector3(0f, num2 * Mathf.Sin(num3), 0f);
	}

	public static Vector3 GetWaveDisplacement(Vector3 position, float steepness, float wavelength, float speed, float[] directions)
	{
		return Vector3.zero + WaveDisplacement.Wave(position, steepness, wavelength, speed, directions[0]) + WaveDisplacement.Wave(position, steepness, wavelength * 2f, speed * 0.9f, directions[1]) + WaveDisplacement.Wave(position, steepness * 0.75f, wavelength * 4f, speed * 0.8f, directions[2]) + WaveDisplacement.Wave(position, steepness * 0.5f, wavelength * 6f, speed * 0.7f, directions[3]);
	}
}
