using System;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IObserveWorkerList
	{
		IObservable<WorkerIdentifier> ObserveAdd { get; }
		IObservable<WorkerIdentifier> ObserveRemove { get; }
	}
}
