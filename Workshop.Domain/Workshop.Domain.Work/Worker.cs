using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class Worker
	{
		public WorkerIdentifier Id { get; }
		public WorkerStatus Status { get; }
		public WorkerAttributes Attributes { get; }
		
		public Worker(WorkerIdentifier id, WorkerStatus status, WorkerAttributes attributes)
		{
			Id = id;
			Status = status;
			Attributes = attributes;
		}

		public Worker With(
			Func<WorkerIdentifier, WorkerIdentifier> id = null,
			Func<WorkerStatus, WorkerStatus> status = null,
			Func<WorkerAttributes, WorkerAttributes> attributes = null
		) => new Worker(
			(id ?? Function.Ident)(Id),
			(status ?? Function.Ident)(Status),
			(attributes ?? Function.Ident)(Attributes)
		);

		public static Worker NewDefault()
			=> new Worker(new WorkerIdentifier(), WorkerStatus.Default, WorkerAttributes.Default);
	}
}
