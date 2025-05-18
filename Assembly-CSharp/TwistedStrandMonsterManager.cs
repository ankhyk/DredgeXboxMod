using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwistedStrandMonsterManager : MonoBehaviour
{
	public bool SuppressSpawns { get; set; }

	public bool IsMonsterSpawned
	{
		get
		{
			return this.isMonsterSpawned;
		}
	}

	private void FindTriggerBoxes()
	{
		this.allTriggerBoxes = global::UnityEngine.Object.FindObjectsOfType<TSMonsterTriggerBox>().ToList<TSMonsterTriggerBox>();
	}

	private void OnEnable()
	{
		this.allTriggerBoxes.ForEach(delegate(TSMonsterTriggerBox b)
		{
			b.OnPlayerDetected = (Action<TSMonsterTriggerBox>)Delegate.Combine(b.OnPlayerDetected, new Action<TSMonsterTriggerBox>(this.OnPlayerDetectedInTriggerBox));
		});
		TSMonster tsmonster = this.monster;
		tsmonster.OnDespawned = (Action)Delegate.Combine(tsmonster.OnDespawned, new Action(this.OnMonsterDespawned));
	}

	private void OnDisable()
	{
		this.allTriggerBoxes.ForEach(delegate(TSMonsterTriggerBox b)
		{
			b.OnPlayerDetected = (Action<TSMonsterTriggerBox>)Delegate.Remove(b.OnPlayerDetected, new Action<TSMonsterTriggerBox>(this.OnPlayerDetectedInTriggerBox));
		});
		TSMonster tsmonster = this.monster;
		tsmonster.OnDespawned = (Action)Delegate.Remove(tsmonster.OnDespawned, new Action(this.OnMonsterDespawned));
	}

	private void OnPlayerDetectedInTriggerBox(TSMonsterTriggerBox box)
	{
		if (GameManager.Instance.PlayerAbilities.GetIsAbilityActive(this.banishAbility))
		{
			return;
		}
		if (this.SuppressSpawns)
		{
			return;
		}
		if (this.isMonsterSpawned)
		{
			return;
		}
		if (GameManager.Instance.DialogueRunner.GetActiveTrapState(box.AssociatedMonsterId) != 0)
		{
			return;
		}
		Transform transform = box.SpawnPointCandidates.PickRandom<Transform>();
		this.monster.transform.position = transform.transform.position;
		this.monster.transform.rotation = transform.transform.rotation;
		this.monster.gameObject.SetActive(true);
		this.isMonsterSpawned = true;
	}

	private void OnMonsterDespawned()
	{
		this.monster.gameObject.SetActive(false);
		this.isMonsterSpawned = false;
	}

	[SerializeField]
	private List<TSMonsterTriggerBox> allTriggerBoxes;

	[SerializeField]
	private TSMonster monster;

	[SerializeField]
	private AbilityData banishAbility;

	private bool isMonsterSpawned;

	private bool dueForRefresh;
}
