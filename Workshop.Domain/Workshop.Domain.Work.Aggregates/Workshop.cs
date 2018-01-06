using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Generic;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopEvent : OneOfBase<WorkshopEvent.WorkerAdded, WorkshopEvent.JobAdded>
	{
		public class WorkerAdded : WorkshopEvent, IEquatable<WorkerAdded>
		{
			public WorkerIdentifier WorkerId { get; }

			public WorkerAdded(WorkerIdentifier workerId)
			{
				WorkerId = workerId;
			}

			public override bool Equals(object obj) 
				=> Equals(obj as WorkerAdded);

			public bool Equals(WorkerAdded other) 
				=> !(other is null) && WorkerId.Equals(other.WorkerId);

			public override int GetHashCode()
			{
				var hashCode = -456856358;
				hashCode = hashCode * -1521134295 + WorkerId.GetHashCode();
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
				Job = job;
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
	}

	public enum WorkshopError
	{
		WorkerAlreadyAdded,
		JobAlreadyAdded
	}

	public class Workshop : AggregateRoot<WorkshopEvent>
	{
		private readonly ICollection<WorkerIdentifier> _workers = new HashSet<WorkerIdentifier>();
		private readonly IDictionary<JobIdentifier, Job> _jobs = new Dictionary<JobIdentifier, Job>();

		public Maybe<WorkshopError> AddWorker(WorkerIdentifier workerId)
		{
			if (_workers.Contains(workerId))
				return WorkshopError.WorkerAlreadyAdded.ToMaybe();
			
			Record(new WorkshopEvent.WorkerAdded(workerId));

			return Maybe<WorkshopError>.Nothing;
		}

		public Maybe<WorkshopError> AddJob(Job job)
		{
			if (_jobs.ContainsKey(job.Id))
				return WorkshopError.JobAlreadyAdded.ToMaybe();

			Record(new WorkshopEvent.JobAdded(job));

			return Maybe<WorkshopError>.Nothing;
		}

		protected override void ApplyEvent(WorkshopEvent @event)
			=> @event.Switch(
				workerAdded => _workers.Add(workerAdded.WorkerId),
				jobAdded => _jobs.Add(jobAdded.Job.Id, jobAdded.Job)
			);
	}
}
