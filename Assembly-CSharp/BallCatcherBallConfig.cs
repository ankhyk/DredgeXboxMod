using System;

[Serializable]
public class BallCatcherBallConfig
{
	public BallCatcherBallDirection direction;

	public float delayBeforeNextBall = 1f;

	public BallCatcherBallType ballType;
}
