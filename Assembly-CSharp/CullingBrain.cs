using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CullingBrain : SerializedMonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.CullingBrain = this;
		ApplicationEvents.Instance.OnGameLoaded += this.OnGameLoaded;
	}

	private void OnGameLoaded()
	{
		this.cullingGroups.Clear();
		CullingGroupType[] array = (CullingGroupType[])Enum.GetValues(typeof(CullingGroupType));
		for (int i = 0; i < array.Length; i++)
		{
			CullingGroupType cullingGroupType = array[i];
			this.cullables[cullingGroupType] = this.cullables[cullingGroupType].Where((Cullable c) => c != null).ToList<Cullable>();
			CullingGroup cullingGroup = new CullingGroup();
			cullingGroup.targetCamera = Camera.main;
			cullingGroup.onStateChanged = delegate(CullingGroupEvent evt)
			{
				this.OnCullingGroupStateChanged(evt, cullingGroupType);
			};
			cullingGroup.SetDistanceReferencePoint(Camera.main.transform);
			cullingGroup.SetBoundingDistances(this.boundingDistances[cullingGroupType]);
			this.cullingGroups.Add(cullingGroupType, cullingGroup);
			this.RefreshCullables(cullingGroupType);
		}
		this.QueryAllCullables();
	}

	private void OnDestroy()
	{
		if (GameManager.Instance.CullingBrain == this)
		{
			GameManager.Instance.CullingBrain = null;
		}
		this.cullingGroups.Values.ToList<CullingGroup>().ForEach(delegate(CullingGroup c)
		{
			c.Dispose();
		});
		this.cullables.Values.ToList<List<Cullable>>().ForEach(delegate(List<Cullable> c)
		{
			c.Clear();
		});
		this.cullingGroups.Clear();
	}

	private void QueryAllCullables()
	{
		foreach (CullingGroupType cullingGroupType in (CullingGroupType[])Enum.GetValues(typeof(CullingGroupType)))
		{
			CullingGroup cullingGroup = this.cullingGroups[cullingGroupType];
			List<Cullable> list = this.cullables[cullingGroupType];
			for (int j = 0; j < list.Count; j++)
			{
				this.ProcessCullable(list[j], cullingGroup.IsVisible(j), cullingGroup.GetDistance(j));
			}
		}
	}

	public void AddCullable(Cullable cullable)
	{
		this.cullables[cullable.cullingGroupType].Add(cullable);
		this.RefreshCullables(cullable.cullingGroupType);
	}

	public void RemoveCullable(Cullable cullable)
	{
		this.cullables[cullable.cullingGroupType].Remove(cullable);
		this.RefreshCullables(cullable.cullingGroupType);
	}

	public void RefreshCullables(CullingGroupType groupType)
	{
		if (!GameSceneInitializer.Instance.IsDone())
		{
			return;
		}
		List<Cullable> list = this.cullables[groupType];
		int count = list.Count;
		BoundingSphere[] array = new BoundingSphere[count];
		for (int i = 0; i < count; i++)
		{
			if (list[i] == null)
			{
				Debug.LogWarning(string.Format("[CullingBrain] RefreshCullables({0}) cullable at index {1} is null!", groupType, i));
			}
			else
			{
				array[i] = new BoundingSphere(list[i].gameObject.transform.position + list[i].sphereOffset, list[i].sphereRadius);
			}
		}
		CullingGroup cullingGroup = this.cullingGroups[groupType];
		cullingGroup.SetBoundingSpheres(array);
		cullingGroup.SetBoundingSphereCount(count);
	}

	private void OnCullingGroupStateChanged(CullingGroupEvent evt, CullingGroupType groupType)
	{
		Cullable cullable = this.cullables[groupType][evt.index];
		this.ProcessCullable(cullable, evt.isVisible, evt.currentDistance);
	}

	private void ProcessCullable(Cullable cullable, bool isVisible, int distanceBand)
	{
		if (cullable == null)
		{
			Debug.LogWarning("[CullingBrain] ProcessCullable() cullable is null.");
			return;
		}
		if (distanceBand == 0)
		{
			cullable.gameObject.SetActive(true);
			if (cullable.replacerObject)
			{
				cullable.replacerObject.SetActive(false);
				return;
			}
		}
		else if (distanceBand == 1)
		{
			cullable.gameObject.SetActive(isVisible);
			if (cullable.replacerObject)
			{
				cullable.replacerObject.SetActive(false);
				return;
			}
		}
		else if (distanceBand > 1)
		{
			cullable.gameObject.SetActive(false);
			if (cullable.replacerObject)
			{
				cullable.replacerObject.SetActive(isVisible);
			}
		}
	}

	private void UpdateCullables()
	{
	}

	[SerializeField]
	private Dictionary<CullingGroupType, float[]> boundingDistances = new Dictionary<CullingGroupType, float[]>();

	[SerializeField]
	private Dictionary<CullingGroupType, List<Cullable>> cullables = new Dictionary<CullingGroupType, List<Cullable>>();

	private Dictionary<CullingGroupType, CullingGroup> cullingGroups = new Dictionary<CullingGroupType, CullingGroup>();
}
