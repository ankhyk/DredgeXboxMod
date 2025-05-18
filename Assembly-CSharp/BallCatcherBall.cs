using System;
using UnityEngine;
using UnityEngine.UI;

public class BallCatcherBall : MonoBehaviour
{
	public float TransformedAngle
	{
		get
		{
			return MathUtil.TransformAngleToNegative180Positive180(base.transform.eulerAngles.z);
		}
	}

	public BallCatcherBallType BallType
	{
		get
		{
			return this.ballType;
		}
	}

	public BallCatcherBallDirection Direction
	{
		get
		{
			return this.direction;
		}
	}

	public bool HasGonePastTargetZone
	{
		get
		{
			return this.hasGonePastTargetZone;
		}
	}

	public void Init(BallCatcherBallDirection direction, BallCatcherBallType ballType, float speed, float targetZoneAngleMin, float targetZoneAngleMax)
	{
		this.direction = direction;
		this.speed = speed;
		if (direction == BallCatcherBallDirection.LEFT)
		{
			this.speed = -speed;
			this.targetZoneEdge = targetZoneAngleMin;
			base.transform.eulerAngles = new Vector3(0f, 0f, this.launcherAngle);
		}
		else if (direction == BallCatcherBallDirection.RIGHT)
		{
			this.targetZoneEdge = targetZoneAngleMax;
			base.transform.eulerAngles = new Vector3(0f, 0f, -this.launcherAngle);
		}
		this.SetBallType(ballType);
	}

	public void SetBallType(BallCatcherBallType ballType)
	{
		this.ballType = ballType;
		if (ballType == BallCatcherBallType.OBSTACLE)
		{
			this.ballImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			this.ballImage.sprite = this.negativeBallSprite;
			return;
		}
		if (ballType == BallCatcherBallType.TARGET)
		{
			this.ballImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			this.ballImage.sprite = this.regularBallSprite;
			return;
		}
		if (ballType == BallCatcherBallType.SPECIAL)
		{
			this.ballImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE);
			this.ballImage.sprite = this.regularBallSprite;
		}
	}

	private void Update()
	{
		this.cachedTransformedAngle = this.TransformedAngle;
		if (!this.hasGonePastTargetZone)
		{
			if (this.direction == BallCatcherBallDirection.LEFT && this.cachedTransformedAngle < this.targetZoneEdge)
			{
				this.hasGonePastTargetZone = true;
				this.Deactivate();
			}
			else if (this.direction == BallCatcherBallDirection.RIGHT && this.cachedTransformedAngle > this.targetZoneEdge)
			{
				this.hasGonePastTargetZone = true;
				this.Deactivate();
			}
		}
		if (!this.hasReachedOtherSide)
		{
			base.transform.Rotate(new Vector3(0f, 0f, this.speed * Time.deltaTime), Space.Self);
			if (this.direction == BallCatcherBallDirection.LEFT && this.cachedTransformedAngle < -this.launcherAngle)
			{
				this.hasReachedOtherSide = true;
				Action<BallCatcherBall> onReachedOtherSide = this.OnReachedOtherSide;
				if (onReachedOtherSide == null)
				{
					return;
				}
				onReachedOtherSide(this);
				return;
			}
			else if (this.direction == BallCatcherBallDirection.RIGHT && this.cachedTransformedAngle > this.launcherAngle)
			{
				this.hasReachedOtherSide = true;
				Action<BallCatcherBall> onReachedOtherSide2 = this.OnReachedOtherSide;
				if (onReachedOtherSide2 == null)
				{
					return;
				}
				onReachedOtherSide2(this);
			}
		}
	}

	public void OnInputDisabled()
	{
		if (!this.hasGonePastTargetZone)
		{
			this.ballImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
	}

	public void OnInputReenabled()
	{
		if (!this.hasGonePastTargetZone)
		{
			this.SetBallType(this.ballType);
		}
	}

	private void Deactivate()
	{
		this.ballImage.color = new Color(1f, 1f, 1f, 0.15f);
	}

	[SerializeField]
	private Image ballImage;

	[SerializeField]
	private Sprite regularBallSprite;

	[SerializeField]
	private Sprite negativeBallSprite;

	[SerializeField]
	private float launcherAngle = 165f;

	private BallCatcherBallDirection direction;

	private BallCatcherBallType ballType;

	private float speed;

	public Action<BallCatcherBall> OnReachedOtherSide;

	private float threshold = 5f;

	private bool hasReachedOtherSide;

	private bool hasGonePastTargetZone;

	private float targetZoneEdge;

	private float cachedTransformedAngle;
}
