using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class WorkCompletedSliderPresenter : MonoBehaviour
	{
		[SerializeField]
		private Slider _completedSlider;

		[Inject]
		public JobIdentifier JobId { get; }

		[Inject]
		public void Initialize(IObservable<WorkshopEvent> workshopEvents)
		{
			var addedStatus = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Where(jobAdded => jobAdded.Job.Id == JobId)
				.Select(jobAdded => jobAdded.Job.Status);

			var updatedStatus = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobStatusUpdated>()
				.Where(jobStatusUpdated => jobStatusUpdated.JobId == JobId)
				.Select(jobStatusUpdated => jobStatusUpdated.NewStatus);

			addedStatus
				.Merge(updatedStatus)
				.Select(GetPercentageComplete)
				.Subscribe(SetSliderValue);
		}
		
		private static float GetPercentageComplete(JobStatus jobStatus) 
			=> jobStatus.Completed / jobStatus.Total;

		private void SetSliderValue(float percentage)
			=> _completedSlider.value = percentage;
	}
}
