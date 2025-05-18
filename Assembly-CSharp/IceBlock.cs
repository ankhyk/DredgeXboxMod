using System;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Icebreaker"))
		{
			GameManager.Instance.CullingBrain.RemoveCullable(this.cullable);
			this.iceBlockController.OnIceBlockShatter(base.transform);
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Randomize()
	{
		base.transform.rotation = Quaternion.AngleAxis(global::UnityEngine.Random.Range(0f, 360f), Vector3.up);
		float num = global::UnityEngine.Random.Range(0.65f, 1f);
		base.transform.localScale = new Vector3(num, num, num);
	}

	[SerializeField]
	private IceBlockController iceBlockController;

	[SerializeField]
	private Cullable cullable;
}
