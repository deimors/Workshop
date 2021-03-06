﻿namespace Workshop.Domain.Work.Aggregates
{
	public enum WorkshopError
	{
		WorkerAlreadyAdded,
		JobAlreadyAdded,
		UnknownWorker,
		UnknownJob,
		WorkerNotAssigned,
		WorkNotStarted,
		JobIsBusy,
		WorkerIsBusy,
		JobCompleted
	}
}
