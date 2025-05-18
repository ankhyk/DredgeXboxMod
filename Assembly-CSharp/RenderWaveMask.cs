using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RenderWaveMask : MonoBehaviour
{
	public void SaveTexture()
	{
		float lodBias = QualitySettings.lodBias;
		QualitySettings.lodBias = 100f;
		RenderTexture renderTexture = new RenderTexture(this.imageDimensions, this.imageDimensions, 32, DefaultFormat.HDR);
		this.renderCamera.targetTexture = renderTexture;
		this.renderCamera.orthographicSize = this.gameConfigData.WorldSize * 0.5f;
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
		Color[] pixels = this.waveMask.GetPixels(0);
		Color[] pixels2 = texture2D.GetPixels(0);
		if (this.onlyOverwriteGreenChannel)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = new Color(pixels[i].r, pixels2[i].g, pixels[i].b, pixels[i].a);
			}
			texture2D.SetPixels(pixels, 0);
		}
		else
		{
			texture2D.SetPixels(pixels2, 0);
		}
		texture2D.Apply();
		return texture2D;
	}

	[SerializeField]
	private Camera renderCamera;

	[SerializeField]
	private GameConfigData gameConfigData;

	[SerializeField]
	private int imageDimensions = 1024;

	[SerializeField]
	private string filePath = "Assets/Textures/Environment/Masks/WaveMask.png";

	[SerializeField]
	private Texture2D waveMask;

	[SerializeField]
	private bool onlyOverwriteGreenChannel;
}
