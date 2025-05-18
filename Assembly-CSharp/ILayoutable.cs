using System;
using UnityEngine;

public interface ILayoutable
{
	bool IsLayedOut { get; }

	GameObject GameObject { get; }
}
