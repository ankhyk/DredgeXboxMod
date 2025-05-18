using System;
using System.Collections.Generic;

public class BoolCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		return this.keys.TrueForAll((string key) => saveData.GetBoolVariable(key, false) == this.state);
	}

	public override string Print()
	{
		int count = this.keys.FindAll((string i) => GameManager.Instance.SaveData.GetBoolVariable(i, false) == this.state).Count;
		return string.Format("BoolCondition: {0} [{1} / {2}]", count >= this.keys.Count, count, this.keys.Count);
	}

	public List<string> keys = new List<string>();

	public bool state;
}
