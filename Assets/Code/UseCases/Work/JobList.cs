using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IWriteJobList
	{
		void Add(JobStatus job);

		void Remove(JobIdentifier jobIdentifier);
	}

	public class JobList : IObserveJobList, IReadJobList, IWriteJobList
	{
		private readonly IReactiveDictionary<JobIdentifier, Job> _jobs = new ReactiveDictionary<JobIdentifier, Job>();

		public IReadJob this[JobIdentifier identifier]
		{
			get
			{
				Job job;
				if (((IReadOnlyReactiveDictionary<JobIdentifier, Job>)_jobs).TryGetValue(identifier, out job))
					return job;
				else
					throw new KeyNotFoundException($"{identifier} not found");
			}
		}

		public IObservable<JobIdentifier> ObserveAdd
			=> _jobs.ObserveAdd().Select(addEvent => addEvent.Key);
		
		public IObservable<JobIdentifier> ObserveRemove 
			=> _jobs.ObserveRemove().Select(removeEvent => removeEvent.Key);

		public IEnumerable<JobIdentifier> Keys => _jobs.Keys;

		public IEnumerable<IReadJob> Values => _jobs.Values;

		public void Add(JobStatus job) 
			=> _jobs.Add(job.Id, new Job(job));

		public void Remove(JobIdentifier jobIdentifier) 
			=> _jobs.Remove(jobIdentifier);
	}
}
