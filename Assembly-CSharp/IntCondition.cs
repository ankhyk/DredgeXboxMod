using System;

public class IntCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		int intVariable = GameManager.Instance.SaveData.GetIntVariable(this.key, 0);
		switch (this.evaluationMode)
		{
		case NumericalEvaluationMode.GT:
			return intVariable > this.target;
		case NumericalEvaluationMode.LTE:
			return intVariable <= this.target;
		case NumericalEvaluationMode.LT:
			return intVariable < this.target;
		case NumericalEvaluationMode.EQUAL:
			return intVariable == this.target;
		}
		return intVariable >= this.target;
	}

	public override string Print()
	{
		return string.Format("IntCondition: {0} [{1} / {2}]", this.Evaluate(), GameManager.Instance.SaveData.GetIntVariable(this.key, 0), this.target);
	}

	public string key;

	public int target;

	public NumericalEvaluationMode evaluationMode;
}
