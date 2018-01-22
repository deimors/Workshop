using Functional.Maybe;
using System;
using UniRx;
using UnityEngine;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class WorkerSpeedReadModel
	{
		public float WorkerSpeed { get; private set; } = 1.0f;

		public WorkerSpeedReadModel(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerAdded>()
				.Where(workerAdded => workerAdded.Worker.Id == workerId)
				.Subscribe(workerAdded => WorkerSpeed = workerAdded.Worker.Attributes.Speed);
		}
	}

	public class WorkOnAssignedJobWhenWorkButtonClicked
	{
		private readonly AssignedJobReadModel _assignedJobModel;
		private readonly WorkerSpeedReadModel _workerSpeedModel;
		private readonly IEnqueueCommand<WorkshopCommand> _workshopCommands;
		private readonly IDisplayProgress _displayProgress;

		public WorkOnAssignedJobWhenWorkButtonClicked(AssignedJobReadModel assignedJobModel, WorkerSpeedReadModel workerSpeedModel, IWorkButtonClickedObservable workButtonClicked, IEnqueueCommand<WorkshopCommand> workshopCommands, IDisplayProgress displayProgress)
		{
			_assignedJobModel = assignedJobModel;
			_workerSpeedModel = workerSpeedModel;
			_workshopCommands = workshopCommands;
			_displayProgress = displayProgress;
			
			workButtonClicked.Subscribe(_ => MaybeWorkOnAssignedJob());
		}

		public void MaybeWorkOnAssignedJob()
			=> _assignedJobModel.AssignedJob.Match(WorkOnJob, () => { });

		private void WorkOnJob(JobIdentifier jobId)
		{
			StartWorkOnJob(jobId);
			CompleteWorkOnJobAfterDelay(jobId, 1 / _workerSpeedModel.WorkerSpeed);
		}
		
		private void StartWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.StartWork(jobId));

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId, float delaySeconds)
			=> CompleteWorkOnJobAfterDelay(jobId, Time.time, delaySeconds);

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
