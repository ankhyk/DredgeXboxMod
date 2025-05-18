using System;
using UnityEngine;

public class UniqueId : MonoBehaviour
{
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public void GenerateGuid()
	{
		this.id = Guid.NewGuid().ToString();
	}

	[SerializeField]
	private string id = Guid.NewGuid().ToString();
}
