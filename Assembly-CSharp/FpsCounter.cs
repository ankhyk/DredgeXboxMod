using System;
using CommandTerminal;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private void Awake()
	{
		this.fpsQueue = new FixedSizedQueue<int>(this.frameHistoryLength);
		this.frameTimeQueue = new FixedSizedQueue<int>(this.frameHistoryLength);
	}

	private void OnEnable()
	{
		this._fpsText.gameObject.SetActive(this.onByDefault);
	}

	private void Start()
	{
		Terminal.Shell.AddCommand("fps", new Action<CommandArg[]>(this.ToggleFPS), 1, 1, "Display FPS Counter [0 = off | 1 = on]");
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("fps");
	}

	private void ToggleFPS(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		this._fpsText.gameObject.SetActive(flag);
	}

	private void Update()
	{
		if (this._fpsText.isActiveAndEnabled)
		{
			this.fps = (int)(1f / Time.unscaledDeltaTime);
			this.fpsQueue.Enqueue(this.fps);
			this.frameTime = Mathf.RoundToInt(Time.unscaledDeltaTime * 1000f);
			this.frameTimeQueue.Enqueue(this.frameTime);
			if (Time.unscaledTime > this._timer)
			{
				this.fpsArray = this.fpsQueue.ToArray();
				this.minFps = 999;
				this.maxFps = 0;
				this.totalFps = 0;
				for (int i = 0; i < this.fpsArray.Length; i++)
				{
					this.totalFps += this.fpsArray[i];
					if (this.fpsArray[i] < this.minFps)
					{
						this.minFps = this.fpsArray[i];
					}
					if (this.fpsArray[i] > this.maxFps)
					{
						this.maxFps = this.fpsArray[i];
					}
				}
				this.averageFps = Mathf.RoundToInt((float)(this.totalFps / this.fpsArray.Length));
				this.frameTimeArray = this.frameTimeQueue.ToArray();
				this.minFrameTime = 999;
				this.maxFrameTime = 0;
				this.totalFrameTime = 0;
				for (int j = 0; j < this.frameTimeArray.Length; j++)
				{
					this.totalFrameTime += this.frameTimeArray[j];
					if (this.frameTimeArray[j] < this.minFrameTime)
					{
						this.minFrameTime = this.frameTimeArray[j];
					}
					if (this.frameTimeArray[j] > this.maxFrameTime)
					{
						this.maxFrameTime = this.frameTimeArray[j];
					}
				}
				this.averageFrameTime = Mathf.RoundToInt((float)(this.totalFrameTime / this.frameTimeArray.Length));
				this._fpsText.text = string.Format("{0} fps | min: {1} - avg: {2} - max: {3}\n{4} ms | min: {5} - avg: {6} - max: {7}", new object[] { this.fps, this.minFps, this.averageFps, this.maxFps, this.frameTime, this.minFrameTime, this.averageFrameTime, this.maxFrameTime });
				this._timer = Time.unscaledTime + this._hudRefreshRate;
			}
		}
	}

	[SerializeField]
	private TextMeshProUGUI _fpsText;

	[SerializeField]
	private float _hudRefreshRate = 1f;

	[SerializeField]
	private bool onByDefault;

	[SerializeField]
	private int frameHistoryLength = 1000;

	private float _timer;

	private int fps;

	private int minFps;

	private int maxFps;

	private int frameTime;

	private int totalFps;

	private int averageFps;

	private FixedSizedQueue<int> fpsQueue;

	private int[] fpsArray;

	private int minFrameTime;

	private int maxFrameTime;

	private int totalFrameTime;

	private int averageFrameTime;

	private FixedSizedQueue<int> frameTimeQueue;

	private int[] frameTimeArray;
}
