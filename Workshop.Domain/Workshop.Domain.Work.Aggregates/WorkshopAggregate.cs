using Functional.Maybe;
using System;
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
				StartWork,
				CompleteWork
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
				.FailIf(() => State.Jobs[command.JobId].Status.Busy, () => WorkshopError.JobIsBusy)
				.FailIf(() => State.Workers[command.WorkerId].Status.Busy, () => WorkshopError.WorkerIsBusy)
				.FailIf(() => State.Jobs[command.JobId].Status.IsCompleted, () => WorkshopError.JobCompleted)
				.RecordIf(() => State.Assignments.Values.Contains(command.WorkerId), () => new WorkshopEvent.JobUnassigned(command.WorkerId, GetAssignedJob(command.WorkerId)))
				.RecordIf(() => State.Assignments.ContainsKey(command.JobId), () => new WorkshopEvent.JobUnassigned(State.Assignments[command.JobId], command.JobId))
				.Record(() => new WorkshopEvent.JobAssigned(command.WorkerId, command.JobId))
				.Execute();

		private Maybe<WorkshopError> StartWork(WorkshopCommand.StartWork command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Jobs.ContainsKey(command.JobId), () => WorkshopError.UnknownJob)
				.FailIf(() => !State.Assignments.ContainsKey(command.JobId), () => WorkshopError.WorkerNotAssigned)
				.FailIf(() => State.Jobs[command.JobId].Status.Busy, () => WorkshopError.JobIsBusy)
				.Record(() => BuildWorkerBusyStatusEventFactory(true).Invoke(State.Workers[State.Assignments[command.JobId]]))
				.Record(() => new WorkshopEvent.JobStatusUpdated(command.JobId, State.Jobs[command.JobId].Status.With(busy: _ => true)))
				.Execute();

		private Maybe<WorkshopError> CompleteWork(WorkshopCommand.CompleteWork command)
			=> (this).BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Jobs.ContainsKey(command.JobId), () => WorkshopError.UnknownJob)
				.FailIf(() => !State.Assignments.ContainsKey(command.JobId), () => WorkshopError.WorkerNotAssigned)
				.FailIf(() => !State.Jobs[command.JobId].Status.Busy, () => WorkshopError.WorkNotStarted)
				.Record(() => new WorkshopEvent.JobStatusUpdated(command.JobId, State.Jobs[command.JobId].Status.With(completed: x => x + command.Quantity, busy: _ => false)))
				.Record(() => BuildWorkerBusyStatusEventFactory(false).Invoke(State.Workers[State.Assignments[command.JobId]]))
				.RecordIf(() => State.Jobs[command.JobId].Status.IsCompleted, () => new WorkshopEvent.JobUnassigned(State.Assignments[command.JobId], command.JobId))
				.Execute();
		
		private Maybe<WorkshopError> UnassignWorker(WorkshopCommand.UnassignWorker command)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !State.Workers.ContainsKey(command.WorkerId), () => WorkshopError.UnknownWorker)
				.FailIf(() => !State.Assignments.Values.Contains(command.WorkerId), () => WorkshopError.WorkerNotAssigned)
				.FailIf(() => State.Workers[command.WorkerId].Status.Busy, () => WorkshopError.WorkerIsBusy)
				.Record(() => new WorkshopEvent.JobUnassigned(command.WorkerId, GetAssignedJob(command.WorkerId)))
				.Execute();

		private JobIdentifier GetAssignedJob(WorkerIdentifier workerId)
			=> State.Assignments.Single(pair => pair.Value == workerId).Key;

		private static Func<Worker, WorkshopEvent> BuildWorkerBusyStatusEventFactory(bool busyStatus)
			=> worker => new WorkshopEvent.WorkerStatusUpdated(worker.Id, worker.Status.With(busy: _ => busyStatus));
	}
}
