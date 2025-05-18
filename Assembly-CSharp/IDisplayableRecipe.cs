using System;
using UnityEngine;
using UnityEngine.Localization;

public interface IDisplayableRecipe
{
	Sprite GetSprite();

	int GetWidth();

	int GetHeight();

	LocalizedString GetItemNameKey();

	int GetQuantityProduced();
}
