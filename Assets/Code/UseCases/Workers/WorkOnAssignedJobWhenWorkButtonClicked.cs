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
		private readonly IEnqueueCommand<WorkshopCommand> _workshopCommands;

		public WorkOnAssignedJobWhenWorkButtonClicked(AssignedJobReadModel assignedJobModel, IWorkButtonClickedObservable workButtonClicked, IEnqueueCommand<WorkshopCommand> workshopCommands)
		{
			_workshopCommands = workshopCommands;
			
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
			=> _workshopCommands.Enqueue(new WorkshopCommand.StartWork(jobId));

		private void CompleteWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.CompleteWork(jobId, QuantityOfWork.Unit));
	}
}
