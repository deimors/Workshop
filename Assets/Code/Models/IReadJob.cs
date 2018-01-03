using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IReadJob
	{
		IObservable<JobStatus> Status { get; }

		IObservable<bool> Busy { get; }
	}
}