using System;
using UnityEngine;

public class RealPlayerProxy : PlayerProxy
{
	public override Vector3 GetPlayerPosition()
	{
		GameManager instance = GameManager.Instance;
		if ((instance != null) ? instance.Player : null)
		{
			return GameManager.Instance.Player.transform.position;
		}
		if (this._backupPlayerPos)
		{
			return this._backupPlayerPos.position;
		}
		return Vector3.zero;
	}

	[SerializeField]
	[Tooltip("This value is used in editor (or when a save can't be loaded)")]
	private Transform _backupPlayerPos;
}
