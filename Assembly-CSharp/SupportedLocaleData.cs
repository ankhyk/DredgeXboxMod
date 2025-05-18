using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SupportedLocaleData", menuName = "Dredge/SupportedLocaleData", order = 0)]
public class SupportedLocaleData : SerializedScriptableObject
{
	[SerializeField]
	public List<Locale> locales;

	[SerializeField]
	public Locale testLocale;

	[SerializeField]
	public Dictionary<string, string> nativeLanguageNameOverrides = new Dictionary<string, string>();

	[SerializeField]
	public Dictionary<Locale, TMP_FontAsset> fonts = new Dictionary<Locale, TMP_FontAsset>();
}
