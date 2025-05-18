using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class AdditionalWaveMaskRenderer : MonoBehaviour
{
	private AdditionalWaveMask GetAdditionalWaveMaskParent()
	{
		if (base.GetComponentInParent<AdditionalWaveMask>() == null)
		{
			CustomDebug.EditorLogError("No AdditionalWaveMask component found. This object must be the child of a gameobject with an AdditionalWaveMask component");
			return null;
		}
		return base.GetComponentInParent<AdditionalWaveMask>();
	}

	public void SaveTexture()
	{
		AdditionalWaveMask additionalWaveMaskParent = this.GetAdditionalWaveMaskParent();
		if (additionalWaveMaskParent == null)
		{
			return;
		}
		base.transform.position = additionalWaveMaskParent.gameObject.transform.position;
		float lodBias = QualitySettings.lodBias;
		QualitySettings.lodBias = 100f;
		RenderTexture renderTexture = new RenderTexture(this.imageDimensions, this.imageDimensions, 32, DefaultFormat.HDR);
		this.renderCamera.targetTexture = renderTexture;
		this.renderCamera.orthographicSize = additionalWaveMaskParent.Bounds / 2f;
		this.renderCamera.Render();
		byte[] array = this.toTexture2D(renderTexture).EncodeToPNG();
		File.WriteAllBytes(this.filePath, array);
		QualitySettings.lodBias = lodBias;
	}

	private Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D texture2D = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
		RenderTexture.active = rTex;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)rTex.width, (float)rTex.height), 0, 0);
		Color[] pixels = texture2D.GetPixels(0);
		if (this.waveMask != null && this.onlyOverwriteGreenChannel)
		{
			Color[] pixels2 = this.waveMask.GetPixels(0);
			for (int i = 0; i < pixels2.Length; i++)
			{
				pixels2[i] = new Color(pixels2[i].r, pixels[i].g, pixels2[i].b, pixels2[i].a);
			}
			texture2D.SetPixels(pixels2, 0);
		}
		else
		{
			texture2D.SetPixels(pixels, 0);
		}
		texture2D.Apply();
		return texture2D;
	}

	[SerializeField]
	private Camera renderCamera;

	[SerializeField]
	private int imageDimensions = 1024;

	[SerializeField]
	private string filePath = "Assets/Textures/Environment/Masks/DLC1WaveMask.png";

	[SerializeField]
	private Texture2D waveMask;

	[SerializeField]
	private bool onlyOverwriteGreenChannel;
}
