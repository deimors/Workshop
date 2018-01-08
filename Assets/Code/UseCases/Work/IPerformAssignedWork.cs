using Workshop.Domain.Work;
using Zenject;

namespace Workshop.UseCases.Work
{
	public interface IPerformAssignedWork
	{
		void Perform(WorkerIdentifier worker);
	}
}
