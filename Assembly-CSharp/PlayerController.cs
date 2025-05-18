using System;
using CommandTerminal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float AbilitySpeedModifier { get; set; }

	public Player PlayerRef { get; set; }

	public bool IsMovementAllowed { get; set; }

	public float InputMagnitude { get; set; }

	public Vector3 Velocity
	{
		get
		{
			return this.rb.velocity;
		}
	}

	public bool IsAutoMoving
	{
		get
		{
			return this.autoMoveTarget != null;
		}
	}

	public Vector3 AngularVelocity
	{
		get
		{
			return this.rb.angularVelocity;
		}
	}

	public bool IsMoving
	{
		get
		{
			return this.didMoveThisFrame;
		}
	}

	public float AutoMoveSpeed
	{
		get
		{
			return this._autoMoveSpeed;
		}
	}

	public DredgePlayerActionTwoAxis MoveAction
	{
		get
		{
			return this.moveAction;
		}
	}

	public Transform AutoMoveTarget
	{
		get
		{
			return this.autoMoveTarget;
		}
	}

	private void Awake()
	{
		this.defaultDrag = this.rb.drag;
		this._baseMovementModifier = GameManager.Instance.GameConfigData.BaseMovementSpeedModifier;
		this._baseReverseModifier = GameManager.Instance.GameConfigData.BaseReverseSpeedModifier;
		this._baseTurnSpeed = GameManager.Instance.GameConfigData.BaseTurnSpeed;
		this.OnSettingChanged(SettingType.TURNING_DEADZONE_X);
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.TURNING_DEADZONE_X)
		{
			this.deadzoneX = GameManager.Instance.SettingsSaveData.turningDeadzoneX;
		}
	}

	private void Start()
	{
		this.AddTerminalCommands();
		this.AbilitySpeedModifier = 1f;
		this.moveAction = new DredgePlayerActionTwoAxis("Move", GameManager.Instance.Input.Controls.Move);
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.moveAction }, ActionLayer.BASE);
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerEnteredOoze += this.OnPlayerEnteredOoze;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
		GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerEnteredOoze -= this.OnPlayerEnteredOoze;
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	private void ToggleDrag(bool movementAllowed)
	{
		this.rb.drag = (movementAllowed ? this.defaultDrag : 3f);
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock)
		{
			this.IsMovementAllowed = false;
			this.rb.AddForce(-this.rb.velocity, ForceMode.VelocityChange);
			this.rb.constraints = (RigidbodyConstraints)10;
			return;
		}
		this.rb.constraints = RigidbodyConstraints.None;
		this.IsMovementAllowed = true;
	}

	public void OnGameWindowToggled()
	{
		if (!this.PlayerRef.IsDocked)
		{
			this.IsMovementAllowed = GameManager.Instance.UI.ShowingWindowTypes.Count == 0;
		}
		this.ToggleDrag(this.IsMovementAllowed);
	}

	public void ClearAutoMoveTarget()
	{
		this.autoMoveTarget = null;
	}

	public void ClearAutoRotateTarget()
	{
		this.hasAutoRotateTarget = false;
	}

	public void SetAutoMoveTarget(Transform target)
	{
		this.autoMoveTarget = target;
	}

	public void SetAutoRotateTarget(Vector3 target)
	{
		this.autoRotateTarget = target;
		this.hasAutoRotateTarget = true;
	}

	private void FixedUpdate()
	{
		this.didMoveThisFrame = false;
		if (GameManager.Instance.IsPlaying && this.PlayerRef.IsAlive && (this.autoMoveTarget != null || this.hasAutoRotateTarget))
		{
			if (this.autoMoveTarget != null)
			{
				this.rb.velocity = Vector3.zero;
				Vector3 vector = Vector3.MoveTowards(base.transform.position, this.autoMoveTarget.position, this._autoMoveSpeed * Time.fixedDeltaTime);
				base.transform.position = new Vector3(vector.x, base.transform.position.y, vector.z);
			}
			if (this.hasAutoRotateTarget)
			{
				Quaternion quaternion = Quaternion.LookRotation(this.autoRotateTarget);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this._lookSpeed * Time.fixedDeltaTime);
			}
			this.didMoveThisFrame = true;
		}
		else if (GameManager.Instance.IsPlaying && this.PlayerRef.IsAlive && this.IsMovementAllowed && this.autoMoveTarget == null)
		{
			Vector2 value = GameManager.Instance.Input.GetValue(this.moveAction);
			float num = ((Mathf.Abs(value.x) >= this.deadzoneX) ? value.x : 0f);
			float y = value.y;
			this.InputMagnitude = Mathf.Max(Mathf.Abs(num), Mathf.Abs(y));
			this.didMoveThisFrame = Mathf.Abs(num) > Mathf.Epsilon || Mathf.Abs(y) > Mathf.Epsilon;
			GameConfigData gameConfigData = GameManager.Instance.GameConfigData;
			PlayerStats playerStats = GameManager.Instance.PlayerStats;
			float num2 = playerStats.AttachedMonsterMovementSpeedFactor * playerStats.MovementSpeedModifier;
			float num3 = Mathf.Max(gameConfigData.BasePlayerSpeed, num2) * this._baseMovementModifier;
			float num4 = 0f;
			if (y > 0f)
			{
				num4 = y * num3 * this.AbilitySpeedModifier;
			}
			else if (y < 0f)
			{
				num4 = y * num3 * this._baseReverseModifier * Mathf.Min((float)playerStats.ReverseSpeedGadgetModifier, 1f / this._baseReverseModifier);
			}
			float num5 = num * this._baseTurnSpeed * (float)playerStats.TurningSpeedGadgetModifier;
			float num6 = num3 * 0.0001f;
			float num7 = Mathf.Clamp01(GameManager.Instance.WaveController.SampleWaveSteepnessAtPlayerPosition() * 10f);
			num7 = num7 * GameManager.Instance.WaveController.Steepness * num6;
			Vector3 vector2 = new Vector3(this.rb.velocity.x * num7, 0f, this.rb.velocity.z * num7);
			this.rb.AddForce(-vector2, ForceMode.VelocityChange);
			this.rb.AddForce(base.transform.forward * (num4 / 50f));
			this.rb.AddTorque(new Vector3(0f, num5 / 50f, 0f));
		}
		if (!this.PlayerRef.IsAlive)
		{
			this.rb.velocity = Vector3.zero;
			this.didMoveThisFrame = false;
		}
	}

	private void OnPlayerEnteredOoze()
	{
		this.rb.AddForce(-this.rb.velocity * GameManager.Instance.GameConfigData.MaxMovementPropInOoze, ForceMode.VelocityChange);
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("player.move", new Action<CommandArg[]>(this.SetPlayerBaseMoveSpeed), 1, 1, "Sets player base movement speed");
		Terminal.Shell.AddCommand("player.turn", new Action<CommandArg[]>(this.SetPlayerBaseTurnSpeed), 1, 1, "Sets player base turn speed");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("player.move");
		Terminal.Shell.RemoveCommand("player.turn");
	}

	private void SetPlayerBaseMoveSpeed(CommandArg[] args)
	{
		this._baseMovementModifier = args[0].Float;
	}

	private void SetPlayerBaseTurnSpeed(CommandArg[] args)
	{
		this._baseTurnSpeed = args[0].Float;
	}

	[SerializeField]
	private Rigidbody rb;

	private bool didMoveThisFrame;

	private Transform autoMoveTarget;

	private Vector3 autoRotateTarget;

	private bool hasAutoRotateTarget;

	private DredgePlayerActionBase openInventoryAction;

	private DredgePlayerActionTwoAxis moveAction;

	private float defaultDrag;

	private float deadzoneX;

	private float _baseMovementModifier;

	private float _baseReverseModifier;

	private float _baseTurnSpeed;

	private float _autoMoveSpeed = 1f;

	private float _lookSpeed = 1f;
}
