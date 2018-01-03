using System.Collections.Generic;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IReadWorkerList
	{
		IReadWorker this[WorkerIdentifier identifier] { get; }

		IEnumerable<WorkerIdentifier> Keys { get; }
		IEnumerable<IReadWorker> Values { get; }
	}
}
