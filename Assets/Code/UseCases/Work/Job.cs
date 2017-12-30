using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public class Job : IReadJob, IWriteJob
	{
		private readonly IReactiveProperty<JobStatus> _status = new ReactiveProperty<JobStatus>();
		IObservable<JobStatus> IReadJob.Status => _status;
		JobStatus IWriteJob.Status { set { _status.Value = value; } }

		private readonly IReactiveProperty<bool> _busy = new ReactiveProperty<bool>();
		IObservable<bool> IReadJob.Busy => _busy;
		bool IWriteJob.Busy { set { _busy.Value = value; } }

		public Job(JobStatus status)
		{
			_status.Value = status;
			_busy.Value = false;
		}
	}
}
