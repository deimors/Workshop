using Assets.Code.UseCases.Work;
using System.Collections.Generic;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IReadWorkerList
	{
		IReadWorker this[WorkerIdentifier identifier] { get; }

		IEnumerable<WorkerIdentifier> Keys { get; }
		IEnumerable<IReadWorker> Values { get; }
	}
}
