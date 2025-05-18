using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UnityExtensions
{
	public static bool Contains(this LayerMask mask, int layer)
	{
		return mask == (mask | (1 << layer));
	}

	public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
	{
		Vector3 vector = rectTransform.pivot - pivot;
		vector.Scale(rectTransform.rect.size);
		vector.Scale(rectTransform.localScale);
		vector = rectTransform.rotation * vector;
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= vector;
	}

	public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

	public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect scroller, RectTransform child)
	{
		Canvas.ForceUpdateCanvases();
		Vector2 vector = scroller.transform.InverseTransformPoint(scroller.content.position);
		Vector2 vector2 = scroller.transform.InverseTransformPoint(child.position);
		Vector2 vector3 = vector - vector2;
		if (!scroller.horizontal)
		{
			vector3.x = vector.x;
		}
		if (!scroller.vertical)
		{
			vector3.y = vector.y;
		}
		return vector3;
	}

	public static IEnumerable<Type> GetFilteredTypeList(Type t)
	{
		return from x in t.Assembly.GetTypes()
			where !x.IsAbstract
			where t.IsAssignableFrom(x)
			select x;
	}
}
