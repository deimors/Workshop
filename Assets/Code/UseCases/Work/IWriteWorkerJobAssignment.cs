using Functional.Maybe;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IWriteWorkerJobAssignment
	{
		Maybe<WorkerIdentifier> this[JobIdentifier job] { set; }
		Maybe<JobIdentifier> this[WorkerIdentifier worker] { set; }
	}
}
