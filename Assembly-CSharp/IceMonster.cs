using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceMonster : MonoBehaviour, IGameModeResponder
{
	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && this.currentState == IceMonster.IceMonsterState.CHASING)
		{
			this.Despawn();
		}
	}

	private void OnEnable()
	{
		this.currentState = IceMonster.IceMonsterState.SPAWNING;
		IceMonsterAnimationHelper iceMonsterAnimationHelper = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper.OnHuntCompleteAction = (Action)Delegate.Combine(iceMonsterAnimationHelper.OnHuntCompleteAction, new Action(this.OnHuntComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper2 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper2.OnJumpCompleteAction = (Action)Delegate.Combine(iceMonsterAnimationHelper2.OnJumpCompleteAction, new Action(this.OnJumpComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper3 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper3.OnFeedShouldRemoveFishAction = (Action)Delegate.Combine(iceMonsterAnimationHelper3.OnFeedShouldRemoveFishAction, new Action(this.OnFeedShouldRemoveFish));
		IceMonsterAnimationHelper iceMonsterAnimationHelper4 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper4.OnFeedingCompleteAction = (Action)Delegate.Combine(iceMonsterAnimationHelper4.OnFeedingCompleteAction, new Action(this.OnFeedingComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper5 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper5.OnDespawnCompleteAction = (Action)Delegate.Combine(iceMonsterAnimationHelper5.OnDespawnCompleteAction, new Action(this.OnDespawnComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper6 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper6.OnSpawnCallAction = (Action)Delegate.Combine(iceMonsterAnimationHelper6.OnSpawnCallAction, new Action(this.OnSpawnCall));
		IceMonsterAnimationHelper iceMonsterAnimationHelper7 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper7.OnSniffAction = (Action)Delegate.Combine(iceMonsterAnimationHelper7.OnSniffAction, new Action(this.OnSniff));
		GameEvents.Instance.OnDialogueStarted += this.OnDialogueStarted;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Combine(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Combine(targetFollow.OnPathError, new Action(this.OnPathError));
		this.grid = GameManager.Instance.SaveData.GetGridByKey(this.linkedGridKey);
		this.idleAudioSource.volume = 0f;
		this.spawnAudioSource.Play();
		this.navMeshAgent.speed = this.moveSpeedScalar * Mathf.Clamp(this.speedProportionOfPlayer * GameManager.Instance.PlayerStats.MovementSpeedModifier, this.boatSpeedMin, this.boatSpeedMax);
	}

	private void OnDisable()
	{
		IceMonsterAnimationHelper iceMonsterAnimationHelper = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper.OnHuntCompleteAction = (Action)Delegate.Remove(iceMonsterAnimationHelper.OnHuntCompleteAction, new Action(this.OnHuntComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper2 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper2.OnJumpCompleteAction = (Action)Delegate.Remove(iceMonsterAnimationHelper2.OnJumpCompleteAction, new Action(this.OnJumpComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper3 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper3.OnFeedShouldRemoveFishAction = (Action)Delegate.Remove(iceMonsterAnimationHelper3.OnFeedShouldRemoveFishAction, new Action(this.OnFeedShouldRemoveFish));
		IceMonsterAnimationHelper iceMonsterAnimationHelper4 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper4.OnFeedingCompleteAction = (Action)Delegate.Remove(iceMonsterAnimationHelper4.OnFeedingCompleteAction, new Action(this.OnFeedingComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper5 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper5.OnDespawnCompleteAction = (Action)Delegate.Remove(iceMonsterAnimationHelper5.OnDespawnCompleteAction, new Action(this.OnDespawnComplete));
		IceMonsterAnimationHelper iceMonsterAnimationHelper6 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper6.OnSpawnCallAction = (Action)Delegate.Remove(iceMonsterAnimationHelper6.OnSpawnCallAction, new Action(this.OnSpawnCall));
		IceMonsterAnimationHelper iceMonsterAnimationHelper7 = this.iceMonsterAnimationHelper;
		iceMonsterAnimationHelper7.OnSniffAction = (Action)Delegate.Remove(iceMonsterAnimationHelper7.OnSniffAction, new Action(this.OnSniff));
		GameEvents.Instance.OnDialogueStarted -= this.OnDialogueStarted;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Remove(targetFollow.OnPathError, new Action(this.OnPathError));
	}

	private void OnSpawnCall()
	{
		this.callAudioSource.clip = this.spawnCallAudioClip;
		this.callAudioSource.Play();
	}

	private void OnSniff()
	{
		this.sniffAudioSource.Play();
	}

	private void OnDialogueStarted()
	{
		if (this.currentState == IceMonster.IceMonsterState.CHASING)
		{
			this.Despawn();
		}
	}

	private void OnPathError()
	{
		this.Despawn();
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name && enabled && (this.currentState == IceMonster.IceMonsterState.SPAWNING || this.currentState == IceMonster.IceMonsterState.SENSING || this.currentState == IceMonster.IceMonsterState.CHASING || this.currentState == IceMonster.IceMonsterState.MOVING_TO_FOOD || this.currentState == IceMonster.IceMonsterState.TURNING_TO_FOOD))
		{
			GameEvents.Instance.TriggerThreatBanished(this.currentState == IceMonster.IceMonsterState.CHASING);
			this.monsterCollider.enabled = false;
			this.Despawn();
		}
	}

	public void Init(Transform spawnAnchor, float maxRangeFromSpawnAnchor, Transform feedingPosition, Collider feedingStationCollider)
	{
		this.spawnAnchor = spawnAnchor;
		this.maxRangeFromSpawnAnchor = maxRangeFromSpawnAnchor;
		this.feedingPosition = feedingPosition;
		this.feedingStationCollider = feedingStationCollider;
	}

	private void OnPlayerHit()
	{
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		GameManager.Instance.Player.PlayerCollisionAudio.PlayRandom();
		this.hitVFX.SetActive(true);
		base.StartCoroutine(this.DelayedDespawn());
	}

	private void OnHuntComplete()
	{
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
		bool flag = this.currentGameMode != GameMode.PASSIVE && !GameManager.Instance.DialogueRunner.IsDialogueRunning;
		if (this.grid.GetFilledCells(ItemSubtype.FISH) > 0 && !GameManager.Instance.DialogueRunner.IsDialogueRunning)
		{
			this.animator.SetTrigger("pursue");
			this.currentState = IceMonster.IceMonsterState.MOVING_TO_FOOD;
			this.navMeshAgent.SetDestination(this.feedingPosition.position);
			this.idleAudioSource.Play();
			this.idleAudioTargetVolume = 1f;
			return;
		}
		if (flag)
		{
			this.animator.SetTrigger("pursue");
			this.currentState = IceMonster.IceMonsterState.CHASING;
			this.targetFollow.enabled = true;
			this.targetFollow.Init(GameManager.Instance.Player.gameObject);
			this.idleAudioSource.Play();
			this.idleAudioTargetVolume = 1f;
			return;
		}
		this.Despawn();
	}

	private void OnJumpComplete()
	{
		this.isJumping = false;
	}

	private void StartFeeding()
	{
		this.currentState = IceMonster.IceMonsterState.FEEDING;
		this.animator.SetTrigger("feed");
		this.feedingStationCollider.enabled = false;
		this.feedAudioSource.Play();
		this.idleAudioTargetVolume = 0f;
	}

	private void OnFeedShouldRemoveFish()
	{
		this.cellsOfFishEaten = this.grid.GetFilledCells(ItemSubtype.FISH);
		this.grid.Clear(true);
		Action onContentsUpdated = this.grid.OnContentsUpdated;
		if (onContentsUpdated == null)
		{
			return;
		}
		onContentsUpdated();
	}

	private void OnFeedingComplete()
	{
		this.OnDespawnComplete();
		this.feedingStationCollider.enabled = true;
	}

	private void Jump()
	{
		this.jumpAudioSource.PlayOneShot(this.jumpAudioSource.clip);
		this.animator.SetTrigger("jump");
		this.isJumping = true;
	}

	private void Update()
	{
		this.idleAudioSource.volume = Mathf.Lerp(this.idleAudioSource.volume, this.idleAudioTargetVolume, Time.deltaTime * this.idleAudioVolumeChangeSpeed);
		if (this.currentState == IceMonster.IceMonsterState.CHASING || this.currentState == IceMonster.IceMonsterState.MOVING_TO_FOOD)
		{
			int num = this.iceAndPlayerLayerMask;
			if (this.currentState == IceMonster.IceMonsterState.MOVING_TO_FOOD)
			{
				num = this.iceLayerMask;
			}
			bool flag = Physics.Raycast(base.transform.position, base.transform.forward, this.iceDetectionRange, num);
			Debug.DrawLine(base.transform.position, base.transform.position + base.transform.forward * this.iceDetectionRange, flag ? Color.red : Color.green, Time.deltaTime);
			if (flag && !this.isJumping)
			{
				this.Jump();
			}
			this.timeUntilNextCall -= Time.deltaTime;
			if (this.timeUntilNextCall <= 0f)
			{
				this.callAudioSource.clip = this.callAudioClips.PickRandom<AudioClip>();
				this.callAudioSource.Play();
				this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
			}
		}
		if (this.currentState == IceMonster.IceMonsterState.MOVING_TO_FOOD && Vector3.Distance(base.transform.position, this.feedingPosition.position) < 0.5f)
		{
			this.currentState = IceMonster.IceMonsterState.TURNING_TO_FOOD;
			this.feedingRotation = Quaternion.LookRotation(this.feedingPosition.transform.forward);
			this.navMeshAgent.updateRotation = false;
		}
		if (this.currentState == IceMonster.IceMonsterState.TURNING_TO_FOOD)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.feedingRotation, this.turnToFoodSpeed * Time.fixedDeltaTime);
			if (Mathf.Abs(Vector3.Angle(base.transform.forward, this.feedingPosition.transform.forward)) < this.foodFacingAngleThreshold)
			{
				this.StartFeeding();
			}
		}
		if (this.currentState == IceMonster.IceMonsterState.CHASING)
		{
			this.timeSpentChasing += Time.deltaTime;
			if (Time.time > this.timeOfLastRangeCheck + this.timeBetweenRangeChecks)
			{
				this.timeOfLastRangeCheck = Time.time;
				if (Vector3.Distance(base.transform.position, this.spawnAnchor.position) > this.maxRangeFromSpawnAnchor)
				{
					this.Despawn();
					return;
				}
				if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > this.maxRangeFromPlayer)
				{
					this.Despawn();
				}
			}
		}
	}

	private IEnumerator DelayedDespawn()
	{
		yield return new WaitForSeconds(0.25f);
		this.monsterCollider.enabled = false;
		yield return new WaitForSeconds(0.25f);
		this.Despawn();
		yield break;
	}

	private void Despawn()
	{
		this.idleAudioTargetVolume = 0f;
		this.despawnAudioSource.Play();
		this.targetFollow.enabled = false;
		this.currentState = IceMonster.IceMonsterState.DESPAWNING;
		this.animator.SetTrigger("banish");
		this.navMeshAgent.speed = 0f;
	}

	private void OnDespawnComplete()
	{
		Action<IceMonster, bool> onDespawnedAction = this.OnDespawnedAction;
		if (onDespawnedAction != null)
		{
			onDespawnedAction(this, this.timeSpentChasing > this.timeSpentChasingToCountAsSuccessful);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private IceMonster.IceMonsterState currentState;

	[SerializeField]
	private TargetFollow targetFollow;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private IceMonsterAnimationHelper iceMonsterAnimationHelper;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private LayerMask iceLayerMask;

	[SerializeField]
	private LayerMask iceAndPlayerLayerMask;

	[SerializeField]
	private Collider monsterCollider;

	[SerializeField]
	private AbilityData banishAbilityData;

	[SerializeField]
	private VariablePlayerDamager variablePlayerDamager;

	[SerializeField]
	private GameObject hitVFX;

	[SerializeField]
	private float timeBetweenPlayerPathing;

	[SerializeField]
	private float timeBetweenRangeChecks;

	[SerializeField]
	private float maxRangeFromPlayer;

	[SerializeField]
	private float moveSpeedScalar;

	[SerializeField]
	private float boatSpeedMin;

	[SerializeField]
	private float boatSpeedMax;

	[SerializeField]
	private float speedProportionOfPlayer;

	[SerializeField]
	private GridKey linkedGridKey;

	[SerializeField]
	private float turnToFoodSpeed;

	[SerializeField]
	private float foodFacingAngleThreshold;

	[SerializeField]
	private AudioSource idleAudioSource;

	[SerializeField]
	private AudioSource jumpAudioSource;

	[SerializeField]
	private AudioSource spawnAudioSource;

	[SerializeField]
	private AudioSource despawnAudioSource;

	[SerializeField]
	private AudioSource feedAudioSource;

	[SerializeField]
	private AudioSource callAudioSource;

	[SerializeField]
	private AudioSource sniffAudioSource;

	[SerializeField]
	private AudioClip spawnCallAudioClip;

	[SerializeField]
	private List<AudioClip> callAudioClips = new List<AudioClip>();

	[SerializeField]
	private float timeBetweenCallsMin;

	[SerializeField]
	private float timeBetweenCallsMax;

	[SerializeField]
	private float idleAudioVolumeChangeSpeed;

	private float idleAudioTargetVolume;

	public int cellsOfFishEaten;

	public Action<IceMonster, bool> OnDespawnedAction;

	private Transform feedingPosition;

	private Quaternion feedingRotation;

	private Collider feedingStationCollider;

	private SerializableGrid grid;

	private Transform spawnAnchor;

	private float maxRangeFromSpawnAnchor;

	private float timeOfLastPlayerPath;

	private float timeOfLastRangeCheck;

	private bool isJumping;

	private float timeUntilNextCall;

	private GameMode currentGameMode;

	private float timeSpentChasing;

	private float timeSpentChasingToCountAsSuccessful = 3f;

	public bool didStartHunting;

	[SerializeField]
	private float iceDetectionRange;

	private enum IceMonsterState
	{
		SPAWNING,
		SENSING,
		CHASING,
		DESPAWNING,
		MOVING_TO_FOOD,
		TURNING_TO_FOOD,
		FEEDING
	}
}
