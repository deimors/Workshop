using Functional.Maybe;
using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateSelectedJobWhenJobAssignedOrJobUnassignedToWorker
	{
		public UpdateSelectedJobWhenJobAssignedOrJobUnassignedToWorker(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents, IDisplaySelectedJob displaySelectedJob)
		{
			var assigned = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.WorkerId == workerId)
				.Select(jobAssigned => jobAssigned.JobId.ToMaybe());

			var unassigned = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.WorkerId == workerId)
				.Select(_ => Maybe<JobIdentifier>.Nothing);

			assigned.Merge(unassigned)
				.Subscribe(assignment => displaySelectedJob.SelectedJob = assignment);
		}
	}
}
