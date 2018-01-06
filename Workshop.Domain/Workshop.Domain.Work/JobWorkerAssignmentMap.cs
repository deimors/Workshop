using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class JobWorkerAssignmentMap : IEquatable<JobWorkerAssignmentMap>
	{
		public static readonly JobWorkerAssignmentMap Empty = new JobWorkerAssignmentMap();

		private readonly IImmutableDictionary<JobIdentifier, WorkerIdentifier> _assignmentsByJob;
		private readonly IImmutableDictionary<WorkerIdentifier, JobIdentifier> _assignmentsByWorker;

		private JobWorkerAssignmentMap() : this(ImmutableDictionary<JobIdentifier, WorkerIdentifier>.Empty, ImmutableDictionary<WorkerIdentifier, JobIdentifier>.Empty) { }

		private JobWorkerAssignmentMap(IImmutableDictionary<JobIdentifier, WorkerIdentifier> assignmentsByJob, IImmutableDictionary<WorkerIdentifier, JobIdentifier> assignmentsByWorker)
		{
			_assignmentsByJob = assignmentsByJob;
			_assignmentsByWorker = assignmentsByWorker;
		}

		public Maybe<JobIdentifier> this[WorkerIdentifier worker] 
			=> _assignmentsByWorker.Lookup(worker);

		public Maybe<WorkerIdentifier> this[JobIdentifier job] 
			=> _assignmentsByJob.Lookup(job);

		public JobWorkerAssignmentMap WithAssignment(JobIdentifier job, WorkerIdentifier worker) 
			=> new JobWorkerAssignmentMap(
				this[worker]
					.SelectOrElse(oldJob => _assignmentsByJob.Remove(oldJob), () => _assignmentsByJob)
					.Remove(job)
					.Add(job, worker),
				this[job]
					.SelectOrElse(oldWorker => _assignmentsByWorker.Remove(oldWorker), () => _assignmentsByWorker)
					.Remove(worker)
					.Add(worker, job)
			);

		public JobWorkerAssignmentMap WithoutAssignment(JobIdentifier job)
			=> new JobWorkerAssignmentMap(
				_assignmentsByJob.Remove(job),
				this[job].SelectOrElse(
					worker => _assignmentsByWorker.Remove(worker),
					() => _assignmentsByWorker
				)
			);
		
		public JobWorkerAssignmentMap WithoutAssignment(WorkerIdentifier worker)
			=> new JobWorkerAssignmentMap(
				this[worker].SelectOrElse(
					job => _assignmentsByJob.Remove(job),
					() => _assignmentsByJob
				),
				_assignmentsByWorker.Remove(worker)
			);

		public override bool Equals(object obj)
			=> Equals(obj as JobWorkerAssignmentMap);

		public bool Equals(JobWorkerAssignmentMap other)
			=> other != null
			&& _assignmentsByJob.Equals(other._assignmentsByJob);

		public override int GetHashCode() 
			=> _assignmentsByJob.GetHashCode();

		public static bool operator ==(JobWorkerAssignmentMap map1, JobWorkerAssignmentMap map2) 
			=> EqualityComparer<JobWorkerAssignmentMap>.Default.Equals(map1, map2);

		public static bool operator !=(JobWorkerAssignmentMap map1, JobWorkerAssignmentMap map2) 
			=> !(map1 == map2);
	}
}
