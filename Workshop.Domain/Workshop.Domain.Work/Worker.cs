using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class Worker
	{
		public WorkerIdentifier Id { get; }
		public WorkerStatus Status { get; }

		public Worker(WorkerIdentifier id, WorkerStatus status)
		{
			Id = id;
			Status = status;
		}

		public Worker With(
			Func<WorkerIdentifier, WorkerIdentifier> id = null,
			Func<WorkerStatus, WorkerStatus> status = null
		) => new Worker(
			(id ?? Function.Ident)(Id),
			(status ?? Function.Ident)(Status)
		);
	}
}
