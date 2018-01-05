using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{

	public class JobList : IObserveJobList, IReadJobList, IWriteJobList
	{
		private readonly IReactiveDictionary<JobIdentifier, JobModel> _jobs = new ReactiveDictionary<JobIdentifier, JobModel>();
		private IReadOnlyReactiveDictionary<JobIdentifier, JobModel> _readJobs => _jobs;

		IReadJob IReadJobList.this[JobIdentifier jobId] => _readJobs[jobId];

		IWriteJob IWriteJobList.this[JobIdentifier jobId] => _readJobs[jobId];

		public IObservable<JobIdentifier> ObserveAdd
			=> _jobs.ObserveAdd().Select(addEvent => addEvent.Key);
		
		public IObservable<JobIdentifier> ObserveRemove 
			=> _jobs.ObserveRemove().Select(removeEvent => removeEvent.Key);

		public IEnumerable<JobIdentifier> Keys => _jobs.Keys;

		public IEnumerable<IReadJob> Values => _jobs.Values;

		public void Add(Job job) 
			=> _jobs.Add(job.Id, new JobModel(job));

		public void Remove(JobIdentifier jobId) 
			=> _jobs.Remove(jobId);
	}
}
