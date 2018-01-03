using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{

	public class JobList : IObserveJobList, IReadJobList, IWriteJobList
	{
		private readonly IReactiveDictionary<JobIdentifier, Job> _jobs = new ReactiveDictionary<JobIdentifier, Job>();
		private IReadOnlyReactiveDictionary<JobIdentifier, Job> _readJobs => _jobs;

		IReadJob IReadJobList.this[JobIdentifier job] => _readJobs[job];

		IWriteJob IWriteJobList.this[JobIdentifier job] => _readJobs[job];

		public IObservable<JobIdentifier> ObserveAdd
			=> _jobs.ObserveAdd().Select(addEvent => addEvent.Key);
		
		public IObservable<JobIdentifier> ObserveRemove 
			=> _jobs.ObserveRemove().Select(removeEvent => removeEvent.Key);

		public IEnumerable<JobIdentifier> Keys => _jobs.Keys;

		public IEnumerable<IReadJob> Values => _jobs.Values;

		public void Add(JobStatus job) 
			=> _jobs.Add(job.Id, new Job(job));

		public void Remove(JobIdentifier job) 
			=> _jobs.Remove(job);
	}
}
