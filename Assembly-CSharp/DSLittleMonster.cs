using System;
using System.Collections.Generic;
using UnityEngine;

public class DSLittleMonster : MonoBehaviour, IGameModeResponder
{
	public bool IsDespawning
	{
		get
		{
			return this.currentState == DSLittleMonster.DSMonsterState.DESPAWNING;
		}
	}

	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && this.currentState == DSLittleMonster.DSMonsterState.CHASING)
		{
			this.currentState = DSLittleMonster.DSMonsterState.RETURNING_HOME;
		}
	}

	private void Awake()
	{
		this.startPosition = base.transform.position;
		this.foamParticles.gameObject.SetActive(false);
		this.currentState = DSLittleMonster.DSMonsterState.IDLE;
	}

	public void Despawn(bool ignoreIfChasing, bool isBanish)
	{
		if (ignoreIfChasing && this.currentState == DSLittleMonster.DSMonsterState.CHASING)
		{
			return;
		}
		this.currentState = DSLittleMonster.DSMonsterState.DESPAWNING;
		this.foamParticles.Stop();
		this.piranhaAnimator.SetTrigger("despawn");
		if (isBanish && this.currentState == DSLittleMonster.DSMonsterState.CHASING)
		{
			this.audioSource.PlayOneShot(this.screamAudioClip);
		}
	}

	public void OnDespawnComplete()
	{
		Action<DSLittleMonster> onDespawn = this.OnDespawn;
		if (onDespawn != null)
		{
			onDespawn(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void EnableParticleEmission()
	{
		this.foamParticles.gameObject.SetActive(true);
	}

	private void FixedUpdate()
	{
		if (base.transform == null || this.homeAnchor == null)
		{
			this.OnDespawnComplete();
			return;
		}
		this.LookForPlayer();
		this.CheckHomeDistance();
		this.TurnToTarget();
		this.WiggleTurn();
		this.Move();
		this.UpdateAttachment();
	}

	private void Update()
	{
		if (this.suppressionTimer > 0f)
		{
			this.suppressionTimer -= Time.deltaTime;
		}
		if (this.isCountedAsAttached)
		{
			this.TryDoLatchedCall();
		}
	}

	private void CheckHomeDistance()
	{
		this.distanceFromHome = Vector3.Distance(base.transform.position, this.homeAnchor.position);
		if (this.currentState == DSLittleMonster.DSMonsterState.CHASING && this.distanceFromHome > this.maxRange)
		{
			this.currentState = DSLittleMonster.DSMonsterState.RETURNING_HOME;
			return;
		}
		if (this.currentState == DSLittleMonster.DSMonsterState.RETURNING_HOME && this.distanceFromHome < this.circleDistance * 2f)
		{
			this.currentState = DSLittleMonster.DSMonsterState.IDLE;
			return;
		}
		if (this.currentState == DSLittleMonster.DSMonsterState.IDLE && this.distanceFromHome > this.displacedDespawnDistance)
		{
			this.Despawn(false, false);
		}
	}

	private void GainPlayer()
	{
		this.currentState = DSLittleMonster.DSMonsterState.CHASING;
	}

	private void LosePlayer()
	{
		this.currentState = DSLittleMonster.DSMonsterState.IDLE;
	}

	private void LookForPlayer()
	{
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			return;
		}
		if (Time.time < this.timeOfLastLook + this.lookFrequencySec)
		{
			return;
		}
		if (this.suppressionTimer > 0f)
		{
			return;
		}
		if (this.currentState == DSLittleMonster.DSMonsterState.IDLE || (this.canLosePlayerBySight && this.currentState == DSLittleMonster.DSMonsterState.CHASING))
		{
			this.timeOfLastLook = Time.time;
			bool flag = false;
			if (Vector3.Distance(this.homeAnchor.transform.position, this.playerDetectTransform.position) < this.maxRange)
			{
				Vector3 normalized = (this.playerDetectTransform.position - base.transform.position).normalized;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, normalized, out raycastHit, this.viewDistance, this.layerMask))
				{
					flag = raycastHit.transform.gameObject.CompareTag("Player");
				}
			}
			if (this.currentState == DSLittleMonster.DSMonsterState.IDLE && flag)
			{
				this.GainPlayer();
				return;
			}
			if (this.currentState == DSLittleMonster.DSMonsterState.CHASING && !flag)
			{
				this.LosePlayer();
			}
		}
	}

	private void FaceObject(Transform objectToFace)
	{
		this.SmoothLookAt(objectToFace.position, this.turnSpeed);
	}

	private void CircleObject(Transform objectToFace)
	{
		if (objectToFace)
		{
			Vector3 position = objectToFace.position;
			position.x += Mathf.Sin(Time.time * this.circleSpeed) * this.circleDistance;
			position.z += Mathf.Cos(Time.time * this.circleSpeed) * this.circleDistance;
			this.SmoothLookAt(position, this.circleSpeed);
		}
	}

	private void TurnToTarget()
	{
		if (this.currentState == DSLittleMonster.DSMonsterState.CHASING)
		{
			this.FaceObject(this.playerAnchor);
			return;
		}
		if (this.currentState == DSLittleMonster.DSMonsterState.IDLE || this.currentState == DSLittleMonster.DSMonsterState.DESPAWNING || this.currentState == DSLittleMonster.DSMonsterState.RETURNING_HOME)
		{
			this.CircleObject(this.homeAnchor);
		}
	}

	private void WiggleTurn()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.y += Mathf.Sin(Time.time * this.wiggleSpeed) * this.wiggleAmount;
		base.transform.eulerAngles = eulerAngles;
	}

	private void Move()
	{
		Vector3 vector = base.transform.forward * this.speed;
		this.rb.AddForce(vector);
	}

	private void RotateToVelocity()
	{
		Vector3 vector = base.transform.position + this.rb.velocity.normalized;
		base.transform.LookAt(vector, Vector3.up);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Vent"))
		{
			if (this.currentState == DSLittleMonster.DSMonsterState.CHASING)
			{
				this.audioSource.PlayOneShot(this.screamAudioClip);
			}
			this.SendHome();
		}
	}

	public void DismissAndSuppress(float duration)
	{
		this.SendHome();
		this.suppressionTimer = duration;
	}

	public void SendHome()
	{
		this.currentState = DSLittleMonster.DSMonsterState.RETURNING_HOME;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!other.transform.CompareTag("SafeCollider"))
		{
			if (this.currentState == DSLittleMonster.DSMonsterState.CHASING)
			{
				this.currentState = DSLittleMonster.DSMonsterState.RETURNING_HOME;
				return;
			}
			if (this.currentState == DSLittleMonster.DSMonsterState.RETURNING_HOME)
			{
				this.Despawn(false, false);
			}
		}
	}

	private void SmoothLookAt(Vector3 target, float turnSpeed)
	{
		Quaternion rotation = base.transform.rotation;
		base.transform.LookAt(target);
		Quaternion rotation2 = base.transform.rotation;
		base.transform.rotation = rotation;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation2, turnSpeed * Time.fixedDeltaTime);
	}

	private void UpdateAttachment()
	{
		if (this.currentGameMode != GameMode.PASSIVE && Vector3.Distance(this.playerAnchor.transform.position, base.transform.position) < this.attachDistanceThreshold)
		{
			this.Attach();
			return;
		}
		this.Detach();
	}

	private void Attach()
	{
		if (!this.isCountedAsAttached)
		{
			GameManager.Instance.PlayerStats.AttachMonsterToPlayer();
			this.audioSource.PlayOneShot(this.detectAudioClip);
			this.isCountedAsAttached = true;
			this.timeUntilNextLatchedCall = global::UnityEngine.Random.Range(this.latchedCallDelayMin, this.latchedCallDelayMax);
			GameManager.Instance.VibrationManager.Vibrate(this.latchVibration, VibrationRegion.WholeBody, true);
		}
	}

	private void Detach()
	{
		if (this.isCountedAsAttached)
		{
			if (GameManager.Instance.PlayerStats.AttachedMonsterCount > 0)
			{
				GameManager.Instance.PlayerStats.RemoveMonsterFromPlayer();
			}
			this.isCountedAsAttached = false;
		}
	}

	private void TryDoLatchedCall()
	{
		this.timeUntilNextLatchedCall -= Time.deltaTime;
		if (this.timeUntilNextLatchedCall <= 0f)
		{
			this.audioSource.PlayOneShot(this.latchedCallAudioClips.PickRandom<AudioClip>());
			this.timeUntilNextLatchedCall = global::UnityEngine.Random.Range(this.latchedCallDelayMin, this.latchedCallDelayMax);
		}
	}

	private void OnDestroy()
	{
		this.Detach();
	}

	[SerializeField]
	private Animator piranhaAnimator;

	[SerializeField]
	private LayerMask layerMask;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private ParticleSystem foamParticles;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> latchedCallAudioClips;

	[SerializeField]
	private AudioClip detectAudioClip;

	[SerializeField]
	private AudioClip screamAudioClip;

	[SerializeField]
	private float latchedCallDelayMin;

	[SerializeField]
	private float latchedCallDelayMax;

	[SerializeField]
	private VibrationData latchVibration;

	private float timeUntilNextLatchedCall;

	private DSLittleMonster.DSMonsterState currentState;

	public Transform homeAnchor;

	public bool canLosePlayerBySight;

	public float attachDistanceThreshold;

	public float maxRange;

	public float speed;

	public float wiggleAmount;

	public float wiggleSpeed;

	public float viewDistance;

	public float circleSpeed;

	public float circleDistance;

	public float turnSpeed;

	public float lookFrequencySec;

	public float displacedDespawnDistance;

	public Action<DSLittleMonster> OnDespawn;

	public Player player;

	public Transform playerAnchor;

	public Transform playerDetectTransform;

	private Vector3 currentDirection;

	private float timeOfLastLook;

	private float distanceFromHome;

	private float suppressionTimer;

	private Vector3 startPosition;

	private bool isCountedAsAttached;

	private GameMode currentGameMode;

	private enum DSMonsterState
	{
		IDLE,
		CHASING,
		DESPAWNING,
		RETURNING_HOME
	}
}
