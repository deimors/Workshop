using System;
using UniRx;
using UnityEngine;
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
				.Where(statusUpdated => statusUpdated.JobId == jobId && statusUpdated.NewStatus.IsFinished)
				.Select(_ => false)
				.Merge(jobAddedEvents.Select(_ => false));

			jobUnassignedEvents.AsUnitObservable()
				.Merge(jobAddedEvents.AsUnitObservable())
				.CombineLatest(jobIsCompleted, (_, isCompleted) => isCompleted ? "Completed" : "Unassigned")
				.Subscribe(statusText => displayStatus.Status = statusText);
		}
	}
}
