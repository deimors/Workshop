using System;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IObserveWorkerJobAssignment
	{
		IObservable<JobWorkerAssignmentMap> Assignments { get; }
	}
}
