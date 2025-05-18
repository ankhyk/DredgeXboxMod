using System;
using TMPro;
using UnityEngine;

public class TimeLabel : MonoBehaviour
{
	private void Update()
	{
		this.timeUntilUpdate -= Time.deltaTime;
		if (this.timeUntilUpdate <= 0f)
		{
			this.timeUntilUpdate = this.timeBetweenUpdatesSec;
			this.textField.text = GameManager.Instance.Time.GetTimeFormatted(GameManager.Instance.Time.Time);
		}
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	private float timeUntilUpdate;

	private float timeBetweenUpdatesSec = 0.25f;
}
