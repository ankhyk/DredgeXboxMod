using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FlickerLightsWorldEvent : WorldEvent
{
	public override void Activate()
	{
		this.lightAbility = GameManager.Instance.PlayerAbilities.GetAbilityForData(this.lightAbilityData) as LightAbility;
		if (this.lightAbility)
		{
			this.lightAbility.Activate();
			this.lightAbility.Locked = true;
			GameManager.Instance.Player.BoatModelProxy.LightFlickerEffect.BeginFlicker(this.flickerCurve, base.worldEventData.durationSec, this.enableAfterFinish);
			GameManager.Instance.AudioPlayer.PlaySFX(this.flickerSFX, AudioLayer.SFX_PLAYER, this.flickerVolume, 1f);
			base.Invoke("DelayedEventFinish", base.worldEventData.durationSec);
		}
	}

	private void DelayedEventFinish()
	{
		this.lightAbility.Locked = false;
		this.lightAbility.Deactivate();
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private AnimationCurve flickerCurve;

	[SerializeField]
	private bool enableAfterFinish;

	[SerializeField]
	private AbilityData lightAbilityData;

	[SerializeField]
	private AssetReference flickerSFX;

	[SerializeField]
	private float flickerVolume;

	private LightAbility lightAbility;
}
