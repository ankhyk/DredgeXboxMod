using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class VersionLabelUI : MonoBehaviour
{
	private void Awake()
	{
		this.textField.text = "";
		BuildInfo buildInfo = GameManager.Instance.BuildInfo;
		string text = "prod";
		if (buildInfo)
		{
			this.textField.text = string.Format("v{0}.{1}.{2} {3} | build {4}", new object[] { buildInfo.VersionMajor, buildInfo.VersionMinor, buildInfo.VersionRevision, text, buildInfo.BuildNumber });
		}
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private AssetReference buildInfoReference;
}
