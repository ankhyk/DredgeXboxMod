using System;
using UnityEngine.UI;

public class SelectableWithNavigationPriority : BasicButton
{
	public override Selectable FindSelectableOnUp()
	{
		if (!(this.upSelectable != null))
		{
			return base.FindSelectableOnUp();
		}
		return this.upSelectable;
	}

	public override Selectable FindSelectableOnDown()
	{
		if (!(this.downSelectable != null))
		{
			return base.FindSelectableOnDown();
		}
		return this.downSelectable;
	}

	public override Selectable FindSelectableOnLeft()
	{
		if (!(this.leftSelectable != null))
		{
			return base.FindSelectableOnLeft();
		}
		return this.leftSelectable;
	}

	public override Selectable FindSelectableOnRight()
	{
		if (!(this.rightSelectable != null))
		{
			return base.FindSelectableOnRight();
		}
		return this.rightSelectable;
	}

	public Selectable upSelectable;

	public Selectable downSelectable;

	public Selectable leftSelectable;

	public Selectable rightSelectable;
}
