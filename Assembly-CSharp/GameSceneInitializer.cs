using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[DefaultExecutionOrder(-1000)]
public class GameSceneInitializer : MonoBehaviour
{
	public GameObject HarvestPoiContainer
	{
		get
		{
			return this.harvestPoiContainer;
		}
		set
		{
			this.harvestPoiContainer = value;
		}
	}

	private void Awake()
	{
		GameSceneInitializer.Instance = this;
	}

	private void Start()
	{
		base.StartCoroutine(this.LoadAll());
	}

	private IEnumerator LoadAll()
	{
		yield return base.StartCoroutine(this.LoadPlayer());
		yield return base.StartCoroutine(this.LoadHarvestPOIs());
		yield return base.StartCoroutine(this.LoadTeleportAnchor());
		yield break;
	}

	public bool IsDone()
	{
		return this.IsPlayerLoaded && this.ArePOIsLoaded && this.IsTeleportAnchorLoaded;
	}

	private IEnumerator LoadPlayer()
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(this.playerPrefabReference);
		yield return handle;
		if (handle.Result != null)
		{
			List<DockPOI> list = global::UnityEngine.Object.FindObjectsOfType<DockPOI>().ToList<DockPOI>();
			string dockId = GameManager.Instance.SaveData.dockId;
			int dockSlotIndex = GameManager.Instance.SaveData.dockSlotIndex;
			DockPOI dockPOI = list.Find((DockPOI dockPoi) => dockPoi.dock.Data.Id == dockId);
			if (dockPOI == null)
			{
				CustomDebug.EditorLogError("Failed to find dock with id: " + dockId + " to spawn player at");
			}
			Transform transform = null;
			if (dockPOI.dockSlots.Length == 0)
			{
				CustomDebug.EditorLogError("Dock id " + dockId + " has no slots to spawn player in.");
			}
			else if (GameManager.Instance.SaveData.dockSlotIndex < dockPOI.dockSlots.Length)
			{
				transform = dockPOI.dockSlots[GameManager.Instance.SaveData.dockSlotIndex];
			}
			else
			{
				transform = dockPOI.dockSlots[0];
			}
			Player componentInChildren = global::UnityEngine.Object.Instantiate<GameObject>(handle.Result, transform.position, Quaternion.identity).GetComponentInChildren<Player>();
			componentInChildren.Dock(dockPOI.dock, dockSlotIndex, false);
			componentInChildren.transform.rotation = transform.rotation;
			float num = Mathf.Clamp01(GameManager.Instance.WaveController.SampleWaveSteepnessAtPosition(componentInChildren.transform.position) * 10f);
			Vector3 waveDisplacement = WaveDisplacement.GetWaveDisplacement(componentInChildren.transform.position, GameManager.Instance.WaveController.Steepness * num, GameManager.Instance.WaveController.Wavelength, GameManager.Instance.WaveController.Speed, GameManager.Instance.WaveController.Directions);
			waveDisplacement.y += this.playerSpawnYOffset;
			componentInChildren.transform.position = waveDisplacement;
		}
		this.IsPlayerLoaded = true;
		yield break;
	}

	private IEnumerator LoadHarvestPOIs()
	{
		GameManager.Instance.SaveData.serializedCrabPotPOIs.ForEach(new Action<SerializedCrabPotPOIData>(this.CreatePlacedHarvestPOI));
		this.ArePOIsLoaded = true;
		yield return null;
		yield break;
	}

	private IEnumerator LoadTeleportAnchor()
	{
		if (GameManager.Instance.SaveData.GetHasTeleportAnchor())
		{
			GameManager.Instance.ItemLogicHandler.CreateTeleportAnchor(GameManager.Instance.SaveData.GetTeleportAnchorPosition(), false);
		}
		this.IsTeleportAnchorLoaded = true;
		yield return null;
		yield break;
	}

	public void CreatePlacedHarvestPOI(SerializedCrabPotPOIData data)
	{
		float yRotation = data.yRotation;
		Vector3 vector = new Vector3(data.x, 0f, data.z);
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>((this.placedMaterialHHarvesterData.id == data.deployableItemId) ? this.placedMaterialPOIPrefab : this.placedPOIPrefab, vector, Quaternion.identity, this.harvestPoiContainer.transform);
		gameObject.transform.eulerAngles = new Vector3(0f, yRotation, 0f);
		gameObject.name = "PlacedHarvestPOI";
		HarvestPOI component = gameObject.GetComponent<HarvestPOI>();
		if (component)
		{
			component.Harvestable = data;
			Cullable component2 = component.GetComponent<Cullable>();
			if (component2)
			{
				GameManager.Instance.CullingBrain.AddCullable(component2);
			}
		}
		if (data != null)
		{
			data.Init();
		}
	}

	public static GameSceneInitializer Instance;

	[SerializeField]
	private GameObject placedPOIPrefab;

	[SerializeField]
	private GameObject placedMaterialPOIPrefab;

	[SerializeField]
	private DeployableItemData placedMaterialHHarvesterData;

	[SerializeField]
	private GameObject harvestPoiContainer;

	[SerializeField]
	private AssetReference playerPrefabReference;

	[SerializeField]
	private float playerSpawnYOffset;

	private bool IsPlayerLoaded;

	private bool ArePOIsLoaded;

	private bool IsTeleportAnchorLoaded;
}
