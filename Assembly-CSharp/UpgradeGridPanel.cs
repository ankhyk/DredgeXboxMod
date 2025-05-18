using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class UpgradeGridPanel : QuestGridPanel
{
	public void Show(UpgradeData upgradeData)
	{
		base.Show(upgradeData.GridConfig, true);
		this.upgradeData = upgradeData;
		this.OnStateChanged();
		BasicButtonWrapper basicButtonWrapper = this.bottomButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnPurchaseButtonClicked));
		GameEvents.Instance.TriggerUpgradePreviewed(upgradeData);
		GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
		this.OnPlayerFundsChanged(GameManager.Instance.SaveData.Funds, 0m);
		object[] array = null;
		if (upgradeData is HullUpgradeData)
		{
			array = new object[]
			{
				upgradeData.tier,
				upgradeData.GetNewCellCount()
			};
		}
		else if (upgradeData is SlotUpgradeData)
		{
			array = new object[] { upgradeData.GetNewCellCount() };
		}
		this.localizedDescriptionField.enabled = false;
		this.localizedDescriptionField.StringReference = upgradeData.DescriptionKey;
		this.localizedDescriptionField.StringReference.Arguments = array;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.localizedDescriptionField.enabled = true;
	}

	private void OnPlayerFundsChanged(decimal total, decimal change)
	{
		string text = "$" + this.upgradeData.MonetaryCost.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		string text2;
		if (total >= this.upgradeData.MonetaryCost)
		{
			text2 = text;
		}
		else
		{
			text2 = string.Concat(new string[]
			{
				"<color=#",
				GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
				">",
				text,
				"</color>"
			});
		}
		this.bottomButton.LocalizedString.StringReference.Arguments = new string[] { text2 };
		this.bottomButton.LocalizedString.StringReference.RefreshString();
	}

	public override void Hide()
	{
		GameEvents.Instance.TriggerUpgradePreviewed(null);
		GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
		BasicButtonWrapper basicButtonWrapper = this.bottomButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnPurchaseButtonClicked));
		this.bottomButton.Interactable = false;
		base.Hide();
	}

	private void OnPurchaseButtonClicked()
	{
		if (!base.SlidePanel.IsShowing)
		{
			return;
		}
		if (this.upgradeData is HullUpgradeData && GameManager.Instance.SaveData.GetNumberOfDamagedSlots() > 0)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.upgrade-hull-damaged");
			return;
		}
		if (this.upgradeData is HullUpgradeData && GameManager.Instance.SaveData.OverflowStorage.spatialItems.Count > 0)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.upgrade.items-in-overflow");
			return;
		}
		if (GameManager.Instance.SaveData.Funds >= this.upgradeData.MonetaryCost)
		{
			GameManager.Instance.UpgradeManager.AddUpgrade(this.upgradeData, false);
			GameManager.Instance.AudioPlayer.PlaySFX(this.upgradeCompleteSFX, AudioLayer.SFX_UI, 1f, 1f);
			Action<int> exitEvent = this.ExitEvent;
			if (exitEvent == null)
			{
				return;
			}
			exitEvent(1);
		}
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		bool flag = this.result == QuestGridResult.COMPLETE && !GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully() && GameManager.Instance.SaveData.Funds >= this.upgradeData.MonetaryCost;
		this.bottomButton.Interactable = flag;
	}

	[SerializeField]
	private AssetReference upgradeCompleteSFX;

	[SerializeField]
	private BasicButtonWrapper bottomButton;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	private UpgradeData upgradeData;
}
