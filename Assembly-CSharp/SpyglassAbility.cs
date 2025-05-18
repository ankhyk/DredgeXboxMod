using System;
using Cinemachine;
using UnityEngine;

public class SpyglassAbility : Ability
{
	private void OnEnable()
	{
		this.spyglassPOV = this.spyglassVCam.GetCinemachineComponent<CinemachinePOV>();
		this.transitionAngleOffset = -GameManager.Instance.Player.transform.parent.transform.localRotation.eulerAngles.y;
	}

	public override void Init()
	{
		base.Init();
		this.spyglassVCam.enabled = false;
	}

	public override bool Activate()
	{
		bool flag = false;
		if (this.isActive)
		{
			this.Deactivate();
		}
		else
		{
			this.isActive = true;
			this.spyglassPOV.m_VerticalAxis.Value = 0.5f;
			if (this.playerVCam.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace)
			{
				float y = GameManager.Instance.Player.transform.localRotation.eulerAngles.y;
				float num = this.playerVCam.m_XAxis.Value + (this.transitionAngleOffset - y);
				this.spyglassPOV.m_HorizontalAxis.Value = num;
			}
			else
			{
				this.spyglassPOV.m_HorizontalAxis.Value = this.playerVCam.m_XAxis.Value;
			}
			this.spyglassVCam.enabled = true;
			flag = base.Activate();
			if (!GameManager.Instance.Input.IsUsingController)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SPYGLASS);
		}
		return flag;
	}

	public override void Deactivate()
	{
		if (this.isActive)
		{
			this.spyglassVCam.enabled = false;
			if (this.playerVCam.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace)
			{
				this.playerVCam.m_XAxis.Value = this.spyglassPOV.m_HorizontalAxis.Value - (this.transitionAngleOffset - GameManager.Instance.Player.transform.localRotation.eulerAngles.y);
			}
			else
			{
				this.playerVCam.m_XAxis.Value = this.spyglassPOV.m_HorizontalAxis.Value;
			}
			GameManager.Instance.Input.RefreshCursorLockState();
			GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
		}
		base.Deactivate();
	}

	private void Update()
	{
		if (!base.IsActive)
		{
			return;
		}
		if (!this.cameraRef)
		{
			this.cameraRef = Camera.main;
			if (!this.cameraRef)
			{
				return;
			}
		}
		RaycastHit[] array = Physics.RaycastAll(this.spyglassVCam.transform.position, this.cameraRef.transform.forward, this.maxRange, this.spyglassLayerMask);
		HarvestPOI harvestPOI = null;
		float num = float.PositiveInfinity;
		foreach (RaycastHit raycastHit in array)
		{
			HarvestPOI component = raycastHit.transform.GetComponent<HarvestPOI>();
			if (component)
			{
				float num2 = Mathf.Abs(Vector3.Angle(component.transform.position - this.spyglassVCam.transform.position, this.cameraRef.transform.forward));
				if (num2 < num && num2 < this.maxAngle)
				{
					float num3 = Vector3.Distance(component.transform.position, this.spyglassVCam.transform.position);
					if (Physics.RaycastAll(this.spyglassVCam.transform.position, component.transform.position - this.spyglassVCam.transform.position, num3, this.obscuredLayerMask, QueryTriggerInteraction.Ignore).Length == 0)
					{
						num = num2;
						harvestPOI = component;
					}
				}
			}
		}
		if (harvestPOI != this.previousPOI)
		{
			GameManager.Instance.UI.SpyglassUI.SetFocusedHarvestPOI(harvestPOI);
			this.previousPOI = harvestPOI;
		}
	}

	[SerializeField]
	private CinemachineVirtualCamera spyglassVCam;

	[SerializeField]
	private CinemachineFreeLook playerVCam;

	[SerializeField]
	private LayerMask spyglassLayerMask;

	[SerializeField]
	private LayerMask obscuredLayerMask;

	[SerializeField]
	private float maxRange;

	[SerializeField]
	private float maxAngle;

	[SerializeField]
	private float transitionAngleOffset;

	private Camera cameraRef;

	private CinemachinePOV spyglassPOV;

	private float prevSpyglassX;

	private HarvestPOI previousPOI;
}
