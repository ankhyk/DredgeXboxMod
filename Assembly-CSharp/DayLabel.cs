using System;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DayLabel : MonoBehaviour
{
	private void Update()
	{
		if (GameManager.Instance.Time.Day != this.prevDay)
		{
			this.prevDay = GameManager.Instance.Time.Day;
			int num = GameManager.Instance.Time.Day % 7;
			string text = string.Format("day-and-date.{0}", num);
			int num2 = GameManager.Instance.Time.Day + 1;
			this.localizedTextField.StringReference.Arguments = new object[] { num2 };
			this.localizedTextField.StringReference.SetReference(LanguageManager.STRING_TABLE, text);
			this.localizedTextField.RefreshString();
			this.localizedTextField.enabled = true;
		}
	}

	[SerializeField]
	private LocalizeStringEvent localizedTextField;

	private int prevDay = -1;
}
