using System;
using UnityEngine;

[Serializable]
public class SettingsSaveData
{
	public GameMode CurrentGameMode()
	{
		if (this.gameMode == 1)
		{
			return GameMode.PASSIVE;
		}
		if (this.gameMode == 2)
		{
			return GameMode.NIGHTMARE;
		}
		return GameMode.NORMAL;
	}

	[SerializeField]
	public int lastSaveSlot;

	[Header("Camera")]
	[SerializeField]
	public float cameraSensitivityX;

	[SerializeField]
	public float cameraSensitivityY;

	[SerializeField]
	public float cameraShakeAmount;

	[SerializeField]
	public int cameraInvertX;

	[SerializeField]
	public int cameraInvertY;

	[SerializeField]
	public int cameraFreelook;

	[SerializeField]
	public int cameraRecenter;

	[SerializeField]
	public int cameraFollow;

	[SerializeField]
	public float spyglassCameraSensitivityX;

	[SerializeField]
	public float spyglassCameraSensitivityY;

	[Header("Accessibility")]
	[SerializeField]
	public int colorNeutral;

	[SerializeField]
	public int colorEmphasis;

	[SerializeField]
	public int colorPositive;

	[SerializeField]
	public int colorNegative;

	[SerializeField]
	public int colorCritical;

	[SerializeField]
	public int colorWarning;

	[SerializeField]
	public int colorValuable;

	[SerializeField]
	public int colorDisabled;

	[SerializeField]
	public int radialTriggerMode;

	[SerializeField]
	public int noFailBehaviour;

	[SerializeField]
	public float turningDeadzoneX;

	[SerializeField]
	public float motionSicknessAmount;

	[SerializeField]
	public int constrainCursor;

	[SerializeField]
	public int panicVisuals;

	[SerializeField]
	public int notificationDuration;

	[SerializeField]
	public int textSpeed;

	[SerializeField]
	public int gameMode;

	[SerializeField]
	public int hasteVFX;

	[SerializeField]
	public int heartbeatSFX;

	[Header("Display")]
	[SerializeField]
	public string localeId = "";

	[SerializeField]
	public int tutorials;

	[SerializeField]
	public int antiAliasing;

	[SerializeField]
	public int units;

	[SerializeField]
	public int clockStyle;

	[SerializeField]
	public int shadowQuality;

	[SerializeField]
	public int reflections;

	[SerializeField]
	public int vsync;

	[SerializeField]
	public int pauseOnFocusLoss;

	[SerializeField]
	public int controlIconStyle;

	[SerializeField]
	public int resx;

	[SerializeField]
	public int resy;

	[SerializeField]
	public int FullScreenMode;

	[SerializeField]
	public int DisplayIndex;

	[Header("Audio")]
	[SerializeField]
	public float masterVolume = 1f;

	[SerializeField]
	public float musicVolume = 1f;

	[SerializeField]
	public float sfxVolume = 1f;

	[SerializeField]
	public float uiVolume = 1f;

	[SerializeField]
	public float voiceVolume = 1f;

	[Header("Controls")]
	[SerializeField]
	public string controlBindings;

	[SerializeField]
	public int titleTheme;

	[SerializeField]
	public bool hasEverTouchedTitleTheme;
}
