using System;

public class BooleanSetWorldStateAction : WorldStateAction
{
	public override void Do()
	{
		GameManager.Instance.SaveData.SetBoolVariable(this.boolName, this.enabledIfMet);
	}

	public string boolName;

	public bool enabledIfMet;
}
