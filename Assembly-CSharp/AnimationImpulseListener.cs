using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AnimationImpulseListener : MonoBehaviour
{
	public void GenerateImpulse(int index)
	{
		if (index < 0 || index > this.impulses.Count)
		{
			return;
		}
		this.impulses[index].GenerateImpulseAt(this.impulses[index].transform.position, Vector3.up);
	}

	[SerializeField]
	private List<CinemachineImpulseSource> impulses;
}
