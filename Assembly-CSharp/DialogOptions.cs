using System;

public struct DialogOptions
{
	public string text;

	public object[] textArguments;

	public DialogButtonOptions[] buttonOptions;

	public bool disableGameCanvas;

	public bool showScrim;

	public bool useDeathScreenPopup;
}
