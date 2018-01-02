using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IWriteWorkerList
	{
		void Add(WorkerIdentifier worker);

		void Remove(WorkerIdentifier worker);
	}
}
