using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

[CreateAssetMenu(fileName = "MouseControlIconData", menuName = "Dredge/MouseControlIconData", order = 0)]
public class MouseControlIconData : BaseControlIconData
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

	public Dictionary<Mouse, ControlIconData> controlIcons = new Dictionary<Mouse, ControlIconData>();
}
