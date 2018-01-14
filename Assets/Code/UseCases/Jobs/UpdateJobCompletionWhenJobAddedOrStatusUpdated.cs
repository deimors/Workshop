using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateJobCompletionWhenJobAddedOrStatusUpdated
	{
		public UpdateJobCompletionWhenJobAddedOrStatusUpdated(JobIdentifier jobId, IObservable<WorkshopEvent> workshopEvents, IDisplayJobCompletion displayJobCompletion)
		{
			var addedStatus = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Where(jobAdded => jobAdded.Job.Id == jobId)
				.Select(jobAdded => jobAdded.Job.Status);

			var updatedStatus = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobStatusUpdated>()
				.Where(jobStatusUpdated => jobStatusUpdated.JobId == jobId)
				.Select(jobStatusUpdated => jobStatusUpdated.NewStatus);

			addedStatus
				.Merge(updatedStatus)
				.Select(GetPercentageComplete)
				.Subscribe(percentageComplete => displayJobCompletion.PercentComplete = percentageComplete);
		}

		private static float GetPercentageComplete(JobStatus jobStatus)
			=> jobStatus.Completed / jobStatus.Total;
	}
}
