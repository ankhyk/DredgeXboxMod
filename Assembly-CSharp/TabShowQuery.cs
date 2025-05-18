using System;
using UnityEngine;

public class TabShowQuery : MonoBehaviour
{
	public virtual bool GetCanNavigate()
	{
		return true;
	}

	public virtual bool GetCanShow()
	{
		return true;
	}

	public Action<bool> canNavigateChanged;

	public Action<bool> canShowChanged;
}
