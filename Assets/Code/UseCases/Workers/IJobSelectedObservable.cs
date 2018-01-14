using Functional.Maybe;
using System;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IJobSelectedObservable : IObservable<Maybe<JobIdentifier>> { }

}
