using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestableParticles : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.HarvestableParticleSystem)
		{
			this.particleCache = new ParticleSystem.Particle[this.HarvestableParticleSystem.particleCount];
		}
		this.UpdateParticles();
	}

	public void Init(bool useOozeParticles)
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>((useOozeParticles && this.disturbedOozeParticles != null) ? this.disturbedOozeParticles : this.disturbedWaterParticles, base.transform);
		LOD lod = default(LOD);
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
		lod.renderers = componentsInChildren;
		this.lodGroup.SetLODs(new LOD[] { lod });
		gameObject.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>().ForEach(new Action<ParticleSystem>(this.toggleParticles.Add));
		this.UpdateParticles();
	}

	public int ParticlesAmount
	{
		get
		{
			return this.particlesAmount;
		}
		set
		{
			this.particlesAmount = value;
			this.UpdateParticles();
		}
	}

	public void SetHarvestParticleOverride(float particleDepth, bool flatten)
	{
		if (this.HarvestableParticleSystem)
		{
			ParticleSystem.ShapeModule shape = this.HarvestableParticleSystem.shape;
			if (flatten)
			{
				shape.shapeType = ParticleSystemShapeType.Circle;
				shape.radiusThickness = 0.5f;
			}
			this.HarvestableParticleSystem.transform.position = new Vector3(this.HarvestableParticleSystem.transform.position.x, particleDepth, this.HarvestableParticleSystem.transform.position.z);
		}
	}

	public void SetSpecialStatus(bool isSpecial)
	{
		if (isSpecial && this.particlesAmount > 0)
		{
			if (this.specialParticleObject == null)
			{
				this.specialParticleObject = global::UnityEngine.Object.Instantiate<GameObject>(this.specialParticlePrefab, base.transform);
			}
			this.specialParticleObject.GetComponentsInChildren<ParticleSystem>(true).ToList<ParticleSystem>().ForEach(delegate(ParticleSystem p)
			{
				p.Play();
			});
			return;
		}
		if (!isSpecial && this.specialParticleObject != null)
		{
			this.specialParticleObject.GetComponentsInChildren<ParticleSystem>(true).ToList<ParticleSystem>().ForEach(delegate(ParticleSystem p)
			{
				p.Stop();
			});
		}
	}

	private void UpdateParticles()
	{
		if (this.HarvestableParticleSystem)
		{
			int particleCount = this.HarvestableParticleSystem.particleCount;
			int num = this.particlesPerStock * this.particlesAmount;
			if (num == 0 && this.particlesAmount > 0)
			{
				num = 1;
			}
			this.HarvestableParticleSystem.main.maxParticles = num;
			if (particleCount > num)
			{
				this.HarvestableParticleSystem.GetParticles(this.particleCache);
				for (int i = Math.Max(0, num); i < Math.Min(particleCount, this.particleCache.Length); i++)
				{
					this.particleCache[i].remainingLifetime = 0.2f;
				}
				this.HarvestableParticleSystem.SetParticles(this.particleCache);
			}
		}
		if (this.particlesAmount <= 0)
		{
			for (int j = 0; j < this.toggleParticles.Count; j++)
			{
				this.toggleParticles[j].Stop();
			}
			for (int k = 0; k < this.toggleObjects.Length; k++)
			{
				this.toggleObjects[k].SetActive(false);
			}
			if (this.specialParticleObject != null)
			{
				this.specialParticleObject.GetComponentsInChildren<ParticleSystem>(true).ToList<ParticleSystem>().ForEach(delegate(ParticleSystem p)
				{
					p.Stop();
				});
				return;
			}
		}
		else
		{
			for (int l = 0; l < this.toggleParticles.Count; l++)
			{
				if (this.toggleParticles[l].isStopped)
				{
					this.toggleParticles[l].Play();
				}
			}
			for (int m = 0; m < this.toggleObjects.Length; m++)
			{
				this.toggleObjects[m].SetActive(true);
			}
		}
	}

	[SerializeField]
	private ParticleSystem HarvestableParticleSystem;

	[SerializeField]
	private int particlesPerStock = 1;

	[SerializeField]
	private GameObject specialParticlePrefab;

	[SerializeField]
	private List<ParticleSystem> toggleParticles = new List<ParticleSystem>();

	[SerializeField]
	private GameObject[] toggleObjects;

	[SerializeField]
	private LODGroup lodGroup;

	[SerializeField]
	private GameObject disturbedWaterParticles;

	[SerializeField]
	private GameObject disturbedOozeParticles;

	private ParticleSystem.Particle[] particleCache;

	private GameObject specialParticleObject;

	private int particlesAmount;
}
