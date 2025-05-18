using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SanityModifierDetector : MonoBehaviour
{
	public bool IgnoreTimescale { get; set; }

	private void OnTriggerEnter(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerEnter(other, base.gameObject, new ReliableOnTriggerExit._OnTriggerExit(this.OnTriggerExit));
		SanityModifier component = other.gameObject.GetComponent<SanityModifier>();
		if (component && !this.currentSanityModifiers.Contains(component))
		{
			this.currentSanityModifiers.Add(component);
		}
		this.IgnoreTimescale = this.currentSanityModifiers.Any((SanityModifier x) => x.IgnoreTimescale);
	}

	private void OnTriggerExit(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerExit(other, base.gameObject);
		SanityModifier component = other.gameObject.GetComponent<SanityModifier>();
		if (component && this.currentSanityModifiers.Contains(component))
		{
			this.currentSanityModifiers.Remove(component);
		}
		this.IgnoreTimescale = this.currentSanityModifiers.Any((SanityModifier x) => x.IgnoreTimescale);
	}

	public float GetTotalModifierValue()
	{
		float num = 0f;
		for (int i = 0; i < this.currentSanityModifiers.Count; i++)
		{
			num += this.currentSanityModifiers[i].GetModifierValueForPoint(base.transform.position);
		}
		return num;
	}

	public List<SanityModifier> currentSanityModifiers = new List<SanityModifier>();
}
