using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TIRWorldPhaseObjectEnabler : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		this.Initialize();
		this.OnTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
	}

	private void Initialize()
	{
		this.objects.ForEach(delegate(List<GameObject> l)
		{
			l.ForEach(delegate(GameObject o)
			{
				o.gameObject.SetActive(false);
			});
		});
	}

	private void OnTIRWorldPhaseChanged(int tirWorldPhase)
	{
		int num = 0;
		while (num < tirWorldPhase && num < this.objects.Count)
		{
			this.objects[num].ForEach(delegate(GameObject o)
			{
				o.gameObject.SetActive(true);
			});
			num++;
		}
		for (int i = tirWorldPhase; i < this.objects.Count; i++)
		{
			this.objects[i].ForEach(delegate(GameObject o)
			{
				o.gameObject.SetActive(false);
			});
		}
	}

	[SerializeField]
	private List<List<GameObject>> objects = new List<List<GameObject>>();
}
