using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleableImageWithBackplate : MonoBehaviour
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

	public Color IdleImageColor
	{
		get
		{
			return this.idleImageColor;
		}
		set
		{
			this.idleImageColor = value;
		}
	}

	public Color HighlightedImageColor
	{
		get
		{
			return this.highlightedImageColor;
		}
		set
		{
			this.highlightedImageColor = value;
		}
	}

	public void SetHighlighted(bool highlighted)
	{
		this.image.color = (highlighted ? this.highlightedImageColor : this.idleImageColor);
		this.backplate.color = (highlighted ? this.highlightedBackplateColor : this.idleBackplateColor);
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image backplate;

	[SerializeField]
	private Color idleImageColor;

	[SerializeField]
	private Color highlightedImageColor;

	[SerializeField]
	private Color idleBackplateColor;

	[SerializeField]
	private Color highlightedBackplateColor;
}
