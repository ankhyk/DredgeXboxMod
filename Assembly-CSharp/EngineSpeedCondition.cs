using System;

public class EngineSpeedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.PlayerStats.MovementSpeedModifier >= this.target;
	}

	public override string Print()
	{
		float movementSpeedModifier = GameManager.Instance.PlayerStats.MovementSpeedModifier;
		return string.Format("EngineSpeedCondition: {0} [{1} / {2}]", movementSpeedModifier >= this.target, movementSpeedModifier, this.target);
	}

	public float target;
}
