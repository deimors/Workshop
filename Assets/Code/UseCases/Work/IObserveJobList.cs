using System;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IObserveJobList
	{
		IObservable<JobIdentifier> ObserveAdd { get; }
		IObservable<JobIdentifier> ObserveRemove { get; }
	}
}
