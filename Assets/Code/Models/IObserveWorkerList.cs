using System;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IObserveWorkerList
	{
		IObservable<WorkerIdentifier> ObserveAdd { get; }
		IObservable<WorkerIdentifier> ObserveRemove { get; }
	}
}
