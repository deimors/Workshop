using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopEvent : OneOfBase<WorkshopEvent.WorkerAdded, WorkshopEvent.JobAdded, WorkshopEvent.JobAssigned, WorkshopEvent.JobUnassigned>
	{
		public class WorkerAdded : WorkshopEvent, IEquatable<WorkerAdded>
		{
			public Worker Worker { get; }

			public WorkerAdded(Worker worker)
			{
				Worker = worker ?? throw new ArgumentNullException(nameof(worker));
			}

			public override bool Equals(object obj) 
				=> Equals(obj as WorkerAdded);

			public bool Equals(WorkerAdded other) 
				=> !(other is null) && Worker.Equals(other.Worker);

			public override int GetHashCode()
			{
				var hashCode = -456856358;
				hashCode = hashCode * -1521134295 + Worker.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(WorkerAdded added1, WorkerAdded added2) 
				=> added1?.Equals(added2) ?? added2 is null;

			public static bool operator !=(WorkerAdded added1, WorkerAdded added2) 
				=> !(added1 == added2);
		}

		public class JobAdded : WorkshopEvent, IEquatable<JobAdded>
		{
			public Job Job { get; }

			public JobAdded(Job job)
			{
				Job = job ?? throw new ArgumentNullException(nameof(job));
			}

			public override bool Equals(object obj) 
				=> Equals(obj as JobAdded);

			public bool Equals(JobAdded other) 
				=> !(other is null) && Job.Equals(other.Job);

			public override int GetHashCode()
			{
				var hashCode = 1121529980;
				hashCode = hashCode * -1521134295 + Job.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(JobAdded added1, JobAdded added2) 
				=> added1?.Equals(added2) ?? (added2 is null);

			public static bool operator !=(JobAdded added1, JobAdded added2) 
				=> !(added1 == added2);
		}

		public class JobAssigned : WorkshopEvent, IEquatable<JobAssigned>
		{
			public WorkerIdentifier WorkerId { get; }
			public JobIdentifier JobId { get; }

			public JobAssigned(WorkerIdentifier workerId, JobIdentifier jobId)
			{
				WorkerId = workerId ?? throw new ArgumentNullException(nameof(workerId));
				JobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
			}

			public override bool Equals(object obj) 
				=> Equals(obj as JobAssigned);

			public bool Equals(JobAssigned other) 
				=> !(other is null) 
				&& WorkerId.Equals(other.WorkerId) 
				&& JobId.Equals(other.JobId);

			public override int GetHashCode()
			{
				var hashCode = -217597125;
				hashCode = hashCode * -1521134295 + WorkerId.GetHashCode();
				hashCode = hashCode * -1521134295 + JobId.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(JobAssigned assigned1, JobAssigned assigned2) 
				=> assigned1?.Equals(assigned2) ?? (assigned2 is null);

			public static bool operator !=(JobAssigned assigned1, JobAssigned assigned2) 
				=> !(assigned1 == assigned2);
		}

		public class JobUnassigned : WorkshopEvent, IEquatable<JobUnassigned>
		{
			public WorkerIdentifier WorkerId { get; }
			public JobIdentifier JobId { get; }

			public JobUnassigned(WorkerIdentifier workerId, JobIdentifier jobId)
			{
				WorkerId = workerId ?? throw new ArgumentNullException(nameof(workerId));
				JobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
			}

			public override bool Equals(object obj) 
				=> Equals(obj as JobUnassigned);

			public bool Equals(JobUnassigned other) 
				=> !(other is null)
				&& WorkerId.Equals(other.WorkerId)
				&& JobId.Equals(other.JobId);

			public override int GetHashCode()
			{
				var hashCode = -217597125;
				hashCode = hashCode * -1521134295 + WorkerId.GetHashCode();
				hashCode = hashCode * -1521134295 + JobId.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(JobUnassigned unassigned1, JobUnassigned unassigned2) 
				=> unassigned1?.Equals(unassigned2) ?? (unassigned2 is null);

			public static bool operator !=(JobUnassigned unassigned1, JobUnassigned unassigned2) 
				=> !(unassigned1 == unassigned2);
		}
	}

	public enum WorkshopError
	{
		WorkerAlreadyAdded,
		JobAlreadyAdded,
		UnknownWorker,
		UnknownJob
	}

	public class WorkshopAggregate : AggregateRoot<WorkshopEvent>
	{
		private readonly IDictionary<WorkerIdentifier, Worker> _workers = new Dictionary<WorkerIdentifier, Worker>();
		private readonly IDictionary<JobIdentifier, Job> _jobs = new Dictionary<JobIdentifier, Job>();
		private readonly IDictionary<JobIdentifier, WorkerIdentifier> _assignments = new Dictionary<JobIdentifier, WorkerIdentifier>();

		public Maybe<WorkshopError> AddWorker(Worker worker)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => _workers.ContainsKey(worker.Id), () => WorkshopError.WorkerAlreadyAdded)
				.Record(new WorkshopEvent.WorkerAdded(worker))
				.Execute();

		public Maybe<WorkshopError> AddJob(Job job)
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => _jobs.ContainsKey(job.Id), () => WorkshopError.JobAlreadyAdded)
				.Record(new WorkshopEvent.JobAdded(job))
				.Execute();

		public Maybe<WorkshopError> AssignJob(WorkerIdentifier workerId, JobIdentifier jobId) 
			=> this.BuildCommand<WorkshopEvent, WorkshopError>()
				.FailIf(() => !_workers.ContainsKey(workerId), () => WorkshopError.UnknownWorker)
				.FailIf(() => !_jobs.ContainsKey(jobId), () => WorkshopError.UnknownJob)
				.RecordIf(() => _assignments.Values.Contains(workerId), () => new WorkshopEvent.JobUnassigned(workerId, _assignments.Single(pair => pair.Value == workerId).Key))
				.RecordIf(() => _assignments.ContainsKey(jobId), () => new WorkshopEvent.JobUnassigned(_assignments[jobId], jobId))
				.Record(new WorkshopEvent.JobAssigned(workerId, jobId))
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
