using System;
using UnityEngine;

public class SKUSpecificDisabler : MonoBehaviour
{
	private void Awake()
	{
		bool flag = true;
		flag = flag && this.supportedPlatforms.HasFlag(Platform.PC_STEAM);
		if (GameManager.Instance.IsRunnningOnSteamDeck && this.unsupportedOnSteamDeck)
		{
			flag = false;
		}
		if (!flag || !this.supportedBuilds.HasFlag(BuildEnvironment.PROD))
		{
			if (this.destroyIfUnavailable)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			base.gameObject.SetActive(false);
		}
	}

	[SerializeField]
	private Platform supportedPlatforms;

	[SerializeField]
	private BuildEnvironment supportedBuilds;

	[SerializeField]
	private bool unsupportedOnSteamDeck;

	[SerializeField]
	private bool allowInConventionBuilds = true;

	[SerializeField]
	private bool destroyIfUnavailable = true;
}
