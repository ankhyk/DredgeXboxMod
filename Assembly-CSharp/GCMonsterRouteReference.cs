using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GCMonsterRouteReference : SerializedMonoBehaviour
{
	public List<EntityPath> Routes
	{
		get
		{
			return this.routes;
		}
	}

	public List<List<Transform>> ExitRoutes
	{
		get
		{
			return this.exitRoutes;
		}
	}

	[SerializeField]
	private List<EntityPath> routes;

	[SerializeField]
	private List<List<Transform>> exitRoutes;
}
