using System;

public class FishingSpeedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.PlayerStats.FishingSpeedModifier >= this.target;
	}

	public override string Print()
	{
		float fishingSpeedModifier = GameManager.Instance.PlayerStats.FishingSpeedModifier;
		return string.Format("FishingSpeedCondition: {0} [{1} / {2}]", fishingSpeedModifier >= this.target, fishingSpeedModifier, this.target);
	}

	public float target;
}
