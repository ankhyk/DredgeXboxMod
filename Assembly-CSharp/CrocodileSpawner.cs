using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrocodileSpawner : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
	}

	private void Start()
	{
		this.twistedStrandMonsterManager = global::UnityEngine.Object.FindObjectOfType<TwistedStrandMonsterManager>();
	}

	private void OnPlayerDetected()
	{
		if (this.twistedStrandMonsterManager == null)
		{
			this.twistedStrandMonsterManager = global::UnityEngine.Object.FindObjectOfType<TwistedStrandMonsterManager>();
		}
		if (this.twistedStrandMonsterManager != null && !this.twistedStrandMonsterManager.IsMonsterSpawned && GameManager.Instance.WorldEventManager.TestWorldEvent(this.crocodileWorldEventData, true))
		{
			this.SpawnCrocodile();
		}
	}

	private void SpawnCrocodile()
	{
		GameManager.Instance.WorldEventManager.AddWorldEventToHistory(this.crocodileWorldEventData, GameManager.Instance.Time.TimeAndDay);
		Transform transform = this.routeConfig.route[0].transform;
		CrocodileWorldEvent component = global::UnityEngine.Object.Instantiate<GameObject>(this.crocodilePrefab, transform.position, transform.rotation).GetComponent<CrocodileWorldEvent>();
		if (component)
		{
			component.Init(this.routeConfig);
		}
	}

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private GameObject crocodilePrefab;

	[SerializeField]
	private RouteConfig routeConfig;

	[SerializeField]
	private WorldEventData crocodileWorldEventData;

	private TwistedStrandMonsterManager twistedStrandMonsterManager;
}
