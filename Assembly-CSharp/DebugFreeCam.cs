using System;
using Cinemachine;
using CommandTerminal;
using UnityEngine;

public class DebugFreeCam : MonoBehaviour
{
	private void OnValidate()
	{
		this.ocean = GameObject.Find("Ocean");
		this.followPlayer = GameObject.Find("FollowPlayer");
	}

	private void Awake()
	{
		this.rb.drag = 10f;
		this.rotSpeed = 30f;
		this.inputSensitivity = 1000f;
		this.AddTerminalCommands();
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("debugcam", new Action<CommandArg[]>(this.ToggleEnabled), 0, 1, "toggles debug cam on/off");
			Terminal.Shell.AddCommand("debugcam.fov", new Action<CommandArg[]>(this.SetFov), 1, 1, "changes debugcam fov");
			Terminal.Shell.AddCommand("debugcam.fog", new Action<CommandArg[]>(this.SetFog), 1, 1, "changes maximum amount of fog takes a 0 - 1");
			Terminal.Shell.AddCommand("debugcam.maxspeed", new Action<CommandArg[]>(this.SetMaxSpeed), 1, 1, "changes debugcam max speed");
			Terminal.Shell.AddCommand("debug.harvestsfx", new Action<CommandArg[]>(this.ToggleHarvestSFX), 1, 1, "toggles harvest spot SFX off/on 0 - 1");
			Terminal.Shell.AddCommand("debug.interactsfx", new Action<CommandArg[]>(this.ToggleInteractSFX), 1, 1, "toggles interaction SFX off/on 0 - 1");
			Terminal.Shell.AddCommand("debug.interacticons", new Action<CommandArg[]>(this.ToggleInteractIcons), 1, 1, "toggles interaction icons off/on 0 - 1");
		}
	}

	private void SetFov(CommandArg[] args)
	{
		this.targetFov = args[0].Float;
		this.vcam.m_Lens.FieldOfView = args[0].Float;
	}

	private void SetFog(CommandArg[] args)
	{
		Shader.SetGlobalFloat("_FogRemove", 1f - args[0].Float);
	}

	private void ToggleHarvestSFX(CommandArg[] args)
	{
		HarvestPOIHandler.playSFXClips = args[0].Float == 1f;
	}

	private void ToggleInteractSFX(CommandArg[] args)
	{
		IntermittentSFXPlayer.playSFXClips = args[0].Float == 1f;
	}

	private void ToggleInteractIcons(CommandArg[] args)
	{
		InteractPointUI.showInteractIcon = args[0].Float == 1f;
	}

	private void SetMaxSpeed(CommandArg[] args)
	{
		this.targetMaxSpeed = args[0].Float;
		this.maxSpeed = args[0].Float;
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("debugcam");
			Terminal.Shell.RemoveCommand("debugcam.fov");
			Terminal.Shell.RemoveCommand("debugcam.fog");
			Terminal.Shell.RemoveCommand("debugcam.maxspeed");
			Terminal.Shell.RemoveCommand("debug.harvestsfx");
			Terminal.Shell.RemoveCommand("debug.interactsfx");
			Terminal.Shell.RemoveCommand("debug.interacticons");
		}
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	private void ToggleEnabled(CommandArg[] args)
	{
		if (args.Length == 0)
		{
			this.EnableAfterDelay();
			return;
		}
		base.Invoke("EnableAfterDelay", args[0].Float);
	}

	private void EnableAfterDelay()
	{
		base.enabled = !base.enabled;
		this.vcam.gameObject.SetActive(base.enabled);
		this.followPlayer.GetComponent<FollowPlayerInWorld>().enabled = !base.enabled;
		if (base.enabled)
		{
			ApplicationEvents.Instance.TriggerUIDebugToggled(false);
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = 60;
			Time.fixedDeltaTime = 0.01666667f;
			return;
		}
		ApplicationEvents.Instance.TriggerUIDebugToggled(true);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 300;
		Time.fixedDeltaTime = 0.02f;
	}

	private void Update()
	{
		this.moveInput = Vector3.zero;
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		this.moveInput += axis * base.transform.right;
		this.moveInput += axis2 * base.transform.forward;
		if (Input.GetKey("w"))
		{
			this.moveInput += base.transform.forward;
		}
		if (Input.GetKey("s"))
		{
			this.moveInput -= base.transform.forward;
		}
		if (Input.GetKey("a"))
		{
			this.moveInput -= base.transform.right;
		}
		if (Input.GetKey("d"))
		{
			this.moveInput += base.transform.right;
		}
		if (Input.GetKey("q"))
		{
			this.moveInput -= base.transform.up;
		}
		if (Input.GetKey("e"))
		{
			this.moveInput += base.transform.up;
		}
		this.moveInput = this.moveInput.normalized;
		if (Input.mouseScrollDelta.y != 0f)
		{
			if (Input.GetKey("left ctrl"))
			{
				this.targetFov -= Input.mouseScrollDelta.y;
			}
			else
			{
				this.targetMaxSpeed = Mathf.Clamp(this.targetMaxSpeed + Input.mouseScrollDelta.y, 0f, 20f);
			}
		}
		this.vcam.m_Lens.FieldOfView = Mathf.Lerp(this.vcam.m_Lens.FieldOfView, this.targetFov, Time.deltaTime);
		if (Input.GetKeyDown("1"))
		{
			this.rb.drag = 0.25f;
			this.rotSpeed = 1f;
			this.inputSensitivity = 0.5f;
		}
		if (Input.GetKeyDown("2"))
		{
			this.rb.drag = 0.5f;
			this.rotSpeed = 1.15f;
			this.inputSensitivity = 0.5f;
		}
		if (Input.GetKeyDown("3"))
		{
			this.rb.drag = 0.75f;
			this.rotSpeed = 1.3f;
			this.inputSensitivity = 0.5f;
		}
		if (Input.GetKeyDown("4"))
		{
			this.rb.drag = 1f;
			this.rotSpeed = 1.5f;
			this.inputSensitivity = 0.75f;
		}
		if (Input.GetKeyDown("5"))
		{
			this.rb.drag = 1.5f;
			this.rotSpeed = 1.75f;
			this.inputSensitivity = 1f;
		}
		if (Input.GetKeyDown("6"))
		{
			this.rb.drag = 2f;
			this.rotSpeed = 2f;
			this.inputSensitivity = 2f;
		}
		if (Input.GetKeyDown("7"))
		{
			this.rb.drag = 3.5f;
			this.rotSpeed = 3f;
			this.inputSensitivity = 5f;
		}
		if (Input.GetKeyDown("8"))
		{
			this.rb.drag = 5f;
			this.rotSpeed = 5f;
			this.inputSensitivity = 20f;
		}
		if (Input.GetKeyDown("9"))
		{
			this.rb.drag = 10f;
			this.rotSpeed = 30f;
			this.inputSensitivity = 100f;
		}
		if (Input.GetKeyDown("f"))
		{
			this.currentFogMode++;
			if (this.currentFogMode == (DebugFreeCam.FogMode)Enum.GetValues(typeof(DebugFreeCam.FogMode)).Length)
			{
				this.currentFogMode = DebugFreeCam.FogMode.Camera;
			}
		}
		if (Input.GetKeyDown("n"))
		{
			this.playerOffset = base.transform.position - GameManager.Instance.Player.transform.position;
			this.pursuePlayer = !this.pursuePlayer;
		}
		float num = Mathf.Clamp(this.rotSpeed / 2f, 0f, 3f);
		this.torque.y = this.torque.y + Input.GetAxis("Mouse X") * num;
		this.torque.x = this.torque.x + -Input.GetAxis("Mouse Y") * num;
		this.torque.y = Mathf.Lerp(this.torque.y, 0f, Time.deltaTime * this.rotSpeed);
		this.torque.x = Mathf.Lerp(this.torque.x, 0f, Time.deltaTime * this.rotSpeed);
	}

	private void FixedUpdate()
	{
		float num = this.speed;
		this.maxSpeed = Mathf.Lerp(this.maxSpeed, this.targetMaxSpeed, Time.deltaTime);
		if (Input.GetKey("left shift"))
		{
			this.maxSpeed = 100f;
			num *= 5f;
		}
		if (Input.GetKey("space"))
		{
			this.smoothedInput = Vector3.zero;
			this.rb.AddForce(-this.rb.velocity, ForceMode.Force);
		}
		this.smoothedInput = Vector3.Lerp(this.smoothedInput * this.moveInput.magnitude, this.moveInput, Time.fixedDeltaTime * this.inputSensitivity);
		if (this.pursuePlayer)
		{
			this.smoothedInput = Vector3.zero;
			Vector3 vector = GameManager.Instance.Player.transform.position;
			vector.y = 0f;
			vector += this.playerOffset;
			this.rb.position = vector;
		}
		Vector3 vector2 = this.rb.velocity;
		this.rb.velocity = Vector3.zero;
		vector2 = Vector3.ClampMagnitude(vector2, this.maxSpeed);
		this.rb.velocity = vector2;
		if (this.targetMaxSpeed != 0f)
		{
			this.rb.AddForce(this.smoothedInput * (num * this.rb.drag), ForceMode.Force);
		}
		Vector3 vector3 = base.transform.eulerAngles;
		vector3.z = 0f;
		vector3 += this.torque * Time.fixedDeltaTime * this.rotSpeed;
		base.transform.eulerAngles = vector3;
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		this.followPlayer.transform.position = position;
		switch (this.currentFogMode)
		{
		case DebugFreeCam.FogMode.Camera:
			this.fogCenterPosition = base.transform.position;
			break;
		case DebugFreeCam.FogMode.Player:
			this.fogCenterPosition = GameManager.Instance.Player.transform.position;
			break;
		}
		Shader.SetGlobalVector("_FogCenter", new Vector4(this.fogCenterPosition.x, this.fogCenterPosition.y, this.fogCenterPosition.z, 0f));
		Transform transform = base.transform;
		float num = transform.position.x - this.playerPositionLast.x;
		float num2 = transform.position.z - this.playerPositionLast.z;
		if (Mathf.Sqrt(num * num + num2 * num2) > 128f)
		{
			this.followPosition.x = this.RoundTo(transform.position.x, 128f);
			this.followPosition.z = this.RoundTo(transform.position.z, 128f);
			this.ocean.transform.position = this.followPosition;
			this.playerPositionLast = transform.position;
		}
	}

	private float RoundTo(float value, float multipleOf)
	{
		return Mathf.Round(value / multipleOf) * multipleOf;
	}

	private Vector3 moveInput;

	private Vector3 torque;

	[SerializeField]
	private Rigidbody rb;

	private float speed = 40f;

	private float rotSpeed = 30f;

	[SerializeField]
	private GameObject ocean;

	[SerializeField]
	private GameObject followPlayer;

	[SerializeField]
	private CinemachineVirtualCamera vcam;

	private float inputSensitivity = 1000f;

	private float maxSpeed = 10f;

	private Vector3 followPosition = Vector3.zero;

	private Vector3 playerPositionLast = Vector3.zero;

	private Vector3 smoothedInput;

	private Vector3 fogCenterPosition;

	private float targetMaxSpeed = 10f;

	private float targetFov = 40f;

	private DebugFreeCam.FogMode currentFogMode;

	[SerializeField]
	private bool pursuePlayer;

	[SerializeField]
	private Vector3 playerOffset;

	private enum FogMode
	{
		Camera,
		Position,
		Player
	}
}
