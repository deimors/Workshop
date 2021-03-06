﻿using Functional.Maybe;
using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class AssignedJobReadModel
	{
		public Maybe<JobIdentifier> AssignedJob { get; private set; } = Maybe<JobIdentifier>.Nothing;

		public AssignedJobReadModel(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.WorkerId == workerId)
				.Subscribe(jobAssigned => AssignedJob = jobAssigned.JobId.ToMaybe());

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.WorkerId == workerId)
				.Subscribe(_ => AssignedJob = Maybe<JobIdentifier>.Nothing);
		}
	}
}
