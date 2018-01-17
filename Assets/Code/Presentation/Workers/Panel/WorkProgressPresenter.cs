using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.UseCases.Work;

namespace Workshop.Presentation.Workers.Panel
{
	public class WorkProgressPresenter : MonoBehaviour, IDisplayProgress
	{
		[SerializeField]
		private Image _progressImage;

		[SerializeField]
		private Text _workText;

		public void ShowProgress(float progress)
		{
			_progressImage.fillAmount = progress;
			_workText.enabled = progress == 0;
		}
	}
}
