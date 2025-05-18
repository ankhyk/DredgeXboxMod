using System;
using UnityEngine;

public static class VectorUtils
{
	public static int GetClosestIndex(Transform origin, Transform[] objects)
	{
		int num = -1;
		float num2 = float.PositiveInfinity;
		Vector3 position = origin.position;
		for (int i = 0; i < objects.Length; i++)
		{
			float num3 = Vector3.Distance(objects[i].position, position);
			if (num3 < num2)
			{
				num = i;
				num2 = num3;
			}
		}
		return num;
	}

	public static Vector2 GetPointOnUnitCircleCircumference(float radius)
	{
		float num = global::UnityEngine.Random.Range(0f, 6.2831855f);
		return new Vector2(Mathf.Sin(num), Mathf.Cos(num)) * radius;
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}
}
