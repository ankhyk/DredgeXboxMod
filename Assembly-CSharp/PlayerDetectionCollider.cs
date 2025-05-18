using System;
using UnityEngine;

public class PlayerDetectionCollider : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (ability.name == this.lightAbilityData.name)
		{
			this.isLightOn = enabled;
		}
		if (ability.name == this.foghornAbilityData.name)
		{
			this.isFoghornOn = enabled;
		}
	}

	private void FixedUpdate()
	{
		this.currentLightModifier = Mathf.Lerp(this.currentLightModifier, this.isLightOn ? this.addedRadiusForLight : 0f, Time.fixedDeltaTime * this.lerpSpeedLight);
		this.currentFoghornModifier = Mathf.Lerp(this.currentFoghornModifier, this.isFoghornOn ? this.addedRadiusForFoghorn : 0f, Time.fixedDeltaTime * this.lerpSpeedFoghorn);
		this.movementLerpProp = Mathf.InverseLerp(this.movementSpeedMin, this.movementSpeedMax, this.playerRef.Controller.Velocity.magnitude);
		this.movementModifierTarget = Mathf.Lerp(0f, this.addedRadiusForMovement, this.movementLerpProp);
		this.currentMovementModifier = Mathf.Lerp(this.currentMovementModifier, this.movementModifierTarget, Time.fixedDeltaTime * this.lerpSpeedMovement);
		this.sphereCollider.radius = this.baseRadius + this.currentLightModifier + this.currentFoghornModifier + this.currentMovementModifier;
	}

	[SerializeField]
	private float baseRadius;

	[SerializeField]
	private float addedRadiusForLight;

	[SerializeField]
	private float addedRadiusForFoghorn;

	[SerializeField]
	private float addedRadiusForMovement;

	[SerializeField]
	private float lerpSpeedLight;

	[SerializeField]
	private float lerpSpeedFoghorn;

	[SerializeField]
	private float lerpSpeedMovement;

	[SerializeField]
	private float movementSpeedMin;

	[SerializeField]
	private float movementSpeedMax;

	[SerializeField]
	private AbilityData lightAbilityData;

	[SerializeField]
	private AbilityData foghornAbilityData;

	[SerializeField]
	private Player playerRef;

	[SerializeField]
	private SphereCollider sphereCollider;

	private bool isLightOn;

	private bool isFoghornOn;

	private float currentLightModifier;

	private float currentFoghornModifier;

	private float currentMovementModifier;

	private float movementLerpProp;

	private float movementModifierTarget;
}
