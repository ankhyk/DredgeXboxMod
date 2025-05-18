using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayUI : MonoBehaviour
{
	private void Update()
	{
		this.image.rectTransform.eulerAngles = new Vector3(this.image.transform.eulerAngles.x, this.image.transform.eulerAngles.y, -(GameManager.Instance.Time.Time * 360f));
	}

	[SerializeField]
	private Image image;
}
