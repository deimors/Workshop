using System;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IObserveJobList
	{
		IObservable<JobIdentifier> ObserveAdd { get; }
		IObservable<JobIdentifier> ObserveRemove { get; }
	}
}
