using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class DredgePlayerActionSet
{
	public List<DredgePlayerActionBase> PlayerActions()
	{
		return this.playerActions;
	}

	public bool AddAction(DredgePlayerActionBase newAction)
	{
		bool flag = false;
		if (this.playerActions.IndexOf(newAction) == -1)
		{
			this.playerActions.Add(newAction);
			this.playerActions = this.playerActions.OrderBy((DredgePlayerActionBase p) => p.priority).ToList<DredgePlayerActionBase>();
			flag = true;
		}
		return flag;
	}

	public bool RemoveAction(DredgePlayerActionBase newAction)
	{
		bool flag = false;
		while (this.playerActions.IndexOf(newAction) != -1)
		{
			this.playerActions.Remove(newAction);
			flag = true;
		}
		if (flag)
		{
			this.playerActions = this.playerActions.OrderBy((DredgePlayerActionBase p) => p.priority).ToList<DredgePlayerActionBase>();
		}
		return flag;
	}

	public void ResetAllActions()
	{
		this.playerActions.ForEach(delegate(DredgePlayerActionBase action)
		{
			action.Reset();
		});
	}

	public void Update()
	{
		for (int i = 0; i < this.playerActions.Count; i++)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.playerActions[i];
			if (!GameManager.Instance.IsPaused || dredgePlayerActionBase.evaluateWhenPaused)
			{
				dredgePlayerActionBase.Update();
			}
		}
	}

	public void LateUpdate()
	{
		for (int i = 0; i < this.playerActions.Count; i++)
		{
			this.playerActions[i].LateUpdate();
		}
	}

	private List<DredgePlayerActionBase> playerActions = new List<DredgePlayerActionBase>();
}
