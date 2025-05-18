using System;
using UnityEngine;

public static class MathUtil
{
	public static int GetRandomWeightedIndex(float[] weights)
	{
		if (weights == null || weights.Length == 0)
		{
			return -1;
		}
		float num = 0f;
		for (int i = 0; i < weights.Length; i++)
		{
			float num2 = weights[i];
			if (float.IsPositiveInfinity(num2))
			{
				return i;
			}
			if (num2 >= 0f && !float.IsNaN(num2))
			{
				num += weights[i];
			}
		}
		float value = global::UnityEngine.Random.value;
		float num3 = 0f;
		for (int i = 0; i < weights.Length; i++)
		{
			float num2 = weights[i];
			if (!float.IsNaN(num2) && num2 > 0f)
			{
				num3 += num2 / num;
				if (num3 >= value)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public static float GetRandomGaussian(float minValue = 0f, float maxValue = 1f)
	{
		float num;
		float num3;
		do
		{
			num = 2f * global::UnityEngine.Random.value - 1f;
			float num2 = 2f * global::UnityEngine.Random.value - 1f;
			num3 = num * num + num2 * num2;
		}
		while (num3 >= 1f);
		float num4 = num * Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
		float num5 = (minValue + maxValue) / 2f;
		float num6 = (maxValue - num5) / 3f;
		return Mathf.Clamp(num4 * num6 + num5, minValue, maxValue);
	}

	public static int RoundToStep(int value, int factor)
	{
		return (int)Math.Round((double)value / (double)factor, MidpointRounding.AwayFromZero) * factor;
	}

	public static int NegativeMod(int x, int m)
	{
		return (x % m + m) % m;
	}

	public static bool IsBetween(float testValue, float bound1, float bound2)
	{
		return testValue >= Mathf.Min(bound1, bound2) && testValue <= Mathf.Max(bound1, bound2);
	}

	public static float TransformAngleToNegative180Positive180(float input)
	{
		if (input > 180f)
		{
			input = -180f + (input - 180f);
		}
		return input;
	}
}
