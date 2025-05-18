using System;
using UnityEngine;
using UnityEngine.Localization;

public class TextTooltipRequester : TooltipRequester
{
	public LocalizedString LocalizedTitleKey
	{
		get
		{
			return this.localizedTitleKey;
		}
		set
		{
			this.localizedTitleKey = value;
		}
	}

	public LocalizedString LocalizedDescriptionKey
	{
		get
		{
			return this.localizedDescriptionKey;
		}
		set
		{
			this.localizedDescriptionKey = value;
		}
	}

	public Color TitleTextColor
	{
		get
		{
			return this.titleTextColor;
		}
	}

	public Color DescriptionTextColor
	{
		get
		{
			return this.descriptionTextColor;
		}
	}

	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	[SerializeField]
	private LocalizedString localizedTitleKey;

	[SerializeField]
	private LocalizedString localizedDescriptionKey;

	[SerializeField]
	private Color titleTextColor = Color.white;

	[SerializeField]
	private Color descriptionTextColor = Color.white;

	[SerializeField]
	private Sprite icon;
}
