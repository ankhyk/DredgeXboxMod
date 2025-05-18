using System;
using UnityEngine;

public class CustomCursorSetter : MonoBehaviour
{
	private void Awake()
	{
		Cursor.SetCursor(this.cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
	}

	[SerializeField]
	private Texture2D cursorTexture;
}
