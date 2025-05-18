using System;

public class RestDestinationUI : BaseDestinationUI
{
	protected override void OnEnable()
	{
		this.leaveAction.showInControlArea = false;
		base.OnEnable();
	}

	protected override void ShowMainUI()
	{
		this.inputDelaySec = 0f;
		base.ShowMainUI();
		float time = GameManager.Instance.Time.Time;
		float num = 0.25f;
		float num2;
		if (time >= num)
		{
			num2 = num + (1f - time);
		}
		else
		{
			num2 = num - time;
		}
		float num3 = num2 / 0.041666668f;
		GameManager.Instance.Time.ForcefullyPassTime(num3, "feedback.pass-time-rest", TimePassageMode.SLEEP);
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
	}

	private void OnTimeForcefullyPassingChanged(bool isForcefullyPassing, string reason, TimePassageMode mode)
	{
		if (!isForcefullyPassing)
		{
			GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
			base.OnLeavePressComplete();
			GameEvents.Instance.TriggerGameWindowToggled();
		}
	}

	protected override void OnLeavePressComplete()
	{
		GameManager.Instance.Time.StopForcefullyPassingTime();
		base.OnLeavePressComplete();
	}
}
