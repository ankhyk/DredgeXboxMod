using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Components;

public class BoatActionsDestinationUI : MonoBehaviour
{
	public SubDestinationButton UndockDestinationButton
	{
		get
		{
			return this.undockDestinationButton;
		}
	}

	public SubDestinationButton RestDestinationButton
	{
		get
		{
			return this.restDestinationButton;
		}
	}

	public SubDestinationButton ResearchDestinationButton
	{
		get
		{
			return this.researchDestinationButton;
		}
	}

	private void OnEnable()
	{
		this.headerContainer.SetActive(false);
		this.frameTransform.gameObject.SetActive(false);
	}

	public void Init(BoatActionsDestination destination)
	{
		this.destination = destination;
		this.restAvailable = GameManager.Instance.SaveData.availableDestinations.Contains(destination.restDestination.Id);
		this.researchAvailable = GameManager.Instance.SaveData.availableDestinations.Contains(destination.researchDestination.Id);
		this.undockDestinationButton.Init(destination.undockDestination);
		SubDestinationButton subDestinationButton = this.undockDestinationButton;
		subDestinationButton.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
		SubDestinationButton subDestinationButton2 = this.undockDestinationButton;
		subDestinationButton2.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton2.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		if (this.restAvailable)
		{
			this.restDestinationButton.gameObject.SetActive(true);
			this.restDestinationButton.Init(destination.restDestination);
			SubDestinationButton subDestinationButton3 = this.restDestinationButton;
			subDestinationButton3.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton3.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
			SubDestinationButton subDestinationButton4 = this.restDestinationButton;
			subDestinationButton4.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton4.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		}
		else
		{
			this.restDestinationButton.gameObject.SetActive(false);
		}
		if (this.researchAvailable)
		{
			this.researchDestinationButton.gameObject.SetActive(true);
			this.researchDestinationButton.Init(destination.researchDestination);
			SubDestinationButton subDestinationButton5 = this.researchDestinationButton;
			subDestinationButton5.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton5.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
			SubDestinationButton subDestinationButton6 = this.researchDestinationButton;
			subDestinationButton6.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Combine(subDestinationButton6.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		}
		else
		{
			this.researchDestinationButton.gameObject.SetActive(false);
		}
		base.StartCoroutine(this.Resize());
	}

	private IEnumerator Resize()
	{
		yield return new WaitForEndOfFrame();
		this.headerTransform.sizeDelta = new Vector2(Mathf.Max(this.minWidth, this.buttonContainerTransform.sizeDelta.x), this.headerTransform.sizeDelta.y);
		this.frameTransform.sizeDelta = new Vector2(this.buttonContainerTransform.rect.width, this.frameTransform.sizeDelta.y);
		this.frameTransform.gameObject.SetActive(true);
		yield break;
	}

	private void OnDestroy()
	{
		SubDestinationButton subDestinationButton = this.undockDestinationButton;
		subDestinationButton.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
		SubDestinationButton subDestinationButton2 = this.undockDestinationButton;
		subDestinationButton2.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton2.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		if (this.restAvailable)
		{
			SubDestinationButton subDestinationButton3 = this.restDestinationButton;
			subDestinationButton3.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton3.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
			SubDestinationButton subDestinationButton4 = this.restDestinationButton;
			subDestinationButton4.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton4.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		}
		if (this.researchAvailable)
		{
			SubDestinationButton subDestinationButton5 = this.researchDestinationButton;
			subDestinationButton5.OnButtonSelectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton5.OnButtonSelectAction, new Action<BaseDestination>(this.OnButtonSelected));
			SubDestinationButton subDestinationButton6 = this.researchDestinationButton;
			subDestinationButton6.OnButtonDeselectAction = (Action<BaseDestination>)Delegate.Remove(subDestinationButton6.OnButtonDeselectAction, new Action<BaseDestination>(this.OnButtonDeselected));
		}
	}

	private void LateUpdate()
	{
		base.transform.position = Camera.main.WorldToScreenPoint(this.destination.transform.position);
	}

	private void OnButtonSelected(BaseDestination baseDestination)
	{
		this.localizedHeaderString.StringReference.SetReference(LanguageManager.STRING_TABLE, baseDestination.Id);
		this.localizedHeaderString.RefreshString();
		this.headerContainer.SetActive(true);
	}

	private void OnButtonDeselected(BaseDestination baseDestination)
	{
		this.headerContainer.SetActive(false);
	}

	[SerializeField]
	private GameObject headerContainer;

	[SerializeField]
	private RectTransform buttonContainerTransform;

	[SerializeField]
	private RectTransform headerTransform;

	[SerializeField]
	private RectTransform frameTransform;

	[SerializeField]
	private float minWidth;

	[SerializeField]
	private LocalizeStringEvent localizedHeaderString;

	[SerializeField]
	private SubDestinationButton undockDestinationButton;

	[SerializeField]
	private SubDestinationButton restDestinationButton;

	[SerializeField]
	private SubDestinationButton researchDestinationButton;

	private BoatActionsDestination destination;

	private bool restAvailable;

	private bool researchAvailable;
}
