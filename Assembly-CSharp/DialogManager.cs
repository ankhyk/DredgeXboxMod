using System;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.DialogManager = this;
	}

	public void ShowDialog(DialogOptions dialogOptions, Action<DialogButtonOptions> callback)
	{
		PopupDialog component = global::UnityEngine.Object.Instantiate<GameObject>(dialogOptions.useDeathScreenPopup ? this.deathPopupDialogPrefab : this.popupDialogPrefab, this.popupDialogContainer).GetComponent<PopupDialog>();
		if (component)
		{
			if (dialogOptions.disableGameCanvas)
			{
				GameManager.Instance.UI.DisableGameCanvas();
			}
			component.Show(dialogOptions, callback);
		}
	}

	public async Task ShowPopupControllerDropoutDialog(ControllerDropoutParams dropoutParms)
	{
		if (dropoutParms.delayBeforeTrigger > 0)
		{
			await Awaiters.SecondsRealtime((float)dropoutParms.delayBeforeTrigger);
		}
		while (this.popupControllerDropoutPrefab == null)
		{
			await Awaiters.NextFrame;
		}
		bool waitedBetweenScenes = SceneLoader.BetweenScenes;
		while (SceneLoader.BetweenScenes)
		{
			await Awaiters.NextFrame;
		}
		if (!PopupControllerDropout.HasDevice(dropoutParms.forceGamepad))
		{
			if (waitedBetweenScenes)
			{
				await Awaiters.SecondsRealtime(1f);
			}
			if (!PopupControllerDropout.Open)
			{
				PopupControllerDropout.Open = true;
				GameObject go = global::UnityEngine.Object.Instantiate<GameObject>(this.popupControllerDropoutPrefab, this.popupControllerDropoutContainer);
				await Awaiters.NextFrame;
				PopupControllerDropout component = go.GetComponent<PopupControllerDropout>();
				if (component)
				{
					await component.DisplayAsync(dropoutParms, waitedBetweenScenes);
				}
			}
		}
	}

	[SerializeField]
	private GameObject popupDialogPrefab;

	[SerializeField]
	private GameObject deathPopupDialogPrefab;

	[SerializeField]
	private Transform popupDialogContainer;

	[SerializeField]
	private GameObject popupControllerDropoutPrefab;

	[SerializeField]
	private Transform popupControllerDropoutContainer;
}
