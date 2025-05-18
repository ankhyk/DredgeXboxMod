using System;

public class FloatCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		float floatVariable = GameManager.Instance.SaveData.GetFloatVariable(this.key, 0f);
		switch (this.evaluationMode)
		{
		case NumericalEvaluationMode.GT:
			return floatVariable > this.target;
		case NumericalEvaluationMode.LTE:
			return floatVariable <= this.target;
		case NumericalEvaluationMode.LT:
			return floatVariable < this.target;
		case NumericalEvaluationMode.EQUAL:
			return floatVariable == this.target;
		}
		return floatVariable >= this.target;
	}

	public override string Print()
	{
		return string.Format("FloatCondition: {0} [{1} / {2}]", this.Evaluate(), GameManager.Instance.SaveData.GetFloatVariable(this.key, 0f), this.target);
	}

	public string key;

	public float target;

	public NumericalEvaluationMode evaluationMode;
}
