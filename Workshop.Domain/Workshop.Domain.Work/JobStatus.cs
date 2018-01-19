using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class JobStatus : IEquatable<JobStatus>
	{
		public static readonly JobStatus Default = new JobStatus(QuantityOfWork.Unit, QuantityOfWork.None, false);

		public QuantityOfWork Total { get; }

		public QuantityOfWork Completed { get; }

		public bool IsFinished => Total == Completed;

		public bool Busy { get; }

		public JobStatus(QuantityOfWork total, QuantityOfWork completed, bool busy)
		{
			if (total < completed)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(completed)}");

			if (total < QuantityOfWork.None)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(QuantityOfWork.None)}");
			
			Total = total ?? throw new ArgumentNullException(nameof(total));
			Completed = completed ?? throw new ArgumentNullException(nameof(completed));
			Busy = busy;
		}

		public JobStatus With(
			Func<QuantityOfWork, QuantityOfWork> total = null, 
			Func<QuantityOfWork, QuantityOfWork> completed = null,
			Func<bool, bool> busy = null
		) => new JobStatus(
			(total ?? Function.Ident)(Total),
			(completed ?? Function.Ident)(Completed),
			(busy ?? Function.Ident)(Busy)
		);

		public override bool Equals(object obj) 
			=> Equals(obj as JobStatus);

		public bool Equals(JobStatus other) 
			=> !(other is null) 
				&& Equals(Total, other.Total) 
				&& Equals(Completed, other.Completed) 
				&& Busy == other.Busy;

		public override int GetHashCode()
		{
			var hashCode = 766005519;
			hashCode = hashCode * -1521134295 + Total.GetHashCode();
			hashCode = hashCode * -1521134295 + Completed.GetHashCode();
			hashCode = hashCode * -1521134295 + Busy.GetHashCode();
			return hashCode;
		}
	}
}
