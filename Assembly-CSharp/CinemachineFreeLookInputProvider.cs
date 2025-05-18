using System;
using Cinemachine;
using UnityEngine;

public class CinemachineFreeLookInputProvider : MonoBehaviour
{
	public bool Freelook
	{
		get
		{
			return this.freelook;
		}
		set
		{
			this.freelook = value;
		}
	}

	private void Awake()
	{
		this.brain = Camera.main.GetComponent<CinemachineBrain>();
	}

	private void Start()
	{
		CinemachineCore.GetInputAxis = new CinemachineCore.AxisInputDelegate(this.GetAxisCustom);
		this.canMoveCamera = true;
		this.cameraMoveAction = new DredgePlayerActionTwoAxis("Move Camera", GameManager.Instance.Input.Controls.CameraMove);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionTwoAxis[] { this.cameraMoveAction };
		input.AddActionListener(array, ActionLayer.BASE);
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (ability.name == this.spyglassAbility.name)
		{
			this.spyglassEnabled = enabled;
		}
	}

	public void OnGameWindowToggled()
	{
		this.canMoveCamera = GameManager.Instance.UI.ShowingWindowTypes.Count == 0;
	}

	public float GetAxisCustom(string axisName)
	{
		if (!GameManager.Instance.IsPaused && this.canMoveCamera && !this.playerCamera.IsRecentering && !GameManager.Instance.UI.IsShowingRadialMenu && (GameManager.Instance.Input.IsUsingController || this.freelook || this.spyglassEnabled || GameManager.Instance.Input.Controls.CameraMoveButton.IsPressed))
		{
			if (axisName == "Mouse X")
			{
				return this.cameraMoveAction.Value.x;
			}
			if (axisName == "Mouse Y")
			{
				return this.cameraMoveAction.Value.y;
			}
		}
		return 0f;
	}

	[SerializeField]
	private PlayerCamera playerCamera;

	private bool canMoveCamera;

	private DredgePlayerActionTwoAxis cameraMoveAction;

	private CinemachineBrain brain;

	[SerializeField]
	private bool freelook;

	[SerializeField]
	private AbilityData spyglassAbility;

	private bool spyglassEnabled;
}
