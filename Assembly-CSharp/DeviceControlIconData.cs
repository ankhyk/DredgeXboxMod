using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

[CreateAssetMenu(fileName = "DeviceControlIconData", menuName = "Dredge/DeviceControlIconData", order = 0)]
public class DeviceControlIconData : BaseControlIconData
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

	public Dictionary<InputControlType, ControlIconData> controlIcons = new Dictionary<InputControlType, ControlIconData>();
}
