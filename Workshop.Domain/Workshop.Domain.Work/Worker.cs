using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class Worker
	{
		public WorkerIdentifier Id { get; }

		public Worker(WorkerIdentifier id)
		{
			Id = id;
		}

		public Worker With(
			Func<WorkerIdentifier, WorkerIdentifier> id = null
		) => new Worker(
			(id ?? Function.Ident)(Id)
		);
	}
}
