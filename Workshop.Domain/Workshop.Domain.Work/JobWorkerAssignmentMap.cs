using Functional.Maybe;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Workshop.Domain.Work
{
	public class JobWorkerAssignmentMap : IEquatable<JobWorkerAssignmentMap>
	{
		public static readonly JobWorkerAssignmentMap Empty = new JobWorkerAssignmentMap();

		private readonly IImmutableDictionary<JobIdentifier, WorkerIdentifier> _assignments;

		private JobWorkerAssignmentMap() : this(ImmutableDictionary<JobIdentifier, WorkerIdentifier>.Empty) { }

		private JobWorkerAssignmentMap(IImmutableDictionary<JobIdentifier, WorkerIdentifier> assignments)
		{
			_assignments = assignments;
		}

		public Maybe<JobIdentifier> this[WorkerIdentifier worker]
		{
			get
			{
				return FindJob(worker);
			}
		}

		public Maybe<WorkerIdentifier> this[JobIdentifier job]
		{
			get
			{
				return _assignments.ContainsKey(job)
					? _assignments[job].ToMaybe()
					: Maybe<WorkerIdentifier>.Nothing;
			}
		}
		
		public JobWorkerAssignmentMap WithAssignment(JobIdentifier job, WorkerIdentifier worker) 
			=> new JobWorkerAssignmentMap(
				FindJob(worker).SelectOrElse(
					assignedJob => _assignments.Remove(assignedJob),
					() => _assignments.Remove(job)
				).Add(job, worker)
			);

		public JobWorkerAssignmentMap WithoutAssignment(JobIdentifier job)
			=> new JobWorkerAssignmentMap(_assignments.Remove(job));
		
		public JobWorkerAssignmentMap WithoutAssignment(WorkerIdentifier worker)
			=> FindJob(worker).SelectOrElse(
				job => WithoutAssignment(job),
				() => this
			);

		public override bool Equals(object obj)
			=> Equals(obj as JobWorkerAssignmentMap);

		public bool Equals(JobWorkerAssignmentMap other)
			=> other != null
			&& _assignments.Equals(other._assignments);

		private Maybe<JobIdentifier> FindJob(WorkerIdentifier worker)
			=> _assignments
				.Where(pair => pair.Value == worker)
				.Select(pair => pair.Key)
				.SingleMaybe();

		public override int GetHashCode() 
			=> _assignments.GetHashCode();

		public static bool operator ==(JobWorkerAssignmentMap map1, JobWorkerAssignmentMap map2) 
			=> EqualityComparer<JobWorkerAssignmentMap>.Default.Equals(map1, map2);

		public static bool operator !=(JobWorkerAssignmentMap map1, JobWorkerAssignmentMap map2) 
			=> !(map1 == map2);
	}
}
