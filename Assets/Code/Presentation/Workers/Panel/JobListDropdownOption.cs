using Functional.Maybe;
using UnityEngine.UI;
using Workshop.Domain.Work;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobListDropdownOption : Dropdown.OptionData
	{
		public static readonly JobListDropdownOption None = new JobListDropdownOption();

		public Maybe<JobIdentifier> JobId { get; }

		private JobListDropdownOption() : base("None")
		{
			JobId = Maybe<JobIdentifier>.Nothing;
		}

		public JobListDropdownOption(JobIdentifier jobId) : base(jobId.ToString())
		{
			JobId = jobId.ToMaybe();
		}
	}
}
