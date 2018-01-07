using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.Models;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class WorkCompletedSliderPresenter : MonoBehaviour
	{
		[SerializeField]
		private Slider _completedSlider;

		/*
		[Inject]
		public void Initialize(IReadJob workModel)
			=> workModel.Value
				.Select(GetPercentageComplete)
				.Subscribe(SetSliderValue);
		
		private static float GetPercentageComplete(Job job) 
			=> job.Status.Completed / job.Status.Total;

		private void SetSliderValue(float percentage)
			=> _completedSlider.value = percentage;
		*/
	}
}
