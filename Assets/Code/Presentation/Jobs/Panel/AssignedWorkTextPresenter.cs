using Functional.Maybe;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class AssignedWorkTextPresenter : MonoBehaviour
	{
		[SerializeField]
		private Text _assignedWorkText;

		[Inject]
		public JobIdentifier Identifier { get; }

		[Inject]
		public void Setup(IObservable<WorkshopEvent> workshopEvents)
		{
			_assignedWorkText.text = GetAssignmentString(Maybe<WorkerIdentifier>.Nothing);

			var assignedEvents = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.JobId == Identifier)
				.Select(jobAssigned => jobAssigned.WorkerId.ToMaybe());

			var unassignedEvents = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.JobId == Identifier)
				.Select(_ => Maybe<WorkerIdentifier>.Nothing);

			assignedEvents
				.Merge(unassignedEvents)
				.Select(GetAssignmentString)
				.SubscribeToText(_assignedWorkText);
		}
		
		private string GetAssignedWorkString(JobWorkerAssignmentMap assigments)
			=> GetAssignmentString(assigments[Identifier]);

		private string GetAssignmentString(Maybe<WorkerIdentifier> maybeWorker)
			=> maybeWorker.SelectOrElse(
				worker => $"Assigned to {worker}",
				() => "Unassigned"
			);
	}
}
