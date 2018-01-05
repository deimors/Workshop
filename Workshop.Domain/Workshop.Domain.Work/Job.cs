using System;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class Job
	{
		public JobIdentifier Id { get; }

		public JobStatus Status { get; }

		public Job(JobIdentifier id, JobStatus status)
		{
			Id = id;
			Status = status;
		}

		public Job With(
			Func<JobIdentifier, JobIdentifier> id = null,
			Func<JobStatus, JobStatus> status = null
		) => new Job(
			(id ?? Function.Ident)(Id),
			(status ?? Function.Ident)(Status)
		);
	}
}
