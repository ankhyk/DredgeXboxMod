using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Yarn.Unity;

public class TextOptionButton : MonoBehaviour
{
	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	public bool HasQuickExit
	{
		get
		{
			return this.hasQuickExit;
		}
	}

	public void Init(DialogueOption opt, int index, int shownIndex, Action<int> onClick)
	{
		this.dialogueOption = opt;
		string text = LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.YARN_TABLE, this.dialogueOption.TextID, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		text = text.Replace("\\", "");
		text = text.TrimStart(TextOptionButton.trimChars);
		this.textField.text = text;
		this.hasQuickExit = this.dialogueOption.Line.Metadata != null && this.dialogueOption.Line.Metadata.Contains("exit");
		bool flag = this.dialogueOption.Line.Metadata != null && this.dialogueOption.Line.Metadata.Contains("repeat");
		bool flag2 = this.dialogueOption.Line.Metadata != null && this.dialogueOption.Line.Metadata.Contains("paint");
		bool flag3 = this.dialogueOption.Line.Metadata != null && this.dialogueOption.Line.Metadata.Contains("quest");
		bool flag4 = this.dialogueOption.Line.Metadata != null && this.dialogueOption.Line.Metadata.Contains("always-mark-visited");
		bool flag5 = GameManager.Instance.SaveData.visitedNodes.Contains(this.dialogueOption.TextID);
		if (flag3 && !flag && flag5)
		{
			flag3 = false;
		}
		this.attentionCallout.SetActive(flag3);
		if ((!flag || flag4) && flag5 && !this.hasQuickExit)
		{
			ColorBlock colors = this.basicButtonWrapper.Button.colors;
			colors.normalColor = Color.gray;
			colors.pressedColor = Color.gray;
			this.basicButtonWrapper.Button.colors = colors;
		}
		if (this.hasQuickExit)
		{
			this.iconImage.sprite = this.exitIcon;
		}
		else if (flag2)
		{
			this.iconImage.sprite = this.paintIcon;
		}
		this.iconImage.gameObject.SetActive(this.hasQuickExit || flag2);
		this.basicButtonWrapper.transitionInDelaySec = (float)shownIndex * this.perOptionAppearDelaySec;
		this.basicButtonWrapper.Interactable = this.dialogueOption.IsAvailable;
		this.cachedIndex = index;
		this.onClick = onClick;
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.DoSelect));
	}

	public void DoSelect()
	{
		if (!GameManager.Instance.SaveData.visitedNodes.Contains(this.dialogueOption.TextID))
		{
			GameManager.Instance.SaveData.visitedNodes.Add(this.dialogueOption.TextID);
		}
		Action<int> action = this.onClick;
		if (action == null)
		{
			return;
		}
		action(this.cachedIndex);
	}

	[SerializeField]
	private float perOptionAppearDelaySec;

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private Sprite exitIcon;

	[SerializeField]
	private Sprite paintIcon;

	[SerializeField]
	private GameObject attentionCallout;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	private int cachedIndex;

	private bool hasQuickExit;

	private static char[] trimChars = new char[] { ' ' };

	private Action<int> onClick;

	private DialogueOption dialogueOption;
}
