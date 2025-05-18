using System;
using UnityEngine;

public class Net : MonoBehaviour
{
	public NetType NetType
	{
		get
		{
			return this.netType;
		}
	}

	[SerializeField]
	private NetType netType;
}
