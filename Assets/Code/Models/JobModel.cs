using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public class JobModel : IReadJob, IWriteJob
	{
		private readonly IReactiveProperty<Job> _job = new ReactiveProperty<Job>();
		IObservable<Job> IReadJob.Value => _job;
		Job IWriteJob.Value { set { _job.Value = value; } }

		private readonly IReactiveProperty<bool> _busy = new ReactiveProperty<bool>();
		IObservable<bool> IReadJob.Busy => _busy;
		bool IWriteJob.Busy { set { _busy.Value = value; } }

		public JobModel(Job job)
		{
			_job.Value = job;
			_busy.Value = false;
		}
	}
}
