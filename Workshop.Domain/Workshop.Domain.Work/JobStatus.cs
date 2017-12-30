using System;

namespace Workshop.Domain.Work
{
	public class JobStatus
	{
		public JobIdentifier Id { get; }

		public QuantityOfWork Total { get; }

		public QuantityOfWork Completed { get; }

		public JobStatus(JobIdentifier id, QuantityOfWork total, QuantityOfWork completed)
		{
			if (total < completed)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(completed)}");

			if (total < QuantityOfWork.None)
				throw new ArgumentOutOfRangeException(nameof(total), $"{nameof(total)} < {nameof(QuantityOfWork.None)}");

			Id = id ?? throw new ArgumentNullException(nameof(id));
			Total = total ?? throw new ArgumentNullException(nameof(total));
			Completed = completed ?? throw new ArgumentNullException(nameof(completed));
		}

		public static JobStatus Create(JobIdentifier id, QuantityOfWork total)
			=> new JobStatus(id, total, QuantityOfWork.None);

		public JobStatus With(
			Func<JobIdentifier, JobIdentifier> id = null,
			Func<QuantityOfWork, QuantityOfWork> total = null, 
			Func<QuantityOfWork, QuantityOfWork> completed = null
		) => new JobStatus(
				(id ?? Ident).Invoke(Id),
				(total ?? Ident).Invoke(Total),
				(completed ?? Ident).Invoke(Completed)
			);

		public static T Ident<T>(T item) => item;
	}
}
