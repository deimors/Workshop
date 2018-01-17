using Functional.Maybe;
using System;
using UniRx;
using UnityEngine;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class WorkOnAssignedJobWhenWorkButtonClicked
	{
		private readonly TimeSpan _delay = TimeSpan.FromSeconds(2);

		private readonly IEnqueueCommand<WorkshopCommand> _workshopCommands;
		private readonly IDisplayProgress _displayProgress;

		public WorkOnAssignedJobWhenWorkButtonClicked(AssignedJobReadModel assignedJobModel, IWorkButtonClickedObservable workButtonClicked, IEnqueueCommand<WorkshopCommand> workshopCommands, IDisplayProgress displayProgress)
		{
			_workshopCommands = workshopCommands;
			_displayProgress = displayProgress;
			
			workButtonClicked
				.Select(_ => assignedJobModel.AssignedJob)
				.Subscribe(MaybeWorkOnJob);
		}

		public void MaybeWorkOnJob(Maybe<JobIdentifier> maybeJobId)
			=> maybeJobId.Match(WorkOnJob, () => { });

		private void WorkOnJob(JobIdentifier jobId)
		{
			StartWorkOnJob(jobId);
			CompleteWorkOnJobAfterDelay(jobId, _delay);
		}
		
		private void StartWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.StartWork(jobId));

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId, TimeSpan delay)
			=> CompleteWorkOnJobAfterDelay(jobId, Time.time, (float)delay.TotalSeconds);

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId, float startTime, float delaySeconds)
			=> Observable.EveryUpdate()
				.Take(TimeSpan.FromSeconds(delaySeconds))
				.Select(_ => (Time.time - startTime) / delaySeconds)
				.Select(progress => Mathf.Clamp01(progress) % 1f)
				.Subscribe(
					_displayProgress.ShowProgress,
					() => CompleteWorkOnJob(jobId)
				);

		private void CompleteWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.CompleteWork(jobId, QuantityOfWork.Unit));
	}
}
