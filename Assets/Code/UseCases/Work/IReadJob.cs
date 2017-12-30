using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IReadJob
	{
		IObservable<JobStatus> Status { get; }

		IObservable<bool> Busy { get; }
	}
}