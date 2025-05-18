using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PatrollingWildlife : SerializedMonoBehaviour
{
	private void Awake()
	{
		this.pathFollow.Init(this.routeConfig);
	}

	[SerializeField]
	private SimplePathFollow pathFollow;

	[SerializeField]
	private RouteConfig routeConfig;
}
