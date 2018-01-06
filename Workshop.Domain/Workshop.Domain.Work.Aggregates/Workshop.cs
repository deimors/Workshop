using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Generic;
using Workshop.Core;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopEvent : OneOfBase<WorkshopEvent.WorkerAdded>
	{
		public class WorkerAdded : WorkshopEvent, IEquatable<WorkerAdded>
		{
			public WorkerIdentifier WorkerId { get; }

			public WorkerAdded(WorkerIdentifier workerId)
			{
				WorkerId = workerId;
			}

			public override bool Equals(object obj) 
				=> Equals(obj as WorkerAdded);

			public bool Equals(WorkerAdded other) 
				=> !(other is null) && WorkerId.Equals(other.WorkerId);

			public override int GetHashCode()
			{
				var hashCode = -456856358;
				hashCode = hashCode * -1521134295 + WorkerId.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(WorkerAdded added1, WorkerAdded added2) 
				=> added1?.Equals(added2) ?? added2 is null;

			public static bool operator !=(WorkerAdded added1, WorkerAdded added2) 
				=> !(added1 == added2);
		}
	}

	public enum WorkshopError
	{
		WorkerAlreadyAdded
	}

	public class Workshop : AggregateRoot<WorkshopEvent>
	{
		private readonly ICollection<WorkerIdentifier> _workers = new HashSet<WorkerIdentifier>();

		public Maybe<WorkshopError> AddWorker(WorkerIdentifier workerId)
		{
			if (_workers.Contains(workerId))
				return WorkshopError.WorkerAlreadyAdded.ToMaybe();

			_workers.Add(workerId);

			Record(new WorkshopEvent.WorkerAdded(workerId));

			return Maybe<WorkshopError>.Nothing;
		}

		protected override void ApplyEvent(WorkshopEvent @event)
			=> @event.Switch(
				workerAdded => _workers.Add(workerAdded.WorkerId)
			);
	}
}
