using System;
using UnityEngine;

public class DummyPlayerProxy : PlayerProxy
{
	public override Vector3 GetPlayerPosition()
	{
		return this.fakePlayerTransform.position;
	}

	[SerializeField]
	private Transform fakePlayerTransform;
}
