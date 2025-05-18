using System;
using System.Collections.Generic;
using UnityEngine;

public class ReliableOnTriggerExit : MonoBehaviour
{
	public static void NotifyTriggerEnter(Collider c, GameObject caller, ReliableOnTriggerExit._OnTriggerExit onTriggerExit)
	{
		ReliableOnTriggerExit reliableOnTriggerExit = null;
		foreach (ReliableOnTriggerExit reliableOnTriggerExit2 in c.gameObject.GetComponents<ReliableOnTriggerExit>())
		{
			if (reliableOnTriggerExit2.thisCollider == c)
			{
				reliableOnTriggerExit = reliableOnTriggerExit2;
				break;
			}
		}
		if (reliableOnTriggerExit == null)
		{
			reliableOnTriggerExit = c.gameObject.AddComponent<ReliableOnTriggerExit>();
			reliableOnTriggerExit.thisCollider = c;
		}
		if (!reliableOnTriggerExit.waitingForOnTriggerExit.ContainsKey(caller))
		{
			reliableOnTriggerExit.waitingForOnTriggerExit.Add(caller, onTriggerExit);
			reliableOnTriggerExit.enabled = true;
			return;
		}
		reliableOnTriggerExit.ignoreNotifyTriggerExit = true;
		reliableOnTriggerExit.waitingForOnTriggerExit[caller](c);
		reliableOnTriggerExit.ignoreNotifyTriggerExit = false;
	}

	public static void NotifyTriggerExit(Collider c, GameObject caller)
	{
		if (c == null)
		{
			return;
		}
		ReliableOnTriggerExit reliableOnTriggerExit = null;
		foreach (ReliableOnTriggerExit reliableOnTriggerExit2 in c.gameObject.GetComponents<ReliableOnTriggerExit>())
		{
			if (reliableOnTriggerExit2.thisCollider == c)
			{
				reliableOnTriggerExit = reliableOnTriggerExit2;
				break;
			}
		}
		if (reliableOnTriggerExit != null && !reliableOnTriggerExit.ignoreNotifyTriggerExit)
		{
			reliableOnTriggerExit.waitingForOnTriggerExit.Remove(caller);
			if (reliableOnTriggerExit.waitingForOnTriggerExit.Count == 0)
			{
				reliableOnTriggerExit.enabled = false;
			}
		}
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			this.CallCallbacks();
		}
	}

	private void Update()
	{
		if (this.thisCollider == null)
		{
			this.CallCallbacks();
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		if (!this.thisCollider.enabled)
		{
			this.CallCallbacks();
		}
	}

	private void CallCallbacks()
	{
		this.ignoreNotifyTriggerExit = true;
		foreach (KeyValuePair<GameObject, ReliableOnTriggerExit._OnTriggerExit> keyValuePair in this.waitingForOnTriggerExit)
		{
			if (!(keyValuePair.Key == null))
			{
				keyValuePair.Value(this.thisCollider);
			}
		}
		this.ignoreNotifyTriggerExit = false;
		this.waitingForOnTriggerExit.Clear();
		base.enabled = false;
	}

	private Collider thisCollider;

	private bool ignoreNotifyTriggerExit;

	private Dictionary<GameObject, ReliableOnTriggerExit._OnTriggerExit> waitingForOnTriggerExit = new Dictionary<GameObject, ReliableOnTriggerExit._OnTriggerExit>();

	public delegate void _OnTriggerExit(Collider c);
}
