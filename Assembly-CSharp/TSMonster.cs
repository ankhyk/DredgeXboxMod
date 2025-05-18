using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TSMonster : MonoBehaviour, IGameModeResponder
{
	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && this.currentState == TSMonsterState.DRAINING)
		{
			this.Despawn();
		}
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnDialogueStarted += this.Despawn;
		TSMonsterAnimationEvents tsmonsterAnimationEvents = this.animationEvents;
		tsmonsterAnimationEvents.EmergeCompleteAction = (Action)Delegate.Combine(tsmonsterAnimationEvents.EmergeCompleteAction, new Action(this.OnEmergeComplete));
		TSMonsterAnimationEvents tsmonsterAnimationEvents2 = this.animationEvents;
		tsmonsterAnimationEvents2.PeekCompleteAction = (Action)Delegate.Combine(tsmonsterAnimationEvents2.PeekCompleteAction, new Action(this.OnPeekComplete));
		this.Reset();
		this.spawnPos = base.transform.position;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnDialogueStarted -= this.Despawn;
		TSMonsterAnimationEvents tsmonsterAnimationEvents = this.animationEvents;
		tsmonsterAnimationEvents.EmergeCompleteAction = (Action)Delegate.Remove(tsmonsterAnimationEvents.EmergeCompleteAction, new Action(this.OnEmergeComplete));
		TSMonsterAnimationEvents tsmonsterAnimationEvents2 = this.animationEvents;
		tsmonsterAnimationEvents2.PeekCompleteAction = (Action)Delegate.Remove(tsmonsterAnimationEvents2.PeekCompleteAction, new Action(this.OnPeekComplete));
	}

	private void Reset()
	{
		this.timeSpentPeeking = 0f;
		this.timeSpentDetectingPlayer = 0f;
		this.timeSpentLostPlayer = 0f;
		this.timeUntilSearchExpires = this.searchTimeUntilGiveUpSec;
		this.timeUntilRepath = this.timeBetweenRepaths;
		this.timeSpentDraining = 0f;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (enabled && ability.name == this.banishAbility.name)
		{
			GameEvents.Instance.TriggerThreatBanished(true);
			this.Despawn();
		}
	}

	private void Despawn()
	{
		this.currentState = TSMonsterState.DESPAWNING;
		this.animator.SetTrigger("despawn");
		GameManager.Instance.VibrationManager.Vibrate(this.spawnVibration, VibrationRegion.WholeBody, true);
		base.Invoke("OnDespawnComplete", 2.5f);
	}

	private void OnDespawnComplete()
	{
		Action onDespawned = this.OnDespawned;
		if (onDespawned == null)
		{
			return;
		}
		onDespawned();
	}

	private void Update()
	{
		if (this.currentState != TSMonsterState.DESPAWNING && Vector3.Distance(base.transform.position, this.spawnPos) > this.spawnDistanceThreshold)
		{
			this.Despawn();
		}
		if (this.currentState == TSMonsterState.PEEKING)
		{
			this.timeSpentPeeking += Time.deltaTime;
			if (this.timeSpentPeeking >= this.peekTimeUntilEmergeSec)
			{
				this.currentState = TSMonsterState.EMERGING;
				this.animator.SetTrigger("emerge");
				GameManager.Instance.VibrationManager.Vibrate(this.spawnVibration, VibrationRegion.WholeBody, true);
			}
		}
		if (this.currentState == TSMonsterState.SEARCHING || this.currentState == TSMonsterState.DRAINING)
		{
			this.TurnTowardsPlayer(this.eye.transform);
		}
		if (this.currentState == TSMonsterState.SEARCHING)
		{
			if (this.currentGameMode != GameMode.PASSIVE && this.eye.TrackedObject != null && (GameManager.Instance.Player.Controller.Velocity.magnitude > this.velocityMagnitudeThreshold || GameManager.Instance.Player.Controller.AngularVelocity.magnitude > this.angularVelocityMagnitudeThreshold))
			{
				float num = 1f;
				this.timeSpentDetectingPlayer += Time.deltaTime * num;
				this.timeUntilSearchExpires = this.searchTimeUntilGiveUpSec;
				if (this.timeSpentDetectingPlayer >= this.detectTimeUntilDrainSec && this.currentGameMode != GameMode.PASSIVE)
				{
					this.timeSpentLostPlayer = 0f;
					this.timeUntilRepath = 0f;
					this.currentState = TSMonsterState.DRAINING;
					this.animator.SetBool("detectsPlayer", true);
				}
			}
			else
			{
				this.timeUntilSearchExpires -= Time.deltaTime;
				this.timeSpentDetectingPlayer -= Time.deltaTime;
				this.timeSpentDetectingPlayer = Mathf.Max(this.timeSpentDetectingPlayer, 0f);
			}
			if (this.timeUntilSearchExpires <= 0f)
			{
				this.Despawn();
			}
		}
		if (this.currentState == TSMonsterState.DRAINING)
		{
			if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < this.navMeshAgent.stoppingDistance)
			{
				this.navMeshAgent.enabled = false;
				this.TurnTowardsPlayer(base.transform);
				GameManager.Instance.VibrationManager.Vibrate(this.sanityDrainVibration, VibrationRegion.WholeBody, false);
			}
			else
			{
				this.navMeshAgent.enabled = true;
			}
			this.timeSpentDraining += Time.deltaTime;
			if (this.timeSpentDraining >= this.minTimeSpentDraining && GameManager.Instance.Player.Sanity.CurrentSanity <= 0.05f)
			{
				GameManager.Instance.WorldEventManager.DoEvent(this.vineAttackData);
				this.Despawn();
				return;
			}
			bool flag = true;
			this.timeUntilRepath -= Time.deltaTime;
			if (this.timeUntilRepath <= 0f && this.navMeshAgent.enabled)
			{
				this.timeUntilRepath = this.timeBetweenRepaths;
				Vector3 vector = new Vector3(GameManager.Instance.Player.transform.position.x, 0f, GameManager.Instance.Player.transform.position.z);
				this.path = new NavMeshPath();
				this.navMeshAgent.CalculatePath(vector, this.path);
				this.navMeshAgent.SetPath(this.path);
				if (this.path.corners.Length != 0)
				{
					flag = Vector3.Distance(vector, this.path.corners[this.path.corners.Length - 1]) < this.arriveDistanceThreshold;
				}
			}
			if (flag && this.eye.TrackedObject != null)
			{
				this.timeSpentLostPlayer = 0f;
			}
			else
			{
				this.timeSpentLostPlayer += Time.deltaTime;
			}
			if (this.timeSpentLostPlayer >= this.lossTimeUntilLoseDetectionSec)
			{
				this.timeSpentDetectingPlayer = 0f;
				this.timeUntilSearchExpires = this.searchTimeUntilGiveUpSec;
				this.currentState = TSMonsterState.SEARCHING;
				this.animator.SetBool("detectsPlayer", false);
			}
		}
		this.currentDrainVolume = Mathf.Lerp(this.currentDrainVolume, (this.currentState == TSMonsterState.DRAINING) ? 1f : 0f, Time.deltaTime);
		this.currentScanVolume = Mathf.Lerp(this.currentScanVolume, (this.currentState == TSMonsterState.SEARCHING) ? 1f : 0f, Time.deltaTime);
		this.drainAudio.volume = this.currentDrainVolume;
		this.scanAudio.volume = this.currentScanVolume;
	}

	private void TurnTowardsPlayer(Transform makeThisObjectLookAtPlayer)
	{
		Vector3 vector = GameManager.Instance.Player.transform.position - makeThisObjectLookAtPlayer.position;
		float num = this.rotationSpeedRadPerSec * Time.deltaTime;
		Vector3 vector2 = Vector3.RotateTowards(makeThisObjectLookAtPlayer.forward, vector, num, 0f);
		Debug.DrawRay(makeThisObjectLookAtPlayer.position, vector2, Color.red);
		makeThisObjectLookAtPlayer.rotation = Quaternion.LookRotation(vector2);
	}

	private void OnPeekComplete()
	{
		TSMonsterAnimationEvents tsmonsterAnimationEvents = this.animationEvents;
		tsmonsterAnimationEvents.PeekCompleteAction = (Action)Delegate.Remove(tsmonsterAnimationEvents.PeekCompleteAction, new Action(this.OnPeekComplete));
		this.currentState = TSMonsterState.PEEKING;
	}

	private void OnEmergeComplete()
	{
		TSMonsterAnimationEvents tsmonsterAnimationEvents = this.animationEvents;
		tsmonsterAnimationEvents.EmergeCompleteAction = (Action)Delegate.Remove(tsmonsterAnimationEvents.EmergeCompleteAction, new Action(this.OnEmergeComplete));
		this.timeUntilSearchExpires = this.searchTimeUntilGiveUpSec;
		this.currentState = TSMonsterState.SEARCHING;
	}

	public void PlayEmergeSFX()
	{
		this.audioSource.PlayOneShot(this.emergeClips.PickRandom<AudioClip>());
		this.audioSource.PlayOneShot(this.splashClips.PickRandom<AudioClip>());
	}

	public void PlaySubmergeSFX()
	{
		this.audioSource.PlayOneShot(this.submergeClips.PickRandom<AudioClip>());
	}

	[SerializeField]
	private AudioSource scanAudio;

	[SerializeField]
	private AudioSource drainAudio;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> emergeClips;

	[SerializeField]
	private List<AudioClip> splashClips;

	[SerializeField]
	private List<AudioClip> submergeClips;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TSMonsterAnimationEvents animationEvents;

	[SerializeField]
	private FieldOfView eye;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[Header("Config")]
	[SerializeField]
	private float peekTimeUntilEmergeSec;

	[SerializeField]
	private float detectTimeUntilDrainSec;

	[SerializeField]
	private float searchTimeUntilGiveUpSec;

	[SerializeField]
	private float lossTimeUntilLoseDetectionSec;

	[SerializeField]
	private float velocityMagnitudeThreshold;

	[SerializeField]
	private float angularVelocityMagnitudeThreshold;

	[SerializeField]
	private float rotationSpeedRadPerSec;

	[SerializeField]
	private float timeBetweenRepaths;

	[SerializeField]
	private float spawnDistanceThreshold;

	[SerializeField]
	private float arriveDistanceThreshold;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private VibrationData spawnVibration;

	[SerializeField]
	private VibrationData sanityDrainVibration;

	[SerializeField]
	private float minTimeSpentDraining;

	[SerializeField]
	private WorldEventData vineAttackData;

	public Action OnDespawned;

	private TSMonsterState currentState;

	private float timeSpentDraining;

	private float timeSpentPeeking;

	private float timeSpentDetectingPlayer;

	private float timeSpentLostPlayer;

	private float timeUntilSearchExpires;

	private float timeUntilRepath;

	private Vector3 spawnPos;

	private NavMeshPath path;

	private float currentDrainVolume;

	private float currentScanVolume;

	private GameMode currentGameMode;
}
