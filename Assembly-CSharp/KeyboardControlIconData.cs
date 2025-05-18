using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyboardControlIconData", menuName = "Dredge/KeyboardControlIconData", order = 0)]
public class KeyboardControlIconData : BaseControlIconData
{
	public void GenerateNames()
	{
		this.controlIcons.Values.ToList<ControlIconData>().ForEach(delegate(ControlIconData d)
		{
			if (d.upSprite)
			{
				d.upSpriteName = d.upSprite.name;
			}
			if (d.downSprite)
			{
				d.downSpriteName = d.downSprite.name;
			}
		});
	}

	public Dictionary<Key, ControlIconData> controlIcons = new Dictionary<Key, ControlIconData>();
}
