using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("Effects/Planar Reflections")]
public class PlanarReflections : MonoBehaviour
{
	public static event Action<ScriptableRenderContext, Camera> BeginPlanarReflections;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.DoPlanarReflections;
	}

	private void OnDisable()
	{
		this.CleanUp();
		RenderPipelineManager.beginCameraRendering -= this.DoPlanarReflections;
	}

	private void OnDestroy()
	{
		this.CleanUp();
		RenderPipelineManager.beginCameraRendering -= this.DoPlanarReflections;
	}

	private void CleanUp()
	{
		if (PlanarReflections._reflectionCamera)
		{
			PlanarReflections._reflectionCamera.targetTexture = null;
			this.SafeDestroyObject(PlanarReflections._reflectionCamera.gameObject);
		}
		if (PlanarReflections._reflectionTexture)
		{
			RenderTexture.ReleaseTemporary(PlanarReflections._reflectionTexture);
		}
	}

	private void SafeDestroyObject(global::UnityEngine.Object obj)
	{
		if (Application.isEditor)
		{
			global::UnityEngine.Object.DestroyImmediate(obj);
			return;
		}
		global::UnityEngine.Object.Destroy(obj);
	}

	private void UpdateReflectionCamera(Camera realCamera)
	{
		if (PlanarReflections._reflectionCamera == null)
		{
			PlanarReflections._reflectionCamera = this.InitializeReflectionCamera();
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.up;
		if (this.reflectionTarget != null)
		{
			vector = this.reflectionTarget.transform.position + Vector3.up * this.reflectionPlaneOffset;
			vector2 = this.reflectionTarget.transform.up;
		}
		this.UpdateCamera(realCamera, PlanarReflections._reflectionCamera);
		PlanarReflections._reflectionCamera.gameObject.hideFlags = (this.hideReflectionCamera ? HideFlags.HideAndDontSave : HideFlags.DontSave);
		float num = -Vector3.Dot(vector2, vector);
		Vector4 vector3 = new Vector4(vector2.x, vector2.y, vector2.z, num);
		Matrix4x4 matrix4x = Matrix4x4.identity;
		matrix4x *= Matrix4x4.Scale(new Vector3(1f, -1f, 1f));
		PlanarReflections.CalculateReflectionMatrix(ref matrix4x, vector3);
		Vector3 vector4 = PlanarReflections.ReflectPosition(realCamera.transform.position - new Vector3(0f, vector.y * 2f, 0f));
		PlanarReflections._reflectionCamera.transform.forward = Vector3.Scale(realCamera.transform.forward, new Vector3(1f, -1f, 1f));
		PlanarReflections._reflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * matrix4x;
		Vector4 vector5 = this.CameraSpacePlane(PlanarReflections._reflectionCamera, vector - Vector3.up * 0.1f, vector2, 1f);
		Matrix4x4 matrix4x2 = realCamera.CalculateObliqueMatrix(vector5);
		PlanarReflections._reflectionCamera.projectionMatrix = matrix4x2;
		PlanarReflections._reflectionCamera.cullingMask = this.reflectionLayer;
		PlanarReflections._reflectionCamera.transform.position = vector4;
	}

	private void UpdateCamera(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.CopyFrom(src);
		dest.useOcclusionCulling = false;
		UniversalAdditionalCameraData universalAdditionalCameraData;
		if (dest.gameObject.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
		{
			universalAdditionalCameraData.renderShadows = false;
			if (this.reflectSkybox)
			{
				dest.clearFlags = CameraClearFlags.Skybox;
				return;
			}
			dest.clearFlags = CameraClearFlags.Color;
			dest.backgroundColor = Color.black;
		}
	}

	private Camera InitializeReflectionCamera()
	{
		GameObject gameObject = new GameObject("", new Type[] { typeof(Camera) });
		gameObject.name = "Reflection Camera [" + gameObject.GetInstanceID().ToString() + "]";
		UniversalAdditionalCameraData universalAdditionalCameraData = gameObject.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
		universalAdditionalCameraData.requiresColorOption = CameraOverrideOption.Off;
		universalAdditionalCameraData.requiresDepthOption = CameraOverrideOption.Off;
		universalAdditionalCameraData.SetRenderer(0);
		Transform transform = base.transform;
		Camera component = gameObject.GetComponent<Camera>();
		component.transform.SetPositionAndRotation(transform.position, transform.rotation);
		component.depth = -10f;
		component.enabled = false;
		return component;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 vector = worldToCameraMatrix.MultiplyPoint(pos);
		Vector3 vector2 = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector2.x, vector2.y, vector2.z, -Vector3.Dot(vector, vector2));
	}

	private RenderTextureDescriptor GetDescriptor(Camera camera, float pipelineRenderScale)
	{
		int num = (int)Mathf.Max(new float[] { (float)camera.pixelWidth * pipelineRenderScale * this.renderScale });
		int num2 = (int)Mathf.Max(new float[] { (float)camera.pixelHeight * pipelineRenderScale * this.renderScale });
		RenderTextureFormat renderTextureFormat = (camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
		return new RenderTextureDescriptor(num, num2, renderTextureFormat, 16)
		{
			autoGenerateMips = true,
			useMipMap = true
		};
	}

	private void CreateReflectionTexture(Camera camera)
	{
		RenderTextureDescriptor descriptor = this.GetDescriptor(camera, UniversalRenderPipeline.asset.renderScale);
		if (PlanarReflections._reflectionTexture == null)
		{
			PlanarReflections._reflectionTexture = RenderTexture.GetTemporary(descriptor);
			this.previousDescriptor = descriptor;
		}
		else if (!descriptor.Equals(this.previousDescriptor))
		{
			if (PlanarReflections._reflectionTexture)
			{
				RenderTexture.ReleaseTemporary(PlanarReflections._reflectionTexture);
			}
			PlanarReflections._reflectionTexture = RenderTexture.GetTemporary(descriptor);
			this.previousDescriptor = descriptor;
		}
		PlanarReflections._reflectionCamera.targetTexture = PlanarReflections._reflectionTexture;
	}

	private void DoPlanarReflections(ScriptableRenderContext context, Camera camera)
	{
		if (camera.cameraType == CameraType.Reflection || camera.cameraType == CameraType.Preview)
		{
			return;
		}
		if (!this.reflectionTarget)
		{
			return;
		}
		this.UpdateReflectionCamera(camera);
		this.CreateReflectionTexture(camera);
		PlanarReflections.PlanarReflectionSettingData planarReflectionSettingData = new PlanarReflections.PlanarReflectionSettingData();
		planarReflectionSettingData.Set();
		Action<ScriptableRenderContext, Camera> beginPlanarReflections = PlanarReflections.BeginPlanarReflections;
		if (beginPlanarReflections != null)
		{
			beginPlanarReflections(context, PlanarReflections._reflectionCamera);
		}
		if (PlanarReflections._reflectionCamera.WorldToViewportPoint(this.reflectionTarget.transform.position).z < 100000f)
		{
			UniversalRenderPipeline.RenderSingleCamera(context, PlanarReflections._reflectionCamera);
		}
		planarReflectionSettingData.Restore();
		Shader.SetGlobalTexture(this._planarReflectionTextureId, PlanarReflections._reflectionTexture);
	}

	public static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
	{
		reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		reflectionMatrix.m30 = 0f;
		reflectionMatrix.m31 = 0f;
		reflectionMatrix.m32 = 0f;
		reflectionMatrix.m33 = 1f;
	}

	public static Vector3 ReflectPosition(Vector3 pos)
	{
		return new Vector3(pos.x, -pos.y, pos.z);
	}

	public static bool Compare(Vector2 a, Vector2 b)
	{
		return a.x == b.x && a.y == b.y;
	}

	[Range(0.01f, 1f)]
	public float renderScale = 1f;

	public LayerMask reflectionLayer = -1;

	[FormerlySerializedAs("reflectionGameCore")]
	public LayerMask reflectionLayerGameCore = -1;

	public LayerMask basicReflectionLayer = -1;

	public bool reflectSkybox;

	public GameObject reflectionTarget;

	[Range(-2f, 3f)]
	public float reflectionPlaneOffset;

	private static Camera _reflectionCamera;

	private UniversalAdditionalCameraData cameraData;

	private static RenderTexture _reflectionTexture;

	private RenderTextureDescriptor previousDescriptor;

	private readonly int _planarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");

	public bool hideReflectionCamera;

	private class PlanarReflectionSettingData
	{
		public PlanarReflectionSettingData()
		{
			this.fog = RenderSettings.fog;
			this.maximumLODLevel = QualitySettings.maximumLODLevel;
			this.lodBias = QualitySettings.lodBias;
		}

		public void Set()
		{
			GL.invertCulling = true;
			RenderSettings.fog = false;
			QualitySettings.maximumLODLevel = 1;
			QualitySettings.lodBias = this.lodBias * 0.5f;
		}

		public void Restore()
		{
			GL.invertCulling = false;
			RenderSettings.fog = this.fog;
			QualitySettings.maximumLODLevel = this.maximumLODLevel;
			QualitySettings.lodBias = this.lodBias;
		}

		private readonly bool fog;

		private readonly int maximumLODLevel;

		private readonly float lodBias;
	}
}
