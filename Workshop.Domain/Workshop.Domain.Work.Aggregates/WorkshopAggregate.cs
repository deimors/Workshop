using Functional.Maybe;
using System.Linq;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopAggregate : AggregateRoot<WorkshopEvent, WorkshopState>, IHandleCommand<WorkshopCommand, WorkshopError>
	{
		public WorkshopAggregate() : base(new WorkshopState()) { }

		public Maybe<WorkshopError> HandleCommand(WorkshopCommand command)
			=> command.Match(
				AddWorker,
				AddJob,
				AssignJob,
				UnassignWorker,
				UpdateJobStatus
			);

		private Maybe<WorkshopError> AddWorker(WorkshopCommand.AddWorker command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => State.Workers.ContainsKey(command.Worker.Id), () => WorkshopError.WorkerAlreadyAdded)
				.Record(() => new WorkshopEvent.WorkerAdded(command.Worker))
				.Execute();

		private Maybe<WorkshopError> AddJob(WorkshopCommand.AddJob command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => State.Jobs.ContainsKey(command.Job.Id), () => WorkshopError.JobAlreadyAdded)
				.Record(() => new WorkshopEvent.JobAdded(command.Job))
				.Execute();

		private Maybe<WorkshopError> AssignJob(WorkshopCommand.AssignJob command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Workers.ContainsKey(command.WorkerId), () => WorkshopError.UnknownWorker)
				.FailIf(() => !State.Jobs.ContainsKey(command.JobId), () => WorkshopError.UnknownJob)
				.RecordIf(() => State.Assignments.Values.Contains(command.WorkerId), () => new WorkshopEvent.JobUnassigned(command.WorkerId, GetAssignedJob(command.WorkerId)))
				.RecordIf(() => State.Assignments.ContainsKey(command.JobId), () => new WorkshopEvent.JobUnassigned(State.Assignments[command.JobId], command.JobId))
				.Record(() => new WorkshopEvent.JobAssigned(command.WorkerId, command.JobId))
				.Execute();

		private Maybe<WorkshopError> UpdateJobStatus(WorkshopCommand.UpdateJobStatus command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Jobs.ContainsKey(command.JobId), () => WorkshopError.UnknownJob)
				.Record(() => new WorkshopEvent.JobStatusUpdated(command.JobId, command.Status))
				.Execute();
		
		private Maybe<WorkshopError> UnassignWorker(WorkshopCommand.UnassignWorker command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Workers.ContainsKey(command.WorkerId), () => WorkshopError.UnknownWorker)
				.FailIf(() => !State.Assignments.Values.Contains(command.WorkerId), () => WorkshopError.WorkerNotAssigned)
				.Record(() => new WorkshopEvent.JobUnassigned(command.WorkerId, GetAssignedJob(command.WorkerId)))
				.Execute();

		private JobIdentifier GetAssignedJob(WorkerIdentifier workerId)
			=> State.Assignments.Single(pair => pair.Value == workerId).Key;
	}
}
