using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class DockProgressUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.progressImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
	}

	public void Init(DockData dockData)
	{
		if (dockData.DockProgressType == DockProgressType.NONE)
		{
			this.container.SetActive(false);
			return;
		}
		decimal num = 0m;
		decimal num2 = 0m;
		decimal num3 = 0m;
		bool flag = false;
		if (dockData.DockProgressType == DockProgressType.GM_REPAYMENTS)
		{
			flag = GameManager.Instance.SaveData.GetBoolVariable("gm-debt-introduced", false);
			num = GameManager.Instance.SaveData.GreaterMarrowRepayments;
			num2 = GameManager.Instance.GameConfigData.GreaterMarrowDebt;
		}
		num3 = (decimal)Mathf.Clamp((float)(num2 - num), 0f, (float)num2);
		this.progressValueLocalizeString.StringReference.Arguments = new object[] { "$" + num3.ToString("n2", LocalizationSettings.SelectedLocale.Formatter) };
		if (!flag || num >= num2)
		{
			this.container.SetActive(false);
			return;
		}
		float num4 = (float)(num / num2);
		this.progressImage.fillAmount = num4;
		this.progressTitleLocalizeString.StringReference.SetReference(LanguageManager.STRING_TABLE, dockData.ProgressTitleLocalizationKey);
		this.progressValueLocalizeString.StringReference.SetReference(LanguageManager.STRING_TABLE, dockData.ProgressValueLocalizationKey);
		this.transitionEffect.effectFactor = 0f;
		this.container.SetActive(true);
		this.transitionEffect.Show(true);
	}

	public void Hide()
	{
		this.container.SetActive(false);
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private LocalizeStringEvent progressTitleLocalizeString;

	[SerializeField]
	private LocalizeStringEvent progressValueLocalizeString;

	[SerializeField]
	private Image progressImage;

	[SerializeField]
	private UITransitionEffect transitionEffect;
}
