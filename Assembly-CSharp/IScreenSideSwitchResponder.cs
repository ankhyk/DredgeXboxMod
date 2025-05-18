using System;

public interface IScreenSideSwitchResponder
{
	void SwitchToSide();

	void ToggleSwitchIcon(bool show);

	bool GetCanSwitchToThisIfHoldingItem();
}
