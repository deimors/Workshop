using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateWorkButtonInteractableWhenWorkerStatusUpdated
	{
		public UpdateWorkButtonInteractableWhenWorkerStatusUpdated(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents, IDisplayWorkButtonInteractable displayWorkButtonInteractable)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerStatusUpdated>()
				.Where(workerStatusUpdated => workerStatusUpdated.WorkerId == workerId)
				.Select(workerStatusUpdated => workerStatusUpdated.NewStatus.Busy)
				.Subscribe(busy => displayWorkButtonInteractable.Interactable = !busy);
		}
	}
}
