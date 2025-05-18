using System;
using AeLa.EasyFeedback;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FeedbackFormHandler : MonoBehaviour
{
	private void OnEnable()
	{
		SceneManager.sceneUnloaded += this.OnSceneUnloaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
	}

	private void OnSceneUnloaded(Scene scene)
	{
		if (this.feedbackForm.IsOpen)
		{
			this.feedbackForm.Hide();
		}
		this.scrim.SetActive(false);
	}

	public void OnFormOpened()
	{
		ApplicationEvents.Instance.TriggerFeedbackFormToggled(true);
		Time.timeScale = 0f;
		this.scrim.SetActive(true);
	}

	public void OnFormClosed()
	{
		ApplicationEvents.Instance.TriggerFeedbackFormToggled(false);
		if (!GameManager.Instance.IsPaused)
		{
			Time.timeScale = 1f;
		}
		this.scrim.SetActive(false);
	}

	private void OnDestroy()
	{
		if (this.feedbackForm.IsOpen)
		{
			this.OnFormClosed();
		}
	}

	[SerializeField]
	private FeedbackForm feedbackForm;

	[SerializeField]
	private GameObject scrim;
}
