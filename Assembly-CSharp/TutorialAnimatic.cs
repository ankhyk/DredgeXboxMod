using System;
using UnityEngine;

public class TutorialAnimatic : MonoBehaviour, ILayoutable
{
	public bool IsLayedOut
	{
		get
		{
			return true;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return base.gameObject;
		}
	}
}
