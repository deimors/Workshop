using Functional.Maybe;
using System;
using System.Linq;
using System.Collections.Generic;
using Workshop.Domain.Work;

namespace Assets.Code.UseCases.Work
{
	public interface IReadWorkerJobAssignment
	{
		Maybe<WorkerIdentifier> this[JobIdentifier job] { get; }
		Maybe<JobIdentifier> this[WorkerIdentifier worker] { get; }
	}

	public interface IObserveWorkerJobAssignment
	{
		IObservable<Maybe<JobIdentifier>> Observe(WorkerIdentifier worker);
		IObservable<Maybe<WorkerIdentifier>> Observe(JobIdentifier job);
	}

	public interface IWriteWorkerJobAssignment
	{
		Maybe<WorkerIdentifier> this[JobIdentifier job] { set; }
		Maybe<JobIdentifier> this[WorkerIdentifier worker] { set; }
	}

	public class WorkerJobAssignmentMap : IReadWorkerJobAssignment, IWriteWorkerJobAssignment, IObserveWorkerJobAssignment
	{
		private readonly IDictionary<JobIdentifier, WorkerIdentifier> _map = new Dictionary<JobIdentifier, WorkerIdentifier>();

		Maybe<WorkerIdentifier> IReadWorkerJobAssignment.this[JobIdentifier job]
		{
			get
			{
				return _map.Lookup(job);
			}
		}

		Maybe<JobIdentifier> IReadWorkerJobAssignment.this[WorkerIdentifier worker]
		{
			get
			{
				return FindJob(worker);
			}
		}

		

		Maybe<WorkerIdentifier> IWriteWorkerJobAssignment.this[JobIdentifier job]
		{
			set
			{
				value.Match(
					worker => _map[job] = worker,
					() => _map.Remove(job)
				);
			}
		}

		Maybe<JobIdentifier> IWriteWorkerJobAssignment.this[WorkerIdentifier worker]
		{
			set
			{
				FindJob(worker).Match(job => _map.Remove(job), () => { });

				value.Match(job => _map[job] = worker, () => { });
			}
		}

		IObservable<Maybe<JobIdentifier>> IObserveWorkerJobAssignment.Observe(WorkerIdentifier worker)
		{
			throw new NotImplementedException();
		}

		IObservable<Maybe<WorkerIdentifier>> IObserveWorkerJobAssignment.Observe(JobIdentifier job)
		{
			throw new NotImplementedException();
		}

		private Maybe<JobIdentifier> FindJob(WorkerIdentifier worker)
			=> _map
				.Where(pair => pair.Value == worker)
				.Select(pair => pair.Key)
				.SingleMaybe();
	}
}
