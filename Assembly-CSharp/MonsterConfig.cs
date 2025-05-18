using System;
using UnityEngine;

[Serializable]
public class MonsterConfig
{
	public MonsterData monsterData;

	public Transform[] path;

	public int maxSpawned;
}
