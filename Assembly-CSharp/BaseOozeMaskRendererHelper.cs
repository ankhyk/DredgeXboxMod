using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class BaseOozeMaskRendererHelper : MonoBehaviour
{
	private OozePatch GetAdditionalWaveMaskParent()
	{
		if (base.GetComponentInParent<OozePatch>() == null)
		{
			CustomDebug.EditorLogError("No OozePatch component found. This object must be the child of a gameobject with an OozePatch component");
			return null;
		}
		return base.GetComponentInParent<OozePatch>();
	}

	public void SaveParentOozeTexture()
	{
		OozePatch additionalWaveMaskParent = this.GetAdditionalWaveMaskParent();
		if (additionalWaveMaskParent == null)
		{
			return;
		}
		float lodBias = QualitySettings.lodBias;
		base.transform.position = additionalWaveMaskParent.gameObject.transform.position;
		QualitySettings.lodBias = 100f;
		RenderTexture renderTexture = new RenderTexture(this.imageDimensions, this.imageDimensions, 32, DefaultFormat.HDR);
		this.renderCamera.targetTexture = renderTexture;
		this.renderCamera.orthographicSize = additionalWaveMaskParent.PatchSize / 2f;
		this.renderCamera.Render();
		byte[] array = this.toTexture2D(renderTexture).EncodeToPNG();
		File.WriteAllBytes(this.filePath + additionalWaveMaskParent.OozePatchId + ".png", array);
		QualitySettings.lodBias = lodBias;
	}

	public void SaveAllOozeTexturesInScene()
	{
		OozePatch[] array = global::UnityEngine.Object.FindObjectsOfType<OozePatch>();
		float lodBias = QualitySettings.lodBias;
		foreach (OozePatch oozePatch in array)
		{
			base.transform.position = oozePatch.gameObject.transform.position;
			base.transform.rotation = oozePatch.gameObject.transform.rotation;
			QualitySettings.lodBias = 100f;
			RenderTexture renderTexture = new RenderTexture(this.imageDimensions, this.imageDimensions, 32, DefaultFormat.HDR);
			this.renderCamera.targetTexture = renderTexture;
			this.renderCamera.orthographicSize = oozePatch.PatchSize / 2f;
			this.renderCamera.Render();
			byte[] array3 = this.toTexture2D(renderTexture).EncodeToPNG();
			File.WriteAllBytes(this.filePath + oozePatch.OozePatchId + ".png", array3);
		}
		QualitySettings.lodBias = lodBias;
	}

	private Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D texture2D = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
		RenderTexture.active = rTex;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)rTex.width, (float)rTex.height), 0, 0);
		Color[] pixels = texture2D.GetPixels(0);
		texture2D.SetPixels(pixels, 0);
		texture2D.Apply();
		return texture2D;
	}

	[SerializeField]
	private Camera renderCamera;

	[SerializeField]
	private int imageDimensions = 1024;

	[SerializeField]
	private string filePath = "Assets/Textures/Environment/Masks/DLC1WaveMask.png";
}
