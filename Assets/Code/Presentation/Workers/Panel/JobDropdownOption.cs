using Functional.Maybe;
using Workshop.Domain.Work;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobDropdownOption
	{
		public static readonly JobDropdownOption None = new JobDropdownOption();

		public Maybe<JobIdentifier> Job { get; }

		private JobDropdownOption()
		{
			Job = Maybe<JobIdentifier>.Nothing;
		}

		public JobDropdownOption(JobIdentifier job)
		{
			Job = job.ToMaybe();
		}

		public override string ToString()
			=> Job.SelectOrElse(job => job.ToString(), () => "None");
	}
}
