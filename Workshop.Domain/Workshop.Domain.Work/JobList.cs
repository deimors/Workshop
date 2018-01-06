using Functional.Maybe;
using System.Collections.Generic;
using System.Collections.Immutable;
using Workshop.Core;

namespace Workshop.Domain.Work
{
	public class JobList
	{
		private static readonly JobList Empty = new JobList(ImmutableDictionary<JobIdentifier, Job>.Empty);

		private readonly IImmutableDictionary<JobIdentifier, Job> _jobs;

		private JobList(IImmutableDictionary<JobIdentifier, Job> jobs) => _jobs = jobs;

		public Maybe<Job> this[JobIdentifier jobId] => _jobs.Lookup(jobId);

		public IEnumerable<JobIdentifier> Identifiers => _jobs.Keys;

		public IEnumerable<Job> Jobs => _jobs.Values;

		public JobList Add(Job newJob)
			=> new JobList(_jobs.Add(newJob.Id, newJob));
	}
}
