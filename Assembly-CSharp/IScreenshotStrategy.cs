using System;
using System.Collections;

public interface IScreenshotStrategy
{
	void Init();

	void UnInit();

	void Update();

	IEnumerator DoTakePhoto();
}
