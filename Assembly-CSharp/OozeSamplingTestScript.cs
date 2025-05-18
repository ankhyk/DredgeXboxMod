using System;
using UnityEngine;

public class OozeSamplingTestScript : MonoBehaviour
{
	private void Update()
	{
		OozePatch oozePatch = null;
		GameManager.Instance.OozePatchManager.TryGetOozePatchAtPosition(base.transform.position, out oozePatch);
	}
}
