using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateWorkButtonInteractableWhenWorkerAssignedOrUnassigned
	{
		public UpdateWorkButtonInteractableWhenWorkerAssignedOrUnassigned(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents, IDisplayWorkButtonInteractable displayWorkButtonInteractable)
		{
			var assignments = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.WorkerId == workerId)
				.Select(_ => true);

			var unassignments = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.WorkerId == workerId)
				.Select(_ => false);

			assignments.Merge(unassignments)
				.Subscribe(interactable => displayWorkButtonInteractable.Interactable = interactable);
		}
	}
}
