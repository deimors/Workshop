using Functional.Maybe;

namespace Workshop.Domain.Work
{
	public class JobWorkerAssignment
	{
		public JobIdentifier Id { get; }

		public Maybe<WorkerIdentifier> Assigned { get; }
	}
}
