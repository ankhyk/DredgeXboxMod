using System;
using UnityEngine;

public class TrawlLineUpdater : MonoBehaviour
{
	private void Update()
	{
		this.line1.SetPosition(0, this.NetL.position);
		this.line1.SetPosition(1, this.TrawlL.position);
		this.line2.SetPosition(0, this.NetR.position);
		this.line2.SetPosition(1, this.TrawlR.position);
	}

	[SerializeField]
	private LineRenderer line1;

	[SerializeField]
	private LineRenderer line2;

	[SerializeField]
	private Transform NetL;

	[SerializeField]
	private Transform NetR;

	[SerializeField]
	private Transform TrawlL;

	[SerializeField]
	private Transform TrawlR;
}
