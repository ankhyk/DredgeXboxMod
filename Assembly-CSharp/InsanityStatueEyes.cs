using System;
using System.Collections.Generic;
using UnityEngine;

public class InsanityStatueEyes : MonoBehaviour
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
		this.timeUntilNextEvaluate = global::UnityEngine.Random.Range(this.evaluationIntervalMinSec, this.evaluationIntervalMaxSec);
		this.player = GameManager.Instance.Player;
		if (this.player)
		{
			this.angle = Vector3.SignedAngle(this.player.transform.position - base.transform.position, base.transform.forward, Vector3.up);
			this.weightLeft = (1f - Mathf.InverseLerp(-90f, 0f, this.angle)) * 100f;
			this.weightRight = Mathf.InverseLerp(0f, 90f, this.angle) * 100f;
			this.meshRenderers.ForEach(delegate(SkinnedMeshRenderer smr)
			{
				smr.SetBlendShapeWeight(0, this.weightLeft);
				smr.SetBlendShapeWeight(1, this.weightRight);
			});
		}
	}

	[SerializeField]
	private List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>();

	[SerializeField]
	private float evaluationIntervalMinSec;

	[SerializeField]
	private float evaluationIntervalMaxSec;

	private float timeUntilNextEvaluate;

	private float weightLeft;

	private float weightRight;

	private float angle;

	private Player player;
}
