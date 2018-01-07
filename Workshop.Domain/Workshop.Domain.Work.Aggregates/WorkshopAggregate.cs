using Functional.Maybe;
using System.Collections.Generic;
using System.Linq;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopAggregate : AggregateRoot<WorkshopEvent>
	{
		private readonly IDictionary<WorkerIdentifier, Worker> _workers = new Dictionary<WorkerIdentifier, Worker>();
		private readonly IDictionary<JobIdentifier, Job> _jobs = new Dictionary<JobIdentifier, Job>();
		private readonly IDictionary<JobIdentifier, WorkerIdentifier> _assignments = new Dictionary<JobIdentifier, WorkerIdentifier>();

		public Maybe<WorkshopError> HandleCommand(WorkshopCommand command)
			=> command.Match(
				AddWorker,
				AddJob,
				AssignJob
			);

		private Maybe<WorkshopError> AddWorker(WorkshopCommand.AddWorker command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => _workers.ContainsKey(command.Worker.Id), () => WorkshopError.WorkerAlreadyAdded)
				.Record(new WorkshopEvent.WorkerAdded(command.Worker))
				.Execute();

		private Maybe<WorkshopError> AddJob(WorkshopCommand.AddJob command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => _jobs.ContainsKey(command.Job.Id), () => WorkshopError.JobAlreadyAdded)
				.Record(new WorkshopEvent.JobAdded(command.Job))
				.Execute();

		private Maybe<WorkshopError> AssignJob(WorkshopCommand.AssignJob command) 
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !_workers.ContainsKey(command.WorkerId), () => WorkshopError.UnknownWorker)
				.FailIf(() => !_jobs.ContainsKey(command.JobId), () => WorkshopError.UnknownJob)
				.RecordIf(() => _assignments.Values.Contains(command.WorkerId), () => new WorkshopEvent.JobUnassigned(command.WorkerId, _assignments.Single(pair => pair.Value == command.WorkerId).Key))
				.RecordIf(() => _assignments.ContainsKey(command.JobId), () => new WorkshopEvent.JobUnassigned(_assignments[command.JobId], command.JobId))
				.Record(new WorkshopEvent.JobAssigned(command.WorkerId, command.JobId))
				.Execute();

		protected override void ApplyEvent(WorkshopEvent @event)
			=> @event.Switch(
				workerAdded => _workers.Add(workerAdded.Worker.Id, workerAdded.Worker),
				jobAdded => _jobs.Add(jobAdded.Job.Id, jobAdded.Job),
				jobAssigned => _assignments.Add(jobAssigned.JobId, jobAssigned.WorkerId),
				jobUnassigned => _assignments.Remove(jobUnassigned.JobId)
			);
	}
}
