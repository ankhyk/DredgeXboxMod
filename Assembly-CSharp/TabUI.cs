using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TabUI : MonoBehaviour
{
	public Button Button
	{
		get
		{
			return this.button;
		}
	}

	public Sprite SelectedSprite
	{
		get
		{
			return this.selectedSprite;
		}
	}

	public Sprite UnselectedSprite
	{
		get
		{
			return this.unselectedSprite;
		}
	}

	public Sprite LockedSprite
	{
		get
		{
			return this.lockedSprite;
		}
	}

	public TabShowQuery TabShowQuery
	{
		get
		{
			return this.tabShowQuery;
		}
	}

	public Image TabImage
	{
		get
		{
			return this.tabImage;
		}
	}

	public LocalizeStringEvent TabLocalizedString
	{
		get
		{
			return this.tabLocalizedString;
		}
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private TabShowQuery tabShowQuery;

	[Header("Config")]
	[SerializeField]
	private Sprite selectedSprite;

	[SerializeField]
	private Sprite unselectedSprite;

	[SerializeField]
	private Sprite lockedSprite;

	[SerializeField]
	private Image tabImage;

	[SerializeField]
	private LocalizeStringEvent tabLocalizedString;
}
