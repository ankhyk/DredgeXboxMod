using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool : MonoBehaviour
{
	private void Awake()
	{
		ObjectPool._instance = this;
		if (this.startupPoolMode == ObjectPool.StartupPoolMode.Awake)
		{
			ObjectPool.CreateStartupPools();
		}
	}

	private void Start()
	{
		if (this.startupPoolMode == ObjectPool.StartupPoolMode.Start)
		{
			ObjectPool.CreateStartupPools();
		}
	}

	public static void CreateStartupPools()
	{
		if (!ObjectPool.instance.startupPoolsCreated)
		{
			ObjectPool.instance.startupPoolsCreated = true;
			ObjectPool.StartupPool[] array = ObjectPool.instance.startupPools;
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ObjectPool.CreatePool(array[i].prefab, array[i].size);
				}
			}
		}
	}

	public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
	{
		ObjectPool.CreatePool(prefab.gameObject, initialPoolSize);
	}

	public static void CreatePool(GameObject prefab, int initialPoolSize)
	{
		if (prefab != null && !ObjectPool.instance.pooledObjects.ContainsKey(prefab))
		{
			List<GameObject> list = new List<GameObject>();
			ObjectPool.instance.pooledObjects.Add(prefab, list);
			if (initialPoolSize > 0)
			{
				bool activeSelf = prefab.activeSelf;
				prefab.SetActive(false);
				Transform transform = ObjectPool.instance.transform;
				while (list.Count < initialPoolSize)
				{
					GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab);
					gameObject.transform.parent = transform;
					list.Add(gameObject);
				}
				prefab.SetActive(activeSelf);
			}
		}
	}

	public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Transform parent) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	public static T Spawn<T>(T prefab) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		List<GameObject> list;
		GameObject gameObject;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			gameObject = null;
			if (list.Count > 0)
			{
				while (gameObject == null && list.Count > 0)
				{
					gameObject = list[0];
					list.RemoveAt(0);
				}
				if (gameObject != null)
				{
					Transform transform = gameObject.transform;
					transform.parent = parent;
					transform.localPosition = position;
					transform.localRotation = rotation;
					gameObject.SetActive(true);
					ObjectPool.instance.spawnedObjects.Add(gameObject, prefab);
					return gameObject;
				}
			}
			gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab);
			Transform transform2 = gameObject.transform;
			transform2.parent = parent;
			transform2.localPosition = position;
			transform2.localRotation = rotation;
			ObjectPool.instance.spawnedObjects.Add(gameObject, prefab);
			return gameObject;
		}
		gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab);
		Transform component = gameObject.GetComponent<Transform>();
		component.SetParent(parent, false);
		component.localPosition = position;
		component.localRotation = rotation;
		return gameObject;
	}

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Spawn(prefab, null, position, rotation);
	}

	public static GameObject Spawn(GameObject prefab, Transform parent)
	{
		return ObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab)
	{
		return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	public static void Recycle<T>(T obj) where T : Component
	{
		ObjectPool.Recycle(obj.gameObject);
	}

	public static void Recycle(GameObject obj)
	{
		GameObject gameObject;
		if (ObjectPool.instance.spawnedObjects.TryGetValue(obj, out gameObject))
		{
			ObjectPool.Recycle(obj, gameObject);
			return;
		}
		global::UnityEngine.Object.Destroy(obj);
	}

	private static void Recycle(GameObject obj, GameObject prefab)
	{
		ObjectPool.instance.pooledObjects[prefab].Add(obj);
		ObjectPool.instance.spawnedObjects.Remove(obj);
		obj.transform.parent = ObjectPool.instance.transform;
		obj.SetActive(false);
	}

	public static void RecycleAll<T>(T prefab) where T : Component
	{
		ObjectPool.RecycleAll(prefab.gameObject);
	}

	public static void RecycleAll(GameObject prefab)
	{
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				ObjectPool.tempList.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < ObjectPool.tempList.Count; i++)
		{
			ObjectPool.Recycle(ObjectPool.tempList[i]);
		}
		ObjectPool.tempList.Clear();
	}

	public static void RecycleAll()
	{
		ObjectPool.tempList.AddRange(ObjectPool.instance.spawnedObjects.Keys);
		for (int i = 0; i < ObjectPool.tempList.Count; i++)
		{
			ObjectPool.Recycle(ObjectPool.tempList[i]);
		}
		ObjectPool.tempList.Clear();
	}

	public static bool IsSpawned(GameObject obj)
	{
		return ObjectPool.instance.spawnedObjects.ContainsKey(obj);
	}

	public static int CountPooled<T>(T prefab) where T : Component
	{
		return ObjectPool.CountPooled(prefab.gameObject);
	}

	public static int CountPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			return list.Count;
		}
		return 0;
	}

	public static int CountSpawned<T>(T prefab) where T : Component
	{
		return ObjectPool.CountSpawned(prefab.gameObject);
	}

	public static int CountSpawned(GameObject prefab)
	{
		int num = 0;
		foreach (GameObject gameObject in ObjectPool.instance.spawnedObjects.Values)
		{
			if (prefab == gameObject)
			{
				num++;
			}
		}
		return num;
	}

	public static int CountAllPooled()
	{
		int num = 0;
		foreach (List<GameObject> list in ObjectPool.instance.pooledObjects.Values)
		{
			num += list.Count;
		}
		return num;
	}

	public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> list2;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list2))
		{
			list.AddRange(list2);
		}
		return list;
	}

	public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> list2;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab.gameObject, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].GetComponent<T>());
			}
		}
		return list;
	}

	public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		GameObject gameObject = prefab.gameObject;
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == gameObject)
			{
				list.Add(keyValuePair.Key.GetComponent<T>());
			}
		}
		return list;
	}

	public static void DestroyPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				global::UnityEngine.Object.Destroy(list[i]);
			}
			list.Clear();
		}
	}

	public static void DestroyPooled<T>(T prefab) where T : Component
	{
		ObjectPool.DestroyPooled(prefab.gameObject);
	}

	public static void DestroyAll(GameObject prefab)
	{
		ObjectPool.RecycleAll(prefab);
		ObjectPool.DestroyPooled(prefab);
	}

	public static void DestroyAll<T>(T prefab) where T : Component
	{
		ObjectPool.DestroyAll(prefab.gameObject);
	}

	public static ObjectPool instance
	{
		get
		{
			if (ObjectPool._instance != null)
			{
				return ObjectPool._instance;
			}
			ObjectPool._instance = global::UnityEngine.Object.FindObjectOfType<ObjectPool>();
			if (ObjectPool._instance != null)
			{
				return ObjectPool._instance;
			}
			ObjectPool._instance = new GameObject("ObjectPool")
			{
				transform = 
				{
					localPosition = Vector3.zero,
					localRotation = Quaternion.identity,
					localScale = Vector3.one
				}
			}.AddComponent<ObjectPool>();
			return ObjectPool._instance;
		}
	}

	private static ObjectPool _instance;

	private static List<GameObject> tempList = new List<GameObject>();

	private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

	private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

	public ObjectPool.StartupPoolMode startupPoolMode;

	public ObjectPool.StartupPool[] startupPools;

	private bool startupPoolsCreated;

	public enum StartupPoolMode
	{
		Awake,
		Start,
		CallManually
	}

	[Serializable]
	public class StartupPool
	{
		public int size;

		public GameObject prefab;
	}
}
