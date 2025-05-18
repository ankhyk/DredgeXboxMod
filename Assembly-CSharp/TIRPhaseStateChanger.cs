using System;
using System.Collections.Generic;
using UnityEngine;

public class TIRPhaseStateChanger : MonoBehaviour
{
	private void Awake()
	{
		switch (GameManager.Instance.SaveData.TIRWorldPhase)
		{
		case 0:
			this.Phase0();
			return;
		case 1:
			this.Phase1();
			return;
		case 2:
			this.Phase2();
			return;
		case 3:
			this.Phase3();
			return;
		case 4:
			this.Phase4();
			return;
		case 5:
			this.Phase5();
			return;
		}
		this.Phase6();
	}

	private void Phase0()
	{
		this.drillHead.SetActive(false);
		this.SetCrackMaterials(0);
	}

	private void Phase1()
	{
		this.SetDrillDown();
		this.SetCrackMaterials(1);
	}

	private void Phase2()
	{
		this.SetDrillUp();
		this.SetCrackMaterials(2);
	}

	private void Phase3()
	{
		this.SetDrillUp();
		this.SetCrackMaterials(3);
		if (GameManager.Instance.DialogueRunner.GetHasVisitedNode("Defenses_Tier1_Constructed"))
		{
			this.defensesVFX.SetActive(true);
			return;
		}
		this.tentacles.Show();
	}

	private void Phase4()
	{
		this.SetDrillDown();
		this.SetCrackMaterials(4);
		this.defensesVFX.SetActive(true);
	}

	private void Phase5()
	{
		this.SetDrillDown();
		this.SetCrackMaterials(5);
		this.defensesVFX.SetActive(true);
	}

	private void Phase6()
	{
		this.drillHead.SetActive(false);
		this.SetCrackMaterials(6);
		this.rigBaseBroken.SetActive(true);
		this.rigBaseRegular.SetActive(false);
	}

	private void SetCrackMaterials(int tirPhase)
	{
		for (int i = 0; i < this.crackMaterials.Count; i++)
		{
			this.crackMaterials[i].SetFloat("_CrackExpansion", (tirPhase > i) ? 1f : 0f);
		}
	}

	private void SetDrillUp()
	{
		this.drillHead.SetActive(true);
		Vector3 position = this.drillHead.transform.position;
		position.y = this.drillHeadUpPos;
		this.drillHead.transform.position = position;
		Vector3 localScale = this.drillPipe.transform.localScale;
		localScale.y = this.drillPipeUpScale;
		this.drillPipe.transform.localScale = localScale;
	}

	private void SetDrillDown()
	{
		this.drillHead.SetActive(true);
		Vector3 position = this.drillHead.transform.position;
		position.y = this.drillHeadDownPos;
		this.drillHead.transform.position = position;
		Vector3 localScale = this.drillPipe.transform.localScale;
		localScale.y = this.drillPipeDownScale;
		this.drillPipe.transform.localScale = localScale;
	}

	[SerializeField]
	private GameObject drillHead;

	[SerializeField]
	private GameObject drillPipe;

	[SerializeField]
	private float drillHeadUpPos;

	[SerializeField]
	private float drillPipeUpScale;

	[SerializeField]
	private float drillHeadDownPos;

	[SerializeField]
	private float drillPipeDownScale;

	[SerializeField]
	private List<Material> crackMaterials = new List<Material>();

	[SerializeField]
	private TIRTentacles tentacles;

	[SerializeField]
	private GameObject rigBaseRegular;

	[SerializeField]
	private GameObject rigBaseBroken;

	[SerializeField]
	private GameObject defensesVFX;
}
