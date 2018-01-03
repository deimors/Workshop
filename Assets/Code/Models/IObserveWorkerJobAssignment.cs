using System;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IObserveWorkerJobAssignment
	{
		IObservable<JobWorkerAssignmentMap> Assignments { get; }
	}
}
