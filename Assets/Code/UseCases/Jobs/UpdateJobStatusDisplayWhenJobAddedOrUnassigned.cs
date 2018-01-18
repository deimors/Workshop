using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateJobStatusDisplayWhenJobAddedOrUnassigned
	{
		public UpdateJobStatusDisplayWhenJobAddedOrUnassigned(JobIdentifier jobId, IObservable<WorkshopEvent> workshopEvents, IDisplayJobStatus displayStatus)
		{
			var jobAddedEvents = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Where(jobAdded => jobAdded.Job.Id == jobId);

			var jobUnassignedEvents = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.JobId == jobId);

			var jobIsCompleted = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobStatusUpdated>()
				.Where(statusUpdated => statusUpdated.JobId == jobId)
				.Select(statusUpdated => statusUpdated.NewStatus.IsCompleted)
				.Merge(jobAddedEvents.Select(jobAdded => jobAdded.Job.Status.IsCompleted));

			jobAddedEvents.AsUnitObservable()
				.Merge(jobUnassignedEvents.AsUnitObservable())
				.CombineLatest(jobIsCompleted, (_, isCompleted) => isCompleted ? "Completed" : "Unassigned")
				.Subscribe(statusText => displayStatus.Status = statusText);
		}
	}
}
