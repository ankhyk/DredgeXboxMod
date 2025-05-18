using System;
using System.Collections.Generic;
using UnityEngine;

public class IceMonsterSpawner : MonoBehaviour
{
	private void OnEnable()
	{
		this.grid = GameManager.Instance.SaveData.GetGridByKey(this.linkedGridKey);
		if (this.grid != null)
		{
			SerializableGrid serializableGrid = this.grid;
			serializableGrid.OnContentsUpdated = (Action)Delegate.Combine(serializableGrid.OnContentsUpdated, new Action(this.OnGridUpdated));
		}
	}

	private void OnDisable()
	{
		if (this.grid != null)
		{
			SerializableGrid serializableGrid = this.grid;
			serializableGrid.OnContentsUpdated = (Action)Delegate.Remove(serializableGrid.OnContentsUpdated, new Action(this.OnGridUpdated));
		}
	}

	private void OnGridUpdated()
	{
		if (this.grid.GetFilledCells(ItemSubtype.FISH) > 0)
		{
			if (!this.isPendingSpawnAfterClosingGrid)
			{
				GameEvents.Instance.OnDialogueCompleted += this.OnDialogueCompleted;
				this.isPendingSpawnAfterClosingGrid = true;
				return;
			}
		}
		else if (this.isPendingSpawnAfterClosingGrid)
		{
			GameEvents.Instance.OnDialogueCompleted -= this.OnDialogueCompleted;
			this.isPendingSpawnAfterClosingGrid = false;
		}
	}

	private void OnDialogueCompleted()
	{
		GameEvents.Instance.OnDialogueCompleted -= this.OnDialogueCompleted;
		this.isPendingSpawnAfterClosingGrid = false;
		this.CheckShouldSpawn(true);
	}

	private void Update()
	{
		if (this.isReady)
		{
			if (!this.isMonsterSpawned)
			{
				if (this.IsPlayerInPaleReach() && GameManager.Instance.Player.Controller.IsMoving)
				{
					GameManager.Instance.SaveData.IceMonsterTimeUntilSpawn -= (float)GameManager.Instance.Time.GetTimeChangeThisFrame();
				}
				if (GameManager.Instance.SaveData.IceMonsterTimeUntilSpawn <= 0f)
				{
					this.CheckShouldSpawn(false);
					return;
				}
			}
		}
		else if (GameManager.Instance && GameManager.Instance.Player)
		{
			this.isReady = true;
		}
	}

	private bool IsPlayerInPaleReach()
	{
		return GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone() == ZoneEnum.PALE_REACH;
	}

	private bool IsPlayerInRange()
	{
		return Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position) < this.maxRangeFromSpawnAnchor;
	}

	private void CheckShouldSpawn(bool isSpawningForFood)
	{
		if (GameManager.Instance && GameManager.Instance.Player)
		{
			bool flag = GameManager.Instance.SaveData.IceMonsterTimeUntilSpawn < 0f;
			bool flag2 = this.IsPlayerInRange();
			bool isAbilityActive = GameManager.Instance.PlayerAbilities.GetIsAbilityActive(this.banishAbilityData);
			bool isDocked = GameManager.Instance.Player.IsDocked;
			if (!flag)
			{
			}
			if ((flag || isSpawningForFood) && flag2 && !isAbilityActive && !isDocked)
			{
				this.Spawn(isSpawningForFood);
			}
		}
	}

	private void Spawn()
	{
		this.Spawn(false);
	}

	private void Spawn(bool isSpawningForFood)
	{
		if (!this.isMonsterSpawned)
		{
			Transform transform = (isSpawningForFood ? this.foodSpawnPoint : this.spawnPoints.PickRandom<Transform>());
			IceMonster component = global::UnityEngine.Object.Instantiate<GameObject>(this.iceMonsterPrefab, transform.position, transform.rotation).GetComponent<IceMonster>();
			component.Init(base.transform, this.maxRangeFromSpawnAnchor, this.feedingPosition, this.feedingStationCollider);
			component.OnDespawnedAction = (Action<IceMonster, bool>)Delegate.Combine(component.OnDespawnedAction, new Action<IceMonster, bool>(this.OnMonsterDespawned));
			this.isMonsterSpawned = true;
		}
	}

	private void OnMonsterDespawned(IceMonster iceMonster, bool didActuallyHunt)
	{
		iceMonster.OnDespawnedAction = (Action<IceMonster, bool>)Delegate.Remove(iceMonster.OnDespawnedAction, new Action<IceMonster, bool>(this.OnMonsterDespawned));
		this.isMonsterSpawned = false;
		IceMonsterSpawner.AddPostHuntMonsterDelay(iceMonster.cellsOfFishEaten, didActuallyHunt);
	}

	public static void AddPostHuntMonsterDelay(int numCellsOfFishEaten, bool didStartHunting)
	{
		float num = (float)numCellsOfFishEaten * GameManager.Instance.GameConfigData.IceMonsterSpawnDelayPerCellOfFish;
		float num2 = (didStartHunting ? GameManager.Instance.GameConfigData.IceMonsterSpawnDelayBase : GameManager.Instance.GameConfigData.IceMonsterSpawnDelayOnFailedHunt) + num;
		GameManager.Instance.SaveData.IceMonsterTimeUntilSpawn = num2;
	}

	public static void SetDelayForIceMonsterSpawn(float time)
	{
		GameManager.Instance.SaveData.IceMonsterTimeUntilSpawn = time;
	}

	[SerializeField]
	private GameObject iceMonsterPrefab;

	[SerializeField]
	private List<Transform> spawnPoints;

	[SerializeField]
	private Transform foodSpawnPoint;

	[SerializeField]
	private Transform feedingPosition;

	[SerializeField]
	private Collider feedingStationCollider;

	[SerializeField]
	private float maxRangeFromSpawnAnchor;

	[SerializeField]
	private AbilityData banishAbilityData;

	[SerializeField]
	private GridKey linkedGridKey;

	private SerializableGrid grid;

	private bool isMonsterSpawned;

	private bool isPendingSpawnAfterClosingGrid;

	private bool isReady;
}
