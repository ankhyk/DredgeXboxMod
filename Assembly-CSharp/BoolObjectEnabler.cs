using System;
using UnityEngine;

public class BoolObjectEnabler : MonoBehaviour
{
	private void OnEnable()
	{
		bool boolVariable = GameManager.Instance.SaveData.GetBoolVariable(this.boolKey, false);
		if (this.trueObject)
		{
			this.trueObject.SetActive(boolVariable);
		}
		if (this.falseObject)
		{
			this.falseObject.SetActive(!boolVariable);
		}
	}

	[SerializeField]
	private string boolKey;

	[SerializeField]
	private GameObject trueObject;

	[SerializeField]
	private GameObject falseObject;
}
