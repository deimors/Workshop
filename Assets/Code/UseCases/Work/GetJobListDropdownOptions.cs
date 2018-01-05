using Functional.Maybe;
using System.Collections.Generic;
using System.Linq;
using Workshop.Domain.Work;
using Workshop.Models;

namespace Workshop.UseCases.Work
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

	public interface IGetJobDropdownOptions
	{
		IEnumerable<JobDropdownOption> Options { get; }
	}

	public class GetJobListDropdownOptions : IGetJobDropdownOptions
	{
		private readonly IReadJobList _readJobs;
		
		public GetJobListDropdownOptions(IReadJobList readJobs)
		{
			_readJobs = readJobs;
		}

		public IEnumerable<JobDropdownOption> Options
			=> StaticOptions().Concat(_readJobs.Keys.Select(job => new JobDropdownOption(job)));

		private IEnumerable<JobDropdownOption> StaticOptions()
		{
			yield return JobDropdownOption.None;
		}
	}
}
