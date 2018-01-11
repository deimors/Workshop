using Functional.Maybe;
using System;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class PerformAssignedWorkFromAggregate : IPerformAssignedWork
	{
		private readonly JobsAssignedToWorkerReadModel _jobAssignments;
		private readonly JobStatusReadModel _jobStatuses;
		private readonly IWorkOnJob _workOnJob;
		private readonly IQueueWorkshopCommands _queueWorkshopCommands;

		public PerformAssignedWorkFromAggregate(JobsAssignedToWorkerReadModel jobAssignments, JobStatusReadModel jobStatuses, IWorkOnJob workOnJob, IQueueWorkshopCommands queueWorkshopCommands)
		{
			_jobAssignments = jobAssignments;
			_jobStatuses = jobStatuses;
			_workOnJob = workOnJob;
			_queueWorkshopCommands = queueWorkshopCommands;
		}

		public void Perform(WorkerIdentifier workerId)
			=> _jobAssignments[workerId].Match(WorkOnJob, () => { });

		private void WorkOnJob(JobIdentifier jobId)
		{
			StartWorkOnJob(jobId);
			CompleteWorkOnJobAfterDelay(jobId);
		}

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId)
			=> Observable.Timer(TimeSpan.FromSeconds(2))
				.Select(_ => jobId)
				.Subscribe(CompleteWorkOnJob);

		private void StartWorkOnJob(JobIdentifier jobId)
			=> _queueWorkshopCommands.QueueCommand(new WorkshopCommand.StartWork(jobId));

		private void CompleteWorkOnJob(JobIdentifier jobId)
			=> _queueWorkshopCommands.QueueCommand(new WorkshopCommand.CompleteWork(jobId, QuantityOfWork.Unit));
	}
}
