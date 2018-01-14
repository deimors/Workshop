using Functional.Maybe;
using System;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class WorkOnAssignedJobWhenWorkButtonClicked
	{
		private readonly IQueueWorkshopCommands _queueWorkshopCommands;

		public WorkOnAssignedJobWhenWorkButtonClicked(AssignedJobReadModel assignedJobModel, IWorkButtonClickedObservable workButtonClicked, IQueueWorkshopCommands queueWorkshopCommands)
		{
			_queueWorkshopCommands = queueWorkshopCommands;
			
			workButtonClicked
				.Select(_ => assignedJobModel.AssignedJob)
				.Subscribe(MaybeWorkOnJob);
		}

		public void MaybeWorkOnJob(Maybe<JobIdentifier> maybeJobId)
			=> maybeJobId.Match(WorkOnJob, () => { });

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
