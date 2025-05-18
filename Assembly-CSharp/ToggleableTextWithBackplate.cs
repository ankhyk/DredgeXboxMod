using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleableTextWithBackplate : MonoBehaviour
{
	public Color IdleBackplateColor
	{
		get
		{
			return this.idleBackplateColor;
		}
		set
		{
			this.idleBackplateColor = value;
		}
	}

	public Color HighlightedBackplateColor
	{
		get
		{
			return this.highlightedBackplateColor;
		}
		set
		{
			this.highlightedBackplateColor = value;
		}
	}

	public Color IdleTextColor
	{
		get
		{
			return this.idleTextColor;
		}
		set
		{
			this.idleTextColor = value;
		}
	}

	public Color HighlightedTextColor
	{
		get
		{
			return this.highlightedTextColor;
		}
		set
		{
			this.highlightedTextColor = value;
		}
	}

	public void SetHighlighted(bool highlighted)
	{
		this.text.color = (highlighted ? this.highlightedTextColor : this.idleTextColor);
		this.backplate.color = (highlighted ? this.highlightedBackplateColor : this.idleBackplateColor);
	}

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private Image backplate;

	[SerializeField]
	private Color idleTextColor;

	[SerializeField]
	private Color highlightedTextColor;

	[SerializeField]
	private Color idleBackplateColor;

	[SerializeField]
	private Color highlightedBackplateColor;
}
