using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class SKUSpecificLocalizedString : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		if (this.localizedString != null)
		{
			LocalizedString stringOverride = this.GetStringOverride();
			if (stringOverride != null)
			{
				this.localizedString.StringReference = stringOverride;
				this.localizedString.StringReference.RefreshString();
			}
		}
	}

	public LocalizedString GetStringOverride()
	{
		Platform platform = this.SKUSpecificOverrides.Keys.FirstOrDefault((Platform p) => p.HasFlag(Platform.PC_GDK));
		if (platform != Platform.NONE)
		{
			return this.SKUSpecificOverrides[platform];
		}
		return null;
	}

	[SerializeField]
	private LocalizeStringEvent localizedString;

	[SerializeField]
	private Dictionary<Platform, LocalizedString> SKUSpecificOverrides = new Dictionary<Platform, LocalizedString>();
}
