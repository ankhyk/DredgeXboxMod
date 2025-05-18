using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatModelProxy : MonoBehaviour
{
	public MeshRenderer BoatMeshRenderer
	{
		get
		{
			return this.boatMeshRenderer;
		}
	}

	public List<MeshRenderer> LightMeshRenderers
	{
		get
		{
			return this.lightMeshRenderers;
		}
	}

	public GameObject[] Lights
	{
		get
		{
			return this.lights;
		}
	}

	public GameObject[] LightBeams
	{
		get
		{
			return this.lightBeams;
		}
	}

	public Transform DeployPosition
	{
		get
		{
			return this.deployPosition;
		}
	}

	public List<Mesh> DamageStateMeshes
	{
		get
		{
			return this.damageStateMeshes;
		}
	}

	public LightFlickerEffect LightFlickerEffect
	{
		get
		{
			return this.lightFlickerEffect;
		}
	}

	public ParticleSystem ChimneySmoke
	{
		get
		{
			return this.chimneySmoke;
		}
	}

	public GameObject TrawlNetOozeCollider
	{
		get
		{
			return this.trawlNetOozeCollider;
		}
	}

	public Animator GetTrawlNetAnimator()
	{
		Net net = this.allNets.Where((Net n) => n.isActiveAndEnabled).FirstOrDefault<Net>();
		if (net)
		{
			return net.gameObject.GetComponent<Animator>();
		}
		return null;
	}

	public void SetLightStrength(float strength)
	{
		this.boatMeshRenderer.materials[1].SetFloat("_LightStrength", strength);
		this.lightMeshRenderers.ForEach(delegate(MeshRenderer meshRenderer)
		{
			meshRenderer.materials[0].SetFloat("_LightStrength", strength);
		});
	}

	[SerializeField]
	private MeshRenderer boatMeshRenderer;

	[SerializeField]
	private List<MeshRenderer> lightMeshRenderers;

	[SerializeField]
	private GameObject[] lights;

	[SerializeField]
	private GameObject[] lightBeams;

	[SerializeField]
	private Transform deployPosition;

	[SerializeField]
	private List<Net> allNets;

	[SerializeField]
	private List<Mesh> damageStateMeshes;

	[SerializeField]
	private LightFlickerEffect lightFlickerEffect;

	[SerializeField]
	private ParticleSystem chimneySmoke;

	[SerializeField]
	private GameObject trawlNetOozeCollider;
}
