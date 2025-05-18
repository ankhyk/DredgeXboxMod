using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsTeamContainer : MonoBehaviour
{
	private void SetPeople()
	{
		this.teamNameField.text = base.gameObject.name;
		for (int i = this.peopleContainer.childCount; i > 0; i--)
		{
			global::UnityEngine.Object.DestroyImmediate(this.peopleContainer.GetChild(0).gameObject);
		}
		this.people.ForEach(delegate(string p)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.personPrefab, this.peopleContainer).GetComponent<TextMeshProUGUI>().text = p;
		});
	}

	[SerializeField]
	private TextMeshProUGUI teamNameField;

	[SerializeField]
	private Transform peopleContainer;

	[SerializeField]
	private GameObject personPrefab;

	[SerializeField]
	private List<string> people;
}
