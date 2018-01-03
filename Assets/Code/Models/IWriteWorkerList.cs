using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IWriteWorkerList
	{
		void Add(WorkerIdentifier worker);

		void Remove(WorkerIdentifier worker);
	}
}
