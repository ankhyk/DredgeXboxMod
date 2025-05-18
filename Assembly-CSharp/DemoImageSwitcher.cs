using System;
using UnityEngine;
using UnityEngine.UI;

public class DemoImageSwitcher : MonoBehaviour
{
	private void Awake()
	{
		this.image.sprite = this.regularSprite;
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite regularSprite;

	[SerializeField]
	private Sprite demoSprite;
}
