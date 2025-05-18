using System;
using UnityEngine;

public class SceneTimeResponder : MonoBehaviour
{
	private void Update()
	{
		bool activeSelf = this.objectToToggle.activeSelf;
		bool flag = this.onWhenNight && !GameManager.Instance.Time.IsDaytime;
		if (activeSelf != flag)
		{
			this.objectToToggle.SetActive(flag);
		}
	}

	[SerializeField]
	private GameObject objectToToggle;

	[SerializeField]
	private bool onWhenNight;
}
