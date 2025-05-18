using System;
using UnityEngine;

public class InsanityStatue : MonoBehaviour
{
	private void Update()
	{
		this.timeUntilNextEvaluate -= Time.deltaTime;
		if (this.timeUntilNextEvaluate <= 0f)
		{
			this.Evaluate();
		}
	}

	private void Evaluate()
	{
		this.timeUntilNextEvaluate = this.evaluationIntervalSec;
		Player player = GameManager.Instance.Player;
		if (player)
		{
			float num = Vector3.Distance(base.transform.position, player.transform.position);
			float num2 = Mathf.InverseLerp(0f, this.maxDistance, num);
			float num3 = this.panicToDistanceThreshold.Evaluate(num2);
			bool flag = player.Sanity.CurrentSanity < num3;
			this.toggleObject.SetActive(flag);
		}
	}

	[SerializeField]
	private AnimationCurve panicToDistanceThreshold;

	[SerializeField]
	private float maxDistance;

	[SerializeField]
	private float evaluationIntervalSec;

	[SerializeField]
	private GameObject toggleObject;

	private float timeUntilNextEvaluate;
}
