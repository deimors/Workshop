using System.Collections.Generic;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopState : IApplyEvent<WorkshopEvent>
	{
		private readonly Dictionary<WorkerIdentifier, Worker> _workers = new Dictionary<WorkerIdentifier, Worker>();
		private readonly Dictionary<JobIdentifier, Job> _jobs = new Dictionary<JobIdentifier, Job>();
		private readonly Dictionary<JobIdentifier, WorkerIdentifier> _assignments = new Dictionary<JobIdentifier, WorkerIdentifier>();

		public IReadOnlyDictionary<WorkerIdentifier, Worker> Workers => _workers;
		public IReadOnlyDictionary<JobIdentifier, Job> Jobs => _jobs;
		public IReadOnlyDictionary<JobIdentifier, WorkerIdentifier> Assignments => _assignments;

		public void ApplyEvent(WorkshopEvent @event)
			=> @event.Switch(
				workerAdded => _workers.Add(workerAdded.Worker.Id, workerAdded.Worker),
				jobAdded => _jobs.Add(jobAdded.Job.Id, jobAdded.Job),
				jobAssigned => _assignments.Add(jobAssigned.JobId, jobAssigned.WorkerId),
				jobUnassigned => _assignments.Remove(jobUnassigned.JobId),
				jobStatusUpdated => _jobs[jobStatusUpdated.JobId] = Jobs[jobStatusUpdated.JobId].With(status : x => jobStatusUpdated.NewStatus)
			);
	}
}
