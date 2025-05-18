using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalizeFontBypass : MonoBehaviour
{
	private void OnEnable()
	{
		if (GameManager.Instance.LanguageManager != null && !GameManager.Instance.LanguageManager.IsInit)
		{
			LanguageManager languageManager = GameManager.Instance.LanguageManager;
			languageManager.OnInit = (Action)Delegate.Combine(languageManager.OnInit, new Action(this.RefreshFont));
		}
		if (ApplicationEvents.Instance)
		{
			ApplicationEvents.Instance.OnLocaleChanged += this.OnLocaleChanged;
		}
		this.RefreshFont();
	}

	private void OnDisable()
	{
		if (ApplicationEvents.Instance)
		{
			ApplicationEvents.Instance.OnLocaleChanged -= this.OnLocaleChanged;
		}
		if (GameManager.Instance.LanguageManager != null)
		{
			LanguageManager languageManager = GameManager.Instance.LanguageManager;
			languageManager.OnInit = (Action)Delegate.Remove(languageManager.OnInit, new Action(this.RefreshFont));
		}
	}

	private void OnLocaleChanged(Locale locale)
	{
		this.RefreshFont();
	}

	private void RefreshFont()
	{
		bool didHideField = false;
		if (this.textField.enabled)
		{
			this.textField.enabled = false;
			didHideField = true;
		}
		try
		{
			this.textField.font = LocalizationSettings.AssetDatabase.GetLocalizedAsset<TMP_FontAsset>(this.tableString, this.tableEntryString, null);
			if (didHideField)
			{
				this.textField.enabled = true;
			}
		}
		catch (Exception)
		{
			LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<TMP_FontAsset>(this.tableString, this.tableEntryString, null, FallbackBehavior.UseProjectSettings).Completed += delegate(AsyncOperationHandle<TMP_FontAsset> response)
			{
				if (response.Status == AsyncOperationStatus.Succeeded && this.textField != null)
				{
					this.textField.font = response.Result;
					if (didHideField)
					{
						this.textField.enabled = true;
					}
				}
			};
		}
	}

	[SerializeField]
	public TextMeshProUGUI textField;

	[SerializeField]
	public string tableString;

	[SerializeField]
	public string tableEntryString;
}
