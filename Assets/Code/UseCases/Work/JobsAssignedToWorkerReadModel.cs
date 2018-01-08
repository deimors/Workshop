using Functional.Maybe;
using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class JobsAssignedToWorkerReadModel
	{
		private readonly IDictionary<WorkerIdentifier, Maybe<JobIdentifier>> _assignedJobs = new Dictionary<WorkerIdentifier, Maybe<JobIdentifier>>();

		public JobsAssignedToWorkerReadModel(IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Subscribe(jobAssigned => _assignedJobs[jobAssigned.WorkerId] = jobAssigned.JobId.ToMaybe());

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Subscribe(jobUnassigned => _assignedJobs[jobUnassigned.WorkerId] = Maybe<JobIdentifier>.Nothing);
		}

		public Maybe<JobIdentifier> this[WorkerIdentifier workerId] 
			=> _assignedJobs.Lookup(workerId);
	}
}
