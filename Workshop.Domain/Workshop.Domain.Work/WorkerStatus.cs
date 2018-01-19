using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class WorkerStatus : IEquatable<WorkerStatus>
	{
		public static readonly WorkerStatus Default = new WorkerStatus(false);

		public bool Busy { get; }

		public WorkerStatus(bool busy)
		{
			Busy = busy;
		}

		public WorkerStatus With(
			Func<bool, bool> busy = null
		) => new WorkerStatus(
			(busy ?? Function.Ident)(Busy)
		);

		public override bool Equals(object obj) 
			=> Equals(obj as WorkerStatus);

		public bool Equals(WorkerStatus other) 
			=> !(other is null) && Busy == other.Busy;

		public override int GetHashCode() 
			=> -1082567776 + Busy.GetHashCode();
	}
}
