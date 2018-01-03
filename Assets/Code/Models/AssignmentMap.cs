using Functional.Maybe;
using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public class AssignmentMap : IReadWorkerJobAssignment, IWriteWorkerJobAssignment, IObserveWorkerJobAssignment
	{
		private readonly IReactiveProperty<JobWorkerAssignmentMap> _assignmentSubject = new ReactiveProperty<JobWorkerAssignmentMap>(JobWorkerAssignmentMap.Empty);

		IObservable<JobWorkerAssignmentMap> IObserveWorkerJobAssignment.Assignments => _assignmentSubject.AsObservable();

		Maybe<WorkerIdentifier> IReadWorkerJobAssignment.this[JobIdentifier job] => _assignmentSubject.Value[job];

		Maybe<JobIdentifier> IReadWorkerJobAssignment.this[WorkerIdentifier worker] => _assignmentSubject.Value[worker];
		
		Maybe<WorkerIdentifier> IWriteWorkerJobAssignment.this[JobIdentifier job]
		{
			set
			{
				_assignmentSubject.Value = value.SelectOrElse(
					worker => _assignmentSubject.Value.WithAssignment(job, worker),
					() => _assignmentSubject.Value.WithoutAssignment(job)
				);
			}
		}

		Maybe<JobIdentifier> IWriteWorkerJobAssignment.this[WorkerIdentifier worker]
		{
			set
			{
				_assignmentSubject.Value = value.SelectOrElse(
					job => _assignmentSubject.Value.WithAssignment(job, worker),
					() => _assignmentSubject.Value.WithoutAssignment(worker)
				);
			}
		}
	}
}
