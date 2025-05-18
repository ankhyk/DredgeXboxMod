using System;
using UnityEngine;

public class ControlPromptIconWrapper : MonoBehaviour
{
	private void Awake()
	{
		this.controlPromptIcon.Init(null, GameManager.Instance.Input.Controls.GetPlayerAction(this.controlEnum));
	}

	[SerializeField]
	private ControlPromptIcon controlPromptIcon;

	[SerializeField]
	private DredgeControlEnum controlEnum;
}
