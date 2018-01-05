using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class JobStatus
	{
		public QuantityOfWork Total { get; }

		public QuantityOfWork Completed { get; }

		public JobStatus(QuantityOfWork total, QuantityOfWork completed)
		{
			if (total < completed)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(completed)}");

			if (total < QuantityOfWork.None)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(QuantityOfWork.None)}");
			
			Total = total ?? throw new ArgumentNullException(nameof(total));
			Completed = completed ?? throw new ArgumentNullException(nameof(completed));
		}

		public static JobStatus Create(QuantityOfWork total)
			=> new JobStatus(total, QuantityOfWork.None);

		public JobStatus With(
			Func<QuantityOfWork, QuantityOfWork> total = null, 
			Func<QuantityOfWork, QuantityOfWork> completed = null
		) => new JobStatus(
			(total ?? Function.Ident).Invoke(Total),
			(completed ?? Function.Ident).Invoke(Completed)
		);
	}
}
