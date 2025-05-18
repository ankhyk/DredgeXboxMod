using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class HarvestPOIHandler : SerializedMonoBehaviour
{
	public bool IsHandlerActive { get; set; }

	public HarvestPOI HarvestPOI
	{
		get
		{
			return this.harvestPOI;
		}
		set
		{
			this.harvestPOI = value;
		}
	}

	private void Awake()
	{
		this.initiateHarvestModeAction = new DredgePlayerActionPress("prompt.anchor", GameManager.Instance.Input.Controls.Interact);
		DredgePlayerActionPress dredgePlayerActionPress = this.initiateHarvestModeAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
		this.initiateHarvestModeAction.showInControlArea = true;
		this.initiateHarvestModeAction.allowPreholding = true;
	}

	private void OnDestroy()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.initiateHarvestModeAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
	}

	public void Activate(HarvestPOI harvestPOI)
	{
		this.IsHandlerActive = true;
		this.harvestPOI = harvestPOI;
		this.harvester.CurrentHarvestPOI = harvestPOI;
		if (harvestPOI.IsCrabPotPOI)
		{
			this.initiateHarvestModeAction.SetPromptString("prompt.pot");
		}
		else if (harvestPOI.IsDredgePOI)
		{
			this.initiateHarvestModeAction.SetPromptString("prompt.dredge");
		}
		else
		{
			this.initiateHarvestModeAction.SetPromptString("prompt.fish");
		}
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.initiateHarvestModeAction };
		input.AddActionListener(array, ActionLayer.BASE);
		if (this.playRandomHarvestProximitySFXCoroutine != null)
		{
			base.StopCoroutine(this.playRandomHarvestProximitySFXCoroutine);
			this.playRandomHarvestProximitySFXCoroutine = null;
		}
		HarvestableItemData firstHarvestableItem = harvestPOI.Harvestable.GetFirstHarvestableItem();
		if (firstHarvestableItem)
		{
			this.playRandomHarvestProximitySFXCoroutine = base.StartCoroutine(this.PlayRandomHarvestProximityClip(harvestPOI, firstHarvestableItem));
		}
	}

	private IEnumerator PlayRandomHarvestProximityClip(HarvestPOI harvestPOI, HarvestableItemData harvestableItemData)
	{
		if (!HarvestPOIHandler.playSFXClips)
		{
			yield break;
		}
		if (this.harvester.CurrentHarvestPOI.IsHarvestable == HarvestQueryEnum.VALID && harvestableItemData.harvestPOICategory != HarvestPOICategory.NONE)
		{
			float num = global::UnityEngine.Random.Range(1f, 2.5f);
			AudioClip audioClip = this.sfxClips[harvestableItemData.harvestPOICategory].PickRandom<AudioClip>();
			GameManager.Instance.AudioPlayer.PlaySFX(audioClip, harvestPOI.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, 2f, 8f, true, false);
			yield return new WaitForSeconds(audioClip.length + num);
			this.playRandomHarvestProximitySFXCoroutine = base.StartCoroutine(this.PlayRandomHarvestProximityClip(harvestPOI, harvestableItemData));
		}
		yield return null;
		yield break;
	}

	public void Deactivate()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.initiateHarvestModeAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		if (this.playRandomHarvestProximitySFXCoroutine != null)
		{
			base.StopCoroutine(this.playRandomHarvestProximitySFXCoroutine);
			this.playRandomHarvestProximitySFXCoroutine = null;
		}
		this.harvestPOI = null;
		this.IsHandlerActive = false;
	}

	private void OnPressComplete()
	{
		GameEvents.Instance.TriggerPlayerInteractedWithPOI();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.initiateHarvestModeAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		this.harvester.enabled = true;
	}

	public static bool playSFXClips = true;

	[SerializeField]
	private Dictionary<HarvestPOICategory, List<AudioClip>> sfxClips = new Dictionary<HarvestPOICategory, List<AudioClip>>();

	[SerializeField]
	private Harvester harvester;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	private HarvestPOI harvestPOI;

	private DredgePlayerActionPress initiateHarvestModeAction;

	private Coroutine playRandomHarvestProximitySFXCoroutine;
}
