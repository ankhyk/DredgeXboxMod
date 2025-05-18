using System;
using UnityEngine;

public class SceneDarknessResponder : MonoBehaviour
{
	private void Update()
	{
		bool activeSelf = this.objectToToggle.activeSelf;
		bool flag = GameManager.Instance.Time.SceneLightness == 1f;
		bool flag2 = this.onWhenDark && flag;
		if (activeSelf != flag2)
		{
			this.objectToToggle.SetActive(flag2);
		}
	}

	[SerializeField]
	private GameObject objectToToggle;

	[SerializeField]
	private bool onWhenDark;
}
