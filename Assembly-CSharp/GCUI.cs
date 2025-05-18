using System;
using CommandTerminal;
using TMPro;
using UnityEngine;

public class GCUI : MonoBehaviour
{
	private void OnEnable()
	{
		this._gcText.gameObject.SetActive(this.onByDefault);
	}

	private void Start()
	{
		Terminal.Shell.AddCommand("gc", new Action<CommandArg[]>(this.ToggleGC), 1, 1, "Display garbage collector stats [0 = off | 1 = on]");
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("gc");
	}

	private void ToggleGC(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		this._gcText.gameObject.SetActive(flag);
	}

	private void Update()
	{
		if (this._gcText.isActiveAndEnabled && Time.unscaledTime > this._timer)
		{
			this._gcText.text = string.Format("{0} mb", GC.GetTotalMemory(false) / 1024000L);
			this._timer = Time.unscaledTime + this._hudRefreshRate;
		}
	}

	[SerializeField]
	private TextMeshProUGUI _gcText;

	[SerializeField]
	private float _hudRefreshRate = 1f;

	[SerializeField]
	private bool onByDefault;

	private float _timer;
}
