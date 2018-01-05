using Workshop.Domain.Work;
using Workshop.Models;
using Zenject;

namespace Workshop.UseCases.Work
{
	public class PerformWorkFactory : Factory<IWriteJob, IReadJob, IPerformWork> { }

	public interface IPerformAssignedWork
	{
		void Perform(WorkerIdentifier worker);
	}
}
