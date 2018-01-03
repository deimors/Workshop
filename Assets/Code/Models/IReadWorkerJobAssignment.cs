using Functional.Maybe;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IReadWorkerJobAssignment
	{
		Maybe<WorkerIdentifier> this[JobIdentifier job] { get; }
		Maybe<JobIdentifier> this[WorkerIdentifier worker] { get; }
	}
}
