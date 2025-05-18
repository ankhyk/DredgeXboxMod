using System;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
	public void OnClick()
	{
		GameManager.Instance.Save();
	}
}
