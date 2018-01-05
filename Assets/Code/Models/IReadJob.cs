using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IReadJob
	{
		IObservable<Job> Value { get; }

		IObservable<bool> Busy { get; }
	}
}