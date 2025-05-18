using System;
using UnityEngine;

public class CreditNameHelper : MonoBehaviour
{
	[SerializeField]
	private string names;

	[SerializeField]
	private Transform parentTransform;

	[SerializeField]
	private GameObject namePrefab;

	[SerializeField]
	private RectTransform sectionTransform;

	[SerializeField]
	private bool sortAlphabetically;

	[SerializeField]
	private float columns;

	[SerializeField]
	private float nameHeight = 100f;
}
