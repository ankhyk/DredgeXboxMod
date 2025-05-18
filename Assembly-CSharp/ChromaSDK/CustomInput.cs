using System;
using UnityEngine;

namespace ChromaSDK
{
	public class CustomInput
	{
		public static Vector3 mousePosition
		{
			get
			{
				return Input.mousePosition;
			}
		}

		public static bool GetMouseButton(int button)
		{
			return Input.GetMouseButton(button);
		}
	}
}
