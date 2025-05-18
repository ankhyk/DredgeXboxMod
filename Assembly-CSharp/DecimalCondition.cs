using System;

public class DecimalCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		decimal decimalVariable = GameManager.Instance.SaveData.GetDecimalVariable(this.key, 0m);
		switch (this.evaluationMode)
		{
		case NumericalEvaluationMode.GT:
			return decimalVariable > this.target;
		case NumericalEvaluationMode.LTE:
			return decimalVariable <= this.target;
		case NumericalEvaluationMode.LT:
			return decimalVariable < this.target;
		case NumericalEvaluationMode.EQUAL:
			return decimalVariable == this.target;
		}
		return decimalVariable >= this.target;
	}

	public override string Print()
	{
		return string.Format("DecimalCondition: {0} [{1} / {2}]", this.Evaluate(), GameManager.Instance.SaveData.GetDecimalVariable(this.key, 0m), this.target);
	}

	public string key;

	public decimal target;

	public NumericalEvaluationMode evaluationMode;
}
