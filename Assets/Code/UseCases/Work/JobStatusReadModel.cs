using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class JobStatusReadModel
	{
		private readonly IDictionary<JobIdentifier, JobStatus> _jobStatuses = new Dictionary<JobIdentifier, JobStatus>();

		public JobStatusReadModel(IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Subscribe(jobAdded => _jobStatuses[jobAdded.Job.Id] = jobAdded.Job.Status);

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobStatusUpdated>()
				.Subscribe(jobStatusUpdated => _jobStatuses[jobStatusUpdated.JobId] = jobStatusUpdated.NewStatus);
		}

		public JobStatus this[JobIdentifier jobId]
			=> _jobStatuses[jobId];
	}
}
