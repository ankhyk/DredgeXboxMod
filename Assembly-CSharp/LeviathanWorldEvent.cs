using System;
using System.Collections.Generic;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class LeviathanWorldEvent : WorldEvent
{
	private void OnEnable()
	{
		this.playerTransform = GameManager.Instance.Player.transform;
		this.moveSpeed = this.pathLength / (this.spawnDuration + this.holdDuration + this.despawnDuration);
		this.materialCopy = new Material(this.leviathanMaterial);
		this.materialCopy.name = "leviathan-swim-material-copy";
		this.leviathanMeshRenderer.material = this.materialCopy;
	}

	public override void Activate()
	{
		base.Activate();
		Transform transform = GameManager.Instance.Player.transform;
		Vector3 vector = transform.position + transform.forward * this.intersectionPointOffset.z;
		Vector3 vector2 = vector + transform.right * (this.pathLength * 0.5f);
		Vector3 vector3 = vector - transform.right * (this.pathLength * 0.5f);
		int num = 10;
		bool flag = false;
		int num2 = 18;
		int num3 = 0;
		bool flag2 = false;
		for (int i = 0; i < 10; i++)
		{
			num3 -= num2 * i;
			Vector3 vector4 = VectorUtils.RotatePointAroundPivot(vector2, vector, new Vector3(0f, (float)num3, 0f));
			Vector3 vector5 = VectorUtils.RotatePointAroundPivot(vector3, vector, new Vector3(0f, (float)num3, 0f));
			float num4 = 1f / (float)num;
			for (int j = 0; j <= num; j++)
			{
				Vector3 vector6 = Vector3.Lerp(vector4, vector5, num4 * (float)j);
				if (!this.CheckIsDeepEnough(vector6))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
				vector2 = vector4;
				vector3 = vector5;
				break;
			}
		}
		if (!flag)
		{
			GameManager.Instance.WorldEventManager.OnEventSpawnAborted(base.worldEventData);
			this.RequestEventFinish();
			return;
		}
		base.transform.position = new Vector3(vector2.x, base.transform.position.y, vector2.z);
		this.worldDestPos = vector3;
		this.direction = (vector3 - vector2).normalized;
		Quaternion quaternion = Quaternion.LookRotation(base.transform.position + this.direction - base.transform.position);
		base.transform.rotation = quaternion;
		this.spawnTime = Time.time;
		this.currentState = LeviathanWorldEvent.LeviathanState.SPAWNING;
		this.rumbleAudioSource.PlayOneShot(this.leviathanCallSFX.PickRandom<AudioClip>(), 1f);
		this.altAudioSource.PlayOneShot(this.leviathanAppearSFX, 1f);
		GameManager.Instance.VibrationManager.Vibrate(this.leviathanAppearVibration, VibrationRegion.WholeBody, true).Run();
		GameEvents.Instance.OnTeleportBegin += this.OnPlayerTeleportCast;
	}

	private void OnPlayerTeleportCast()
	{
		this.hasPlayerTeleportedAway = true;
		this.animationEvents.HasPlayerTeleportedAway = true;
		GameEvents.Instance.OnTeleportComplete += this.OnPlayerTeleportComplete;
	}

	private void OnPlayerTeleportComplete()
	{
		GameEvents.Instance.OnTeleportComplete -= this.OnPlayerTeleportComplete;
		this.RequestEventFinish();
	}

	private bool CheckIsDeepEnough(Vector3 testPos)
	{
		return GameManager.Instance.WaveController.SampleWaterDepthAtPosition(testPos) > base.worldEventData.minDepth;
	}

	private void FixedUpdate()
	{
		if (this.currentState == LeviathanWorldEvent.LeviathanState.SPAWNING)
		{
			this.propProgress = (Time.time - this.spawnTime) / this.spawnDuration;
			base.transform.position = new Vector3(base.transform.position.x, this.spawnYPosCurve.Evaluate(this.propProgress), base.transform.position.z);
			this.rumbleAudioSource.volume = Mathf.Lerp(0f, this.maxVolume, this.spawnVolumeCurve.Evaluate(this.propProgress));
			this.wakeAudioSource.volume = Mathf.Lerp(0f, this.maxVolume, this.spawnWakeVolumeCurve.Evaluate(this.propProgress));
			this.materialCopy.SetFloat("ArtificialRefractAmount", this.spawnMaterialVisibilityCurve.Evaluate(this.propProgress));
			if (this.propProgress >= 1f)
			{
				this.holdTime = Time.time;
				this.currentState = LeviathanWorldEvent.LeviathanState.HOLDING;
			}
		}
		else if (this.currentState == LeviathanWorldEvent.LeviathanState.DESPAWNING)
		{
			this.propProgress = (Time.time - this.despawnTime) / this.despawnDuration;
			base.transform.position = new Vector3(base.transform.position.x, this.despawnYPosCurve.Evaluate(this.propProgress), base.transform.position.z);
			this.rumbleAudioSource.volume = Mathf.Lerp(0f, this.maxVolume, this.despawnVolumeCurve.Evaluate(this.propProgress));
			this.wakeAudioSource.volume = Mathf.Lerp(0f, this.maxVolume, this.despawnWakeVolumeCurve.Evaluate(this.propProgress));
			this.materialCopy.SetFloat("ArtificialRefractAmount", this.despawnMaterialVisibilityCurve.Evaluate(this.propProgress));
			if (this.propProgress >= 1f)
			{
				if (this.attackAfterDive && this.CheckIsDeepEnough(this.playerTransform.position) && !base.worldEventData.forbiddenZones.HasFlag(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone()) && !GameManager.Instance.WorldEventManager.DoesHitSafeZone(GameManager.Instance.Player.transform.position))
				{
					this.currentState = LeviathanWorldEvent.LeviathanState.ATTACKING;
					this.animator.SetTrigger("attack");
					this.altAudioSource.PlayOneShot(this.leviathanAttackSFX, 1f);
					GameManager.Instance.VibrationManager.Vibrate(this.leviathanSwallowedVibration, VibrationRegion.WholeBody, true).Run();
				}
				else
				{
					this.RequestEventFinish();
				}
			}
		}
		else if (this.currentState == LeviathanWorldEvent.LeviathanState.HOLDING)
		{
			this.propProgress = (Time.time - this.holdTime) / this.holdDuration;
			base.transform.position = new Vector3(base.transform.position.x, this.holdYPosCurve.Evaluate(this.propProgress), base.transform.position.z);
			if (this.propProgress >= 1f)
			{
				this.despawnTime = Time.time;
				this.currentState = LeviathanWorldEvent.LeviathanState.DESPAWNING;
			}
		}
		if (this.currentState == LeviathanWorldEvent.LeviathanState.ATTACKING)
		{
			if (!this.hasPlayerTeleportedAway)
			{
				base.transform.position = this.playerTransform.position;
				base.transform.rotation = Quaternion.LookRotation(this.playerTransform.position - this.worldDestPos);
				return;
			}
		}
		else
		{
			base.transform.position += this.direction * this.moveSpeed * Time.deltaTime;
			Quaternion quaternion = Quaternion.LookRotation(base.transform.position + this.direction - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.turnSpeed * Time.deltaTime);
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.EventFinished();
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnTeleportBegin -= this.OnPlayerTeleportCast;
	}

	[SerializeField]
	private LeviathanAnimationEvents animationEvents;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioClip leviathanAppearSFX;

	[SerializeField]
	private AudioClip leviathanAttackSFX;

	[SerializeField]
	private List<AudioClip> leviathanCallSFX;

	[SerializeField]
	private AudioSource rumbleAudioSource;

	[SerializeField]
	private AudioSource wakeAudioSource;

	[SerializeField]
	private AudioSource altAudioSource;

	[SerializeField]
	private AnimationCurve spawnYPosCurve;

	[SerializeField]
	private AnimationCurve spawnMaterialVisibilityCurve;

	[SerializeField]
	private AnimationCurve spawnVolumeCurve;

	[SerializeField]
	private AnimationCurve spawnWakeVolumeCurve;

	[SerializeField]
	private AnimationCurve holdYPosCurve;

	[SerializeField]
	private AnimationCurve despawnYPosCurve;

	[SerializeField]
	private AnimationCurve despawnMaterialVisibilityCurve;

	[SerializeField]
	private AnimationCurve despawnVolumeCurve;

	[SerializeField]
	private AnimationCurve despawnWakeVolumeCurve;

	[SerializeField]
	private float spawnDuration;

	[SerializeField]
	private float holdDuration;

	[SerializeField]
	private float despawnDuration;

	[SerializeField]
	private float turnSpeed;

	[SerializeField]
	private float pathLength;

	[SerializeField]
	private float maxVolume;

	[SerializeField]
	private bool attackAfterDive;

	[SerializeField]
	private Vector3 intersectionPointOffset;

	[SerializeField]
	private SkinnedMeshRenderer leviathanMeshRenderer;

	[SerializeField]
	private Material leviathanMaterial;

	[SerializeField]
	private VibrationData leviathanAppearVibration;

	[SerializeField]
	private VibrationData leviathanAttackVibration;

	[SerializeField]
	private VibrationData leviathanSwallowedVibration;

	private float moveSpeed;

	private LeviathanWorldEvent.LeviathanState currentState;

	private Vector3 direction;

	private Vector3 worldDestPos;

	private float spawnTime;

	private float holdTime;

	private float despawnTime;

	private float propProgress;

	private Material materialCopy;

	private Transform playerTransform;

	private bool hasPlayerTeleportedAway;

	private enum LeviathanState
	{
		NONE,
		SPAWNING,
		HOLDING,
		DESPAWNING,
		ATTACKING
	}
}
