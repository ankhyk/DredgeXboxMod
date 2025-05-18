using System;

public class LightStrengthCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.PlayerStats.LightLumens >= this.target;
	}

	public override string Print()
	{
		float lightLumens = GameManager.Instance.PlayerStats.LightLumens;
		return string.Format("LightStrengthCondition: {0} [{1} / {2}]", lightLumens >= this.target, lightLumens, this.target);
	}

	public float target;
}
