using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class OozePatch : MonoBehaviour
{
	public string OozePatchId
	{
		get
		{
			return this.oozePatchId;
		}
	}

	public float PatchSize
	{
		get
		{
			return this.patchSize;
		}
		set
		{
			this.patchSize = value;
			this.SetSize();
		}
	}

	public void InitialiseOoze(bool forceReinitialize)
	{
		if (this.isInitialised && !forceReinitialize)
		{
			return;
		}
		this.SetSize();
		if (!Application.isPlaying)
		{
			this.renderTextureSize = Mathf.Clamp(Mathf.NextPowerOfTwo(Mathf.RoundToInt(this.patchSize * this.renderTextureScale)), 0, 2048);
			this.renderTextureSwap = new RenderTexture(this.renderTextureSize, this.renderTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			this.oozeMeshRenderer.sharedMaterial.SetTexture("_RenderTexture", this.renderTextureSwap);
			this.oozeInitializeBlitMaterial.SetFloat("_StartingOozeFill", this.startingFillProportion);
			Graphics.Blit(this.baseShapeDistanceField, this.renderTextureSwap, this.oozeInitializeBlitMaterial);
			return;
		}
		this.localMaterialInstance = this.oozeMeshRenderer.material;
		this.renderTextureSize = Mathf.Clamp(Mathf.NextPowerOfTwo(Mathf.RoundToInt(this.patchSize * this.renderTextureScale)), 0, 2048);
		if (this.isInitialised && this.renderTexture.width != this.renderTextureSize)
		{
			this.ClearTextures();
			this.isInitialised = false;
		}
		if (!this.isInitialised)
		{
			this.renderTexture = new RenderTexture(this.renderTextureSize, this.renderTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			this.renderTexture.anisoLevel = 16;
			this.renderTexture.useMipMap = true;
			this.renderTexture.autoGenerateMips = true;
			this.renderTextureSwap = new RenderTexture(this.renderTextureSize, this.renderTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			this.renderTexture.anisoLevel = 16;
			this.renderTextureSwap.useMipMap = true;
			this.renderTextureSwap.autoGenerateMips = true;
			this.dataTex = new Texture2D(this.renderTextureSize, this.renderTextureSize, TextureFormat.RGBA32, true, true);
			this.dataTex.wrapMode = TextureWrapMode.Clamp;
		}
		this.localMaterialInstance.SetTexture("_RenderTexture", this.renderTexture);
		this.oozeInitializeBlitMaterial.SetFloat("_StartingOozeFill", this.startingFillProportion);
		Graphics.Blit(this.baseShapeDistanceField, this.renderTextureSwap, this.oozeInitializeBlitMaterial);
		Graphics.Blit(this.renderTextureSwap, this.renderTexture);
		this.oozeRemovedSinceLastCollection = 0f;
		this.totalOozeAmountLastCheck = 0f;
		this.baseOozeClearAmount = 0f;
		this.OozeDataTextureUpdate();
		if (this.hasFadedOut)
		{
			this.FadeIn(2f);
		}
		this.isInitialised = true;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!this.isInitialised || !this.isEnabled)
		{
			return;
		}
		this.OozeVisualUpdate(other.transform.position);
		if (Vector3.Distance(other.transform.position, this.lastPositionAtDataUpdate) > this.dataUpdateDistanceInterval)
		{
			this.OozeDataTextureUpdate();
			this.lastPositionAtDataUpdate = other.transform.position;
			this.SaveOozeProportion();
			if (this.baseOozeClearAmount != 0f && this.GetProportionFilled() < GameManager.Instance.GameConfigData.OozePatchProportionMinimum)
			{
				this.AutoCleanPatch();
			}
		}
	}

	private void AutoCleanPatch()
	{
		GameManager.Instance.SaveData.SetOozePatchFillAmount(this.oozePatchId, 0f);
		this.FadeOut(2f);
	}

	private void OnTriggerExit(Collider other)
	{
		this.SaveOozeProportion();
	}

	private void SaveOozeProportion()
	{
		float num = this.GetProportionFilled();
		if (num < GameManager.Instance.GameConfigData.OozePatchProportionMinimum)
		{
			num = 0f;
		}
		GameManager.Instance.SaveData.SetOozePatchFillAmount(this.oozePatchId, num);
	}

	private void OozeVisualUpdate(Vector3 oozeClearPosition)
	{
		Vector3 vector = base.transform.InverseTransformPoint(oozeClearPosition);
		Vector2 vector2 = new Vector2(vector.x, vector.z);
		this.oozeUpdateBlitMaterial.SetVector("_UVOffset", vector2);
		this.oozeUpdateBlitMaterial.SetFloat("_Size", this.patchSize);
		Graphics.Blit(this.renderTextureSwap, this.renderTexture, this.oozeUpdateBlitMaterial, 0);
		Graphics.Blit(this.renderTexture, this.renderTextureSwap);
	}

	private void OozeDataTextureUpdate()
	{
		if (!this.isRequestInProgress)
		{
			this.isRequestInProgress = true;
			AsyncGPUReadback.Request(this.renderTexture, 0, delegate(AsyncGPUReadbackRequest request)
			{
				if (request.hasError)
				{
					this.isRequestInProgress = false;
					return;
				}
				this.dataTex.SetPixelData<byte>(request.GetData<byte>(0), 0, 0);
				this.dataTex.Apply();
				if (this.baseOozeClearAmount == 0f)
				{
					float r = this.dataTex.GetPixel(0, 0, this.dataTex.mipmapCount - 1).r;
					this.baseOozeClearAmount = r;
				}
				this.isRequestInProgress = false;
			});
		}
	}

	private void ClearTextures()
	{
		if (this.renderTexture)
		{
			this.renderTexture.Release();
		}
		if (this.renderTextureSwap)
		{
			this.renderTextureSwap.Release();
		}
		this.dataTex = null;
		this.renderTexture = null;
		this.renderTextureSwap = null;
	}

	public float GetTotalAmountCleared()
	{
		float num = this.dataTex.GetPixel(0, 0, this.dataTex.mipmapCount - 1).r;
		num -= this.baseOozeClearAmount;
		float num2 = num - this.totalOozeAmountLastCheck;
		this.oozeRemovedSinceLastCollection += num2 * this.patchSize;
		this.totalOozeAmountLastCheck = num;
		return this.oozeRemovedSinceLastCollection;
	}

	public float GetProportionFilled()
	{
		float r = this.dataTex.GetPixel(0, 0, this.dataTex.mipmapCount - 1).r;
		float num = Mathf.InverseLerp(this.baseOozeClearAmount, 1f, r);
		float num2 = 1f - num;
		return Mathf.Lerp(0f, this.startingFillProportion, num2);
	}

	public void SetProportionFilled(float proportionFilled)
	{
		if (proportionFilled < GameManager.Instance.GameConfigData.OozePatchProportionMinimum)
		{
			proportionFilled = 0f;
		}
		this.startingFillProportion = proportionFilled;
	}

	public float CollectOoze()
	{
		float totalAmountCleared = this.GetTotalAmountCleared();
		this.oozeRemovedSinceLastCollection = 0f;
		return totalAmountCleared;
	}

	public float OozeAmountAtPosition(Vector3 worldPosition)
	{
		if (!this.PositionIsWithinBounds(worldPosition))
		{
			return 0f;
		}
		int num = this.renderTextureSize - 1;
		Vector3 vector = base.transform.InverseTransformPoint(worldPosition);
		int num2 = Mathf.RoundToInt((1f - (vector.x + 0.5f)) * (float)num);
		int num3 = Mathf.RoundToInt((1f - (vector.z + 0.5f)) * (float)num);
		Color pixel = this.dataTex.GetPixel(num2, num3);
		return 1f - pixel.r;
	}

	public bool PositionIsWithinBounds(Vector3 worldPosition)
	{
		return this.isEnabled && this.oozeCollider.bounds.Contains(worldPosition);
	}

	private void OnValidate()
	{
		this.SetSize();
	}

	private void OnDrawGizmos()
	{
		this.SetSize();
	}

	private void SetSize()
	{
		base.transform.localScale = new Vector3(this.patchSize, 1f, this.patchSize);
	}

	private void OnDestroy()
	{
		this.ClearTextures();
	}

	public void FadeOut(float durationInSeconds = 2f)
	{
		this.localMaterialInstance.DOFloat(0f, "_Opacity", durationInSeconds);
		this.hasFadedOut = true;
		this.isEnabled = false;
	}

	public void FadeIn(float durationInSeconds = 2f)
	{
		this.localMaterialInstance.SetFloat("_Opacity", 1f);
		this.localMaterialInstance.DOFloat(1f, "_Opacity", durationInSeconds);
		this.hasFadedOut = false;
		this.isEnabled = true;
	}

	[SerializeField]
	private string oozePatchId;

	[Header("Ooze shape settings:")]
	[SerializeField]
	private float patchSize = 80f;

	[SerializeField]
	private Texture2D baseShapeDistanceField;

	[Range(0f, 1f)]
	[SerializeField]
	private float startingFillProportion = 1f;

	[Header("Quality and performance settings:")]
	[Range(0.25f, 1f)]
	[SerializeField]
	private float renderTextureScale = 1f;

	[SerializeField]
	private float dataUpdateDistanceInterval = 2f;

	[Header("Materials and ooze mesh renderer:")]
	[SerializeField]
	private Material oozeUpdateBlitMaterial;

	[SerializeField]
	private Material oozeInitializeBlitMaterial;

	[SerializeField]
	private MeshRenderer oozeMeshRenderer;

	[SerializeField]
	private BoxCollider oozeCollider;

	private int renderTextureSize;

	private RenderTexture renderTexture;

	private RenderTexture renderTextureSwap;

	private Texture2D dataTex;

	private bool isRequestInProgress;

	private Vector3 lastPositionAtDataUpdate;

	private float oozeRemovedSinceLastCollection;

	private float totalOozeAmountLastCheck;

	private float baseOozeClearAmount;

	private bool isInitialised;

	private bool isEnabled = true;

	private Material localMaterialInstance;

	private bool hasFadedOut;
}
