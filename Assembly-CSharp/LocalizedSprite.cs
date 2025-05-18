using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizedSprite : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnLocaleChanged += this.OnLocaleChanged;
		this.OnLocaleChanged(LocalizationSettings.SelectedLocale);
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnLocaleChanged -= this.OnLocaleChanged;
	}

	private void OnLocaleChanged(Locale locale)
	{
		Sprite sprite = this.defaultSprite;
		if (this.spriteOverrides.ContainsKey(locale))
		{
			sprite = this.spriteOverrides[locale];
		}
		this.image.sprite = sprite;
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private Dictionary<Locale, Sprite> spriteOverrides;
}
