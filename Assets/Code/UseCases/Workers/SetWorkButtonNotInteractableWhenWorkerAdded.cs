using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class SetWorkButtonNotInteractableWhenWorkerAdded
	{
		public SetWorkButtonNotInteractableWhenWorkerAdded(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents, IDisplayWorkButtonInteractable displayWorkButtonInteractable)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerAdded>()
				.Where(workerAdded => workerAdded.Worker.Id == workerId)
				.Subscribe(_ => displayWorkButtonInteractable.Interactable = false);
		}
	}
}
